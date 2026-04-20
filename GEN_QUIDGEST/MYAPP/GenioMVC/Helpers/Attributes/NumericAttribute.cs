
namespace GenioMVC.Helpers
{
	[AttributeUsage(AttributeTargets.Property)]
	public class NumericAttribute : Attribute
	{
		public int Decimals { get; set; }

		public NumericAttribute(int decimalDigits)
		{
			Decimals = decimalDigits;
		}

	}
}
