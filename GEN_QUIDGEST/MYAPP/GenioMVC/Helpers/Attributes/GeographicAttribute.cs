namespace GenioMVC.Helpers
{
	[AttributeUsage(AttributeTargets.Property)]
	public class GeographicAttribute : Attribute
	{
		private readonly string type;

		public enum GeographicEnum
		{
			Coordinate, Shape, Geometric, Undefined
		}

		public GeographicEnum Type
		{
			get
			{
				switch (type)
				{
					case "GG":
						return GeographicEnum.Coordinate;
					case "GS":
						return GeographicEnum.Shape;
					case "GC":
						return GeographicEnum.Geometric;
					default:
						return GeographicEnum.Undefined;
				}
			}
		}

		public GeographicAttribute(string type)
		{
			this.type = type;
		}
	}
}
