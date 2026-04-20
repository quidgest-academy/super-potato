using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using NetTopologySuite;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;

namespace CSGenio.framework.Geography
{
	public class GeographicData
	{
		public const int SRID = 4326;

		private readonly ICollection<IGeometry> geoCollection;

		public GeographicData() : this("") { }

		public GeographicData(string layerName) : this(layerName, new List<GeographicShape>()) { }

		public GeographicData(string layerName, ICollection<GeographicShape> shapes)
		{
			this.layerName = layerName;
			this.shapes = shapes ?? new List<GeographicShape>();
			geoCollection = new LinkedList<IGeometry>();
		}

		public string layerName { get; set; }

		public ICollection<GeographicShape> shapes { get; set; }

		private IGeometry CreatePolyline(IGeometryFactory gf, GeographicShape shape)
		{
			if (shape == null || shape.latlngs == null || shape.type != GeographicShape.LINESTRING)
				return null;

			var coords = new List<Coordinate>(shape.latlngs.Count);

			foreach (GeographicPoint point in shape.latlngs)
			{
				var coord = new Coordinate(point.lng, point.lat);
				coords.Add(coord);
			}

			return gf.CreateLineString(coords.ToArray());
		}

		private IGeometry CreatePolygon(IGeometryFactory gf, GeographicShape shape)
		{
			if (shape == null || shape.shapeParts == null || !shape.shapeParts.Any() || shape.type != GeographicShape.POLYGON && shape.type != GeographicShape.RECTANGLE)
				return null;

			var shellCoords = new List<Coordinate>(shape.shapeParts.First().Count);

			foreach (GeographicPoint point in shape.shapeParts.First())
			{
				var coord = new Coordinate(point.lng, point.lat);
				shellCoords.Add(coord);
			}

			ILinearRing shell = gf.CreateLinearRing(shellCoords.ToArray());

			var holes = new List<ILinearRing>(shape.shapeParts.Count - 1);
			var shapeParts = shape.shapeParts.ToList();

			for (int i = 1; i < shapeParts.Count; i++)
			{
				ICollection<GeographicPoint> latlngs = shapeParts[i];
				var holeCoords = new List<Coordinate>(latlngs.Count);

				foreach (GeographicPoint point in latlngs)
				{
					var coord = new Coordinate(point.lng, point.lat);
					holeCoords.Add(coord);
				}

				ILinearRing hole = gf.CreateLinearRing(holeCoords.ToArray());
				holes.Add(hole);
			}

			return gf.CreatePolygon(shell, holes.ToArray());
		}

		public IEnumerable<IGeometry> GetGeometryCollection()
		{
			/*
				A lock was added to mitigate an issue where, in the case of a dozen simultaneous requests 
					for GetGlob, which is cached and contains geography fields, sometimes a corrupted element 
					with a null value was added while cloning the glob.
			*/
			lock (geoCollection)
			{
				if (!geoCollection.Any() && shapes.Any())
				{
					IGeometryFactory gf = NtsGeometryServices.Instance.CreateGeometryFactory(SRID);

					foreach (GeographicShape shape in shapes)
					{
						switch (shape.type)
						{
							case GeographicShape.LINESTRING:
								IGeometry polyline = CreatePolyline(gf, shape);
								if (polyline != null)
									geoCollection.Add(polyline);
								break;
							case GeographicShape.POLYGON:
							case GeographicShape.RECTANGLE:
								IGeometry polygon = CreatePolygon(gf, shape);
								if (polygon != null)
									geoCollection.Add(polygon);
								break;
							case GeographicShape.POINT:
								if (shape.latlng != null)
								{
									double lat = shape.latlng.lat;
									double lng = shape.latlng.lng;

									var coord = new Coordinate(lng, lat);
									IPoint point = gf.CreatePoint(coord);
									geoCollection.Add(point);
								}
								break;
							default:
								throw new NotImplementedException();
						}
					}
				}
			}

			return geoCollection;
		}

		public override string ToString()
		{
			if (!shapes.Any())
				return string.Empty;

			IGeometryFactory gf = NtsGeometryServices.Instance.CreateGeometryFactory(SRID);
			var geometries = GetGeometryCollection();

			IGeometryCollection collection = gf.CreateGeometryCollection(geometries.ToArray());
			var geoWriter = new WKTWriter();

			return geoWriter.Write(collection);
		}

		private static string GetShapeType(string type)
		{
			switch (type.ToUpper())
			{
				case "POINT":
					return GeographicShape.POINT;
				case "LINESTRING":
					return GeographicShape.LINESTRING;
				default:
					return type.ToLower();
			}
		}

		private static void PopulateShapeCoords(IGeometry geometry, GeographicData geoData)
		{
			var shape = new GeographicShape(GetShapeType(geometry.GeometryType));

			if (geometry is Point)
			{
				double lat = geometry.Coordinate.CoordinateValue.Y;
				double lng = geometry.Coordinate.CoordinateValue.X;

				shape.latlng = new GeographicPoint(lat, lng);
			}
			else if (geometry is LineString)
			{
				foreach (Coordinate point in geometry.Coordinates)
				{
					double lat = point.CoordinateValue.Y;
					double lng = point.CoordinateValue.X;

					var coord = new GeographicPoint(lat, lng);
					if (shape.latlngs == null)
						shape.latlngs = new List<GeographicPoint>(geometry.Coordinates.Count());
					shape.latlngs.Add(coord);
				}
			}
			else if (geometry is Polygon polygon)
			{
				var shellCoords = new List<GeographicPoint>(polygon.Shell.Coordinates.Count());

				foreach (Coordinate point in polygon.Shell.Coordinates)
				{
					double lat = point.CoordinateValue.Y;
					double lng = point.CoordinateValue.X;

					var coord = new GeographicPoint(lat, lng);
					shellCoords.Add(coord);
				}

				if (shape.shapeParts == null)
					shape.shapeParts = new List<ICollection<GeographicPoint>>(polygon.Holes.Count() + 1);
				shape.shapeParts.Add(shellCoords);

				foreach (ILinearRing hole in polygon.Holes)
				{
					var holeCoords = new List<GeographicPoint>(hole.Coordinates.Count());

					foreach (Coordinate point in hole.Coordinates)
					{
						double lat = point.CoordinateValue.Y;
						double lng = point.CoordinateValue.X;

						var coord = new GeographicPoint(lat, lng);
						holeCoords.Add(coord);
					}

					shape.shapeParts.Add(holeCoords);
				}
			}

			geoData.shapes.Add(shape);
		}

		private static void PopulateShapesData(IGeometry geometries, GeographicData geoData)
		{
			if (geometries is GeometryCollection collection)
			{
				foreach (var geometry in collection.Geometries)
				{
					if (geometry is GeometryCollection)
						PopulateShapesData(geometry, geoData);
					else
						PopulateShapeCoords(geometry, geoData);
				}
			}
			else
				PopulateShapeCoords(geometries, geoData);
		}

		public static GeographicData GetGeographyFromText(string text)
		{
			if (string.IsNullOrWhiteSpace(text) || text == "Null")
				return null;

			var geoReader = new WKTReader();
			IGeometry readData = geoReader.Read(text);

			var geoData = new GeographicData();
			PopulateShapesData(readData, geoData);

			return geoData;
		}

		public static GeographicPoint GetPointFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == "Null")
                return null;

            var geoReader = new WKTReader();
            IGeometry readData = geoReader.Read(text);

			if (readData is Point)
			{
				double lat = readData.Coordinate.CoordinateValue.Y;
				double lng = readData.Coordinate.CoordinateValue.X;
				return new GeographicPoint(lat, lng);
			}

			return null;
        }

		public static ICollection<GeographicShape> SplitGeometry(GeographicData geometry)
		{
			return geometry?.shapes ?? new List<GeographicShape>();
		}

		public static GeographicData JoinGeometries(ICollection<GeographicShape> geometries)
		{
			return new GeographicData("", geometries);
		}
	}
}