using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CSGenio.framework.Geography.Print
{
	public class GeographyPrint
	{
		private static GraphicsPath ToGraphicsPath(IGeometry geometry)
		{
			var path = new GraphicsPath();

			foreach (var coordinate in geometry.Coordinates)
			{
				var point = new PointF((float)coordinate.X, (float)coordinate.Y);
				path.AddLine(point, point);
			}

			return path;
		}

		/// <summary>
		/// Exports the provided geographic data into a base 64 image with the specified width and height
		/// </summary>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The height of the image</param>
		/// <param name="geographicData">The geographic data to export</param>
		/// <returns>A string with the base 64 image representation of the provided geographic data</returns>
		public static string GetGeometriesAsBase64Image(int width, int height, GeographicData geographicData)
		{
			return GetGeometriesAsBase64Image(width, height, geographicData.GetGeometryCollection());
		}

		/// <summary>
		/// Exports the provided geometries into a base 64 image with the specified width and height
		/// </summary>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The height of the image</param>
		/// <param name="geometries">A collection of geometries to export</param>
		/// <returns>A string with the base 64 image representation of the provided geometries</returns>
		public static string GetGeometriesAsBase64Image(int width, int height, IEnumerable<IGeometry> geometries)
		{
			MemoryStream imageStream = GetGeometriesAsStream(width, height, geometries);

			// Convert the MemoryStream to a byte array.
			byte[] imageBytes = imageStream.ToArray();

			// Convert the byte array to a base64-encoded string.
			string base64Image = Convert.ToBase64String(imageBytes);

			return $"data:image/png;base64,{base64Image}";
		}

		/// <summary>
		/// Exports the provided geographic data into an image with the specified width and height
		/// </summary>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The height of the image</param>
		/// <param name="geographicData">The geographic data to export</param>
		/// <returns>A stream with the image of the provided geographic data</returns>
		public static MemoryStream GetGeometriesAsStream(int width, int height, GeographicData geographicData)
		{
			return GetGeometriesAsStream(width, height, geographicData.GetGeometryCollection());
		}

		/// <summary>
		/// Exports the provided geometries into an image with the specified width and height
		/// </summary>
		/// <param name="width">The width of the image</param>
		/// <param name="height">The height of the image</param>
		/// <param name="geometries">A collection of geometries to export</param>
		/// <returns>A stream with the image of the provided geometries</returns>
		public static MemoryStream GetGeometriesAsStream(int width, int height, IEnumerable<IGeometry> geometries)
		{
			float margin = 8.0f;

			// Create a new bitmap.
			Bitmap bitmap = new Bitmap(width, height);

			// Create a Graphics object from the bitmap.
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				// Fill the background with the white color.
				graphics.Clear(Color.White);

				// Calculate the bounding box of all geometries.
				Envelope envelope = new Envelope();
				foreach (var geometry in geometries)
					envelope.ExpandToInclude(geometry.EnvelopeInternal);

				// Calculate the scale factor to fit the bounding box into the canvas.
				float scaleFactor = (float)Math.Min((width - 2 * margin) / envelope.Width, (height - 2 * margin) / envelope.Height);

				// Calculate the translation to adjust the geometries within the canvas with a margin.
				float translateX = margin / scaleFactor - (float)envelope.MinX;
				float translateY = margin / scaleFactor - (float)envelope.MinY;

				// Create a transformation matrix to scale and translate geometries.
				Matrix transformMatrix = new Matrix();
				transformMatrix.Scale(scaleFactor, scaleFactor);
				transformMatrix.Translate(translateX, translateY);

				// Apply the transformation to the Graphics object.
				graphics.Transform = transformMatrix;

				// Get the scaling factors from the transformation matrix.
				float[] matrixElements = transformMatrix.Elements;
				float xScale = matrixElements[0];
				float yScale = matrixElements[3];

				// Calculate the adjusted pen thickness.
				float penThickness = 2.5f / Math.Min(xScale, yScale);

				// Draw each geometry.
				foreach (var geometry in geometries)
				{
					// The geometries are currently drawn in red.
					Pen pen = new Pen(Color.Red, penThickness);

					// Convert NTS geometry to a GraphicsPath.
					GraphicsPath graphicsPath = ToGraphicsPath(geometry);

					// Draw the GraphicsPath.
					graphics.DrawPath(pen, graphicsPath);
				}
			}

			// The drawn image gets mirrored, so we need to rotate it.
			bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

			// Save the bitmap to a MemoryStream.
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Png);
			memoryStream.Seek(0, SeekOrigin.Begin);

			return memoryStream;
		}
	}
}
