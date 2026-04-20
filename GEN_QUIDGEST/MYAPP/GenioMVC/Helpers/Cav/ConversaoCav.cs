using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

using CSGenio.framework;

namespace GenioMVC.Helpers.Cav
{
	public class ConversaoCav
	{
		private static NumberFormatInfo provider = InitProvider();

		private static NumberFormatInfo InitProvider()
		{
			NumberFormatInfo res = new NumberFormatInfo();
			res.NumberDecimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
			res.NumberGroupSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator;
			return res;
		}

		public static object ToInterno(string value, FieldFormatting format)
		{
			switch (format)
			{
				case FieldFormatting.TEMPO:
				case FieldFormatting.CARACTERES:
					return value;
				case FieldFormatting.GUID:
					return value;
				case FieldFormatting.INTEIRO:
				case FieldFormatting.LOGICO:
					return Int32.Parse(value);
				case FieldFormatting.FLOAT:
					return Decimal.Parse(value, provider);
				case FieldFormatting.DATAHORA:
				case FieldFormatting.DATA:
				case FieldFormatting.DATASEGUNDO:
					return DateTime.Parse(value);
				default:
					throw new Exception("Unknown format in ConversaoCav.");
			}
		}

		/// <summary>
		/// Função de conversão de dados em formato interno para string
		/// </summary>
		/// <param name="value">valor</param>
		/// <param name="meta">fonte de metadados</param>
		/// <param name="fieldId">id do campo</param>
		/// <returns>Valor convertido</returns>
		public static string FromInterno(object value, XmlCavService meta, ResultType type, string lang)
		{
			switch (type.ForCampo)
			{
				case FieldFormatting.TEMPO:
				case FieldFormatting.GUID:
					return FromString(value);
				case FieldFormatting.CARACTERES:
					if (type.IsArray())
						return FromArray(value, meta, type.ArrayClassName, lang);
					else
						return FromString(value);
				case FieldFormatting.INTEIRO:
					return FromInteger(value);
				case FieldFormatting.LOGICO:
					if (value == null || value == DBNull.Value)
						return "";
					string valor = FromInteger(value);
					valor = valor == "0" ? "Falso" : valor == "1" ? "Verdadeiro" : valor;
					return valor;
				case FieldFormatting.FLOAT:
					if (type.IsArray())
						return FromArray(value, meta, type.ArrayClassName, lang);
					return FromDouble(value);
				case FieldFormatting.DATAHORA:
				case FieldFormatting.DATA:
				case FieldFormatting.DATASEGUNDO:
					return FromDateTime(value, type.ForCampo);
				default:
					throw new Exception("Unknown format in ConversaoCav.");
			}
		}

		public static string FromArray(object value, XmlCavService meta, string array, string lang)
		{
			string id = (value == null || value == DBNull.Value) ? "" : value.ToString();

			if (string.IsNullOrEmpty(id))
				return "";

			List<Models.Cav.FieldArray> desc = meta.GetListArrayElement(array, lang);
			if (desc != null)
			{
				var arrayElement = desc.First(elarr => elarr.Id == id);
				if (arrayElement != null)
					return arrayElement.Description;
			}

			return id;
		}

		public static string FromDouble(object value)
		{
			if (value == null || value == DBNull.Value)
				value = 0m;

			return Convert.ToDecimal(value).ToString(provider);
		}

		public static string FromInteger(object value)
		{
			if (value == null || value == DBNull.Value)
				value = 0;

			return Convert.ToInt32(value).ToString(provider);
		}

		public static string FromString(object value)
		{
			if (value == null || value == DBNull.Value)
				return "";

			return value.ToString();
		}

		public static string FromDateTime(object value, FieldFormatting format)
		{
			if (value == null || value == DBNull.Value)
				return "";

			switch (format)
			{
				case FieldFormatting.DATAHORA:
					return Convert.ToDateTime(value).ToString("dd/MM/yyyy HH:mm");
				case FieldFormatting.DATA:
					return Convert.ToDateTime(value).ToString("dd/MM/yyyy");
				case FieldFormatting.DATASEGUNDO:
					return Convert.ToDateTime(value).ToString("dd/MM/yyyy HH:mm:ss");
				default:
					throw new Exception("Unknown format in ConversaoCav (FromDateTime).");
			}
		}
	}
}
