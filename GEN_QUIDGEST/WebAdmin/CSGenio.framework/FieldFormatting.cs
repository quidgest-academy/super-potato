using System;
using System.Collections.Generic;

namespace CSGenio.framework
{
	/// <summary>
	/// Classe de enumerados com as formatações de Qfield existentes e válidas.
	/// </summary>
	public enum FieldFormatting
	{
		CARACTERES,
		GUID,
		INTEIRO,
		DATA,
		DATAHORA,
		LOGICO,
		FLOAT,
		JPEG,
		BINARIO,
		DATASEGUNDO,
		TEMPO,
		GEOGRAPHY,
		GEO_SHAPE,
		GEOMETRIC,
		ENCRYPTED
	}
	
	/// <summary>
	/// Field formatter
	/// </summary>
	public abstract class FieldFormatter
	{
		public FieldFormatter() { }
	}

	/// <summary>
	/// Numeric Field formatter
	/// </summary>
	public class NumericFieldFormatter : FieldFormatter
	{
		/// <summary>
		/// Decimal separator
		/// </summary>
		public string DecimalSeparator { get; }

		/// <summary>
		/// Thousands separator
		/// </summary>
		public string GroupSeparator { get; }

        /// <summary>
		/// Negative format
		/// </summary>
		public string NegativeFormat { get; }

		public NumericFieldFormatter(string decimalSeparator, string groupSeparator, string negativeFormat)
		{
			DecimalSeparator = decimalSeparator;
			GroupSeparator = groupSeparator;
			NegativeFormat = negativeFormat;
		}
	}

	/// <summary>
	/// DateTime Field formatter
	/// </summary>
	public class DateTimeFieldFormatter : FieldFormatter
	{
		/// <summary>
		/// Format strings given when creating the object
		/// </summary>
		private string[] givenFormatStrings;

		/// <summary>
		/// All possible format strings, accounting for variations in number of digits
		/// </summary>
		public string[] FormatStrings
		{
			get { return givenFormatStrings; }
		}

		public DateTimeFieldFormatter(string formatString)
		{
			givenFormatStrings = new string[] { formatString };
		}

		public DateTimeFieldFormatter(string[] formatStrings)
		{
			givenFormatStrings = formatStrings;
		}

		/// <summary>
		/// Parse string value to a DateTime object using the format strings of this object
		/// </summary>
		public DateTime parseStringValue(string value)
		{
			if (string.IsNullOrEmpty(value))
				return DateTime.MinValue;
			
			return DateTime.ParseExact(value, FormatStrings, null, System.Globalization.DateTimeStyles.None);
		}
	}
}