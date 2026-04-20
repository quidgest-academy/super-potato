using System.Globalization;

namespace GenioMVC.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
	public class CurrencyAttribute : Attribute
	{
		private CurrencyEnum baseCurrency;
		private int decimalDigits;

		public CurrencyAttribute(string currency, int decimalDigits)
		{
			baseCurrency = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), currency);
			this.decimalDigits = decimalDigits;
		}

		public enum CurrencyEnum
		{
			AOA, AUD, BRL, CAD, CNY, CVE, EUR, GBP, JPY, MZN, NZD, PLN, USD, ZAR, STN
		}

		public static string GetCurrencySymbol(string currency)
		{
			var _currency = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), currency);
			return GetCurrencySymbol(_currency);
		}

		public static string GetCurrencySymbol(CurrencyEnum currency)
		{
			switch (currency)
			{
				case CurrencyEnum.AOA:
					return "Kz";
				case CurrencyEnum.AUD:
				case CurrencyEnum.CAD:
				case CurrencyEnum.CVE:
				case CurrencyEnum.NZD:
				case CurrencyEnum.USD:
					return "$";
				case CurrencyEnum.BRL:
					return "R$";
				case CurrencyEnum.CNY:
				case CurrencyEnum.JPY:
					return "¥";
				case CurrencyEnum.EUR:
					return "€";
				case CurrencyEnum.GBP:
					return "£";
				case CurrencyEnum.MZN:
					return "MT";
				case CurrencyEnum.PLN:
					return "zł";
				case CurrencyEnum.ZAR:
					return "R";
				case CurrencyEnum.STN:
					return "Db";
				default:
					throw new CurrencyNotImplementedException(string.Format("The currency {0} is not implemented by the CurrencyAttribute class.", currency));
			}
		}

		public CultureInfo GetCurrencyWithCurrentCulture(CultureInfo culture)
		{
			string symbol = GetCurrencySymbol(baseCurrency);

			culture.NumberFormat.CurrencySymbol = symbol;
			culture.NumberFormat.CurrencyDecimalDigits = decimalDigits;

			return culture;
		}
	}

	class CurrencyNotImplementedException : Exception
	{
		public CurrencyNotImplementedException(string msg) : base(msg) { }
	}
}
