using System;
using System.Collections.Generic;
using System.Text;

namespace CSGenio.framework.Geography
{
	public class GeographicShape
	{
		// The supported shape types.
		public const string LINESTRING = "polyline";
		public const string POLYGON = "polygon";
		public const string RECTANGLE = "rectangle";
		public const string CIRCLE = "circle";
		public const string CIRCLE_MARKER = "circlemarker";
		public const string POINT = "marker";

		public GeographicShape() : this(null) { }

		public GeographicShape(string type)
		{
			if (!IsValidType(type))
				throw new NotImplementedException();

			this.type = type;
		}

		public string type { get; set; }

		public GeographicPoint center { get; set; }

		public double radius { get; set; }

		public ICollection<GeographicPoint> latlngs { get; set; }

		public ICollection<ICollection<GeographicPoint>> shapeParts { get; set; }

		public GeographicPoint latlng { get; set; }

		private bool IsValidType(string type)
		{
			if (type == null)
				return true;

			switch (type.ToLower())
			{
				case LINESTRING:
				case POLYGON:
				case RECTANGLE:
				case CIRCLE:
				case CIRCLE_MARKER:
				case POINT:
					return true;
				default:
					return false;
			}
		}
	}
}