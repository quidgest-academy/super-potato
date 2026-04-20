using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace GenioMVC.Helpers.Culture
{
	public static class CultureManager
	{
		const string EngCultureName = "en-us";

		public const string DefaultCultureName = EngCultureName;

		static CultureInfo DefaultCulture
		{
			get
			{
				return SupportedCultures[DefaultCultureName];
			}
		}

		static Dictionary<string, CultureInfo> SupportedCultures { get; set; }

		public static System.Collections.ObjectModel.ReadOnlyDictionary<string, CultureInfo> SupportedCultures2
		{
			get { return new System.Collections.ObjectModel.ReadOnlyDictionary<string, CultureInfo>(SupportedCultures); }
		}

		static void AddSupportedCulture(string name)
		{
			SupportedCultures.Add(name, CultureInfo.CreateSpecificCulture(name));
		}

		static void InitializeSupportedCultures()
		{
			SupportedCultures = new Dictionary<string, CultureInfo>();
			AddSupportedCulture(EngCultureName);
		}

		static string ConvertToShortForm(string code)
		{
			return code.Substring(0, 2);
		}

		public static bool CultureIsSupported(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
				return false;
			code = code.ToLowerInvariant();
			if (code.Length == 2)
				return SupportedCultures.ContainsKey(code);
			return CultureFormatChecker.FormattedAsCulture(code) && SupportedCultures.ContainsKey(code);
		}

		static CultureInfo GetCulture(string code)
		{
			if (!CultureIsSupported(code))
				return DefaultCulture;
			string shortForm = code.ToLowerInvariant();
			return SupportedCultures[shortForm];
		}

		public static void SetCulture(string code)
		{
			CultureInfo cultureInfo = GetCulture(code);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			Thread.CurrentThread.CurrentCulture = cultureInfo;
		}

		static CultureManager()
		{
			InitializeSupportedCultures();
		}
	}
}
