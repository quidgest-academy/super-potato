using System;
using System.Collections.Generic;
using System.Text;

namespace CSGenio.framework.Geography
{
	public class GeographicPoint
	{
		public GeographicPoint() : this(0, 0) { }

		public GeographicPoint(double lat, double lng)
		{
			this.lat = lat;
			this.lng = lng;
		}

		public double lat { get; set; }

		public double lng { get; set; }

		public override string ToString()
		{
			return $"POINT({lng} {lat})";
		}
	}
}