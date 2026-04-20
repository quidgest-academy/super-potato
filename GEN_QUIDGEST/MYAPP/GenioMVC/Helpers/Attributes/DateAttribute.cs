namespace GenioMVC.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
	public class DateAttribute : Attribute
	{
		private readonly string type;

		public enum DateEnum
		{
			Date, DateTime, DateTimeSeconds, Time, Undefined
		}

		public DateEnum Type
		{
			get
			{
				switch (type)
				{
					case "D":
					case "OD":
					case "ED":
						return DateEnum.Date;
					case "DS":
					case "OI":
						return DateEnum.DateTimeSeconds;
					case "DT":
						return DateEnum.DateTime;
					case "OT":
					case "ET":
					case "T":
						return DateEnum.Time;
					default:
						return DateEnum.Undefined;
				}
			}
		}

		public DateAttribute(string type)
		{
			this.type = type;
		}

		public static DateEnum ConvertToDateAttribute(Attribute a) {
			if (a == null)
				return DateEnum.Undefined;
			DateAttribute tmp = a as DateAttribute;
			return tmp.Type;
		}
	}
}
