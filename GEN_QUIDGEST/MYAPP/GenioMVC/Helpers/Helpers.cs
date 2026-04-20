using CSGenio.framework;
using GenioMVC.Models.Navigation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GenioMVC.Helpers
{
	public static class HtmlHelpers
	{
		public static CultureInfo GetNumericCulture()
		{
			CultureInfo ci = System.Globalization.CultureInfo.CurrentUICulture;

			if (Configuration.NumberFormat != null)
			{
				ci.NumberFormat.NumberDecimalSeparator = Configuration.NumberFormat.DecimalSeparator ?? CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
				ci.NumberFormat.NumberGroupSeparator = Configuration.NumberFormat.GroupSeparator ?? CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator;
				ci.NumberFormat.CurrencyDecimalSeparator = Configuration.NumberFormat.DecimalSeparator ?? CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
				ci.NumberFormat.CurrencyGroupSeparator = Configuration.NumberFormat.GroupSeparator ?? CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator;
			}

			return ci;
		}

		/// <summary>
		/// Date value for selection menu between limits, taking into account the year of the database.
		/// </summary>
		/// <param name="type">SE limit type</param>
		/// <returns></returns>
		public static DateTime? GetBetweenLimitsDateValue(string type, int Qyear=0)
		{
			var now = DateTime.Now; now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Unspecified);

			if (Qyear == 0)
				Qyear = now.Year;

			if (Qyear != now.Year)
				now = new DateTime(Qyear, 12, 31, 23, 59, 59, DateTimeKind.Unspecified);

			switch (type)
			{
				case "T":
				case "HJ":
					return new DateTime(Qyear, now.Month, now.Day, now.Hour, now.Minute, now.Second);
				case "HJ-":
					return new DateTime(Qyear, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddDays(-1);
				case "HJ+":
					return new DateTime(Qyear, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddDays(1);
				case "1A":
					return new DateTime(Qyear, 1, 1);
				case "UA":
					return new DateTime(Qyear, 12, 31);
				case "1M":
					return new DateTime(Qyear, now.Month, 1);
				case "UM":
					return now.Month == 12 ? new DateTime(Qyear, 12, 31) : new DateTime(Qyear, now.Month + 1, 1).AddDays(-1);
				case "1T":
					return new DateTime(Qyear, ((now.Month - 1) / 3) * 3 + 1, 1);
				case "UT":
					return now.Month >= 10 ? new DateTime(Qyear, 12, 31) : new DateTime(Qyear, ((now.Month + 2) / 3) * 3 + 1, 1).AddDays(-1);
				case "1M-":
					return DateTime.Now.Month == 1 ? new DateTime(Qyear - 1, 12, 1) : new DateTime(Qyear, now.Month - 1, 1);
				case "UM-":
					return DateTime.Now.Month == 1 ? new DateTime(Qyear - 1, 12, 31) : new DateTime(Qyear, now.Month, 1).AddDays(-1);
				case "1M+":
					return DateTime.Now.Month == 12 ? new DateTime(Qyear + 1, 1, 1) : new DateTime(Qyear, now.Month + 1, 1);
				case "UM+":
					return now.Month == 12 ? new DateTime(Qyear + 1, 1, 31) : (now.Month == 11 ? new DateTime(Qyear, 12, 31) : new DateTime(Qyear, now.Month + 2, 1).AddDays(-1));
				case "1T-":
					return DateTime.Now.Month <= 3 ? new DateTime(Qyear - 1, 10, 1) : new DateTime(Qyear, ((now.Month - 4) / 3) * 3 + 1, 1);
				case "UT-":
					return new DateTime(Qyear, ((now.Month - 1) / 3) * 3 + 1, 1).AddDays(-1);
				case "1T+":
					return DateTime.Now.Month >= 10 ? new DateTime(Qyear + 1, 1, 1) : new DateTime(Qyear, ((now.Month + 2) / 3) * 3 + 1, 1);
				case "UT+":
					return DateTime.Now.Month >= 10 ? new DateTime(Qyear + 1, 3, 31) : new DateTime(Qyear, ((now.Month + 2) / 3) * 3 + 4, 1).AddDays(-1);
				default:
					return null;
			}
		}

		/// <summary>
		/// Convert byte array to string Base64 image
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="image">image byte array</param>
		/// <returns>Base64 image</returns>
		public static string? ImageBase64(byte[] image)
		{
			string extension = "image/jpg";

			if (image == null || image.Length == 0)
				return null;

			// Resize image
			if (image != null && image.Length > 0)
			{
				//in the case of the image being a svg or gif, doesn´t not resize it otherwise the svg will not work and the gif will be a static image
				//we should think on replace the below "else" by a thumbnail on the database
				byte[] pngSig = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
				byte[] jpgSig = { 0xFF, 0xD8 };
				byte[] gifSig = { 0x47, 0x49, 0x46 };
				var text = Encoding.UTF8.GetString(image);
				if (text.StartsWith("<?xml ") || text.StartsWith("<svg "))
					extension = "image/svg+xml";
				else
				{
					using (var ms = new System.IO.MemoryStream(image))
					{
						using (System.Drawing.Image _Image = System.Drawing.Image.FromStream(ms))
						{
							using (System.Drawing.Image _ResizedImage = new System.Drawing.Bitmap(_Image, new System.Drawing.Size(75, 75)))
							{
								image = (byte[])new System.Drawing.ImageConverter().ConvertTo(_ResizedImage, typeof(byte[]));
							}
						}
					}
				}
				var imageString = Convert.ToBase64String(image);
				var img = string.Format("data:{0};base64,{1}", extension, imageString);
				return img;
			}

			return null;
		}

		public static MemberInfo FindFirstPropetyInfoMember(Expression exp)
		{
			MemberInfo member = null;
			if (exp is MemberExpression)
				member = (exp as MemberExpression).Member;
			else if (exp is UnaryExpression)
				member = FindFirstPropetyInfoMember((exp as UnaryExpression).Operand);
			else if (exp is ConditionalExpression)
			{
				member = FindFirstPropetyInfoMember((exp as ConditionalExpression).IfTrue);
				if (member == null)
					member = FindFirstPropetyInfoMember((exp as ConditionalExpression).IfFalse);
			}
			else if (exp is LambdaExpression)
				member = FindFirstPropetyInfoMember((exp as LambdaExpression).Body);

			return member;
		}

		public static string FormatDateValue(DateAttribute.DateEnum ftype, object value)
		{
			string dataFormat = string.Empty;

			switch (ftype)
			{
				case DateAttribute.DateEnum.Date:
					dataFormat = Configuration.DateFormat.Date;
					break;
				case DateAttribute.DateEnum.DateTime:
					dataFormat = Configuration.DateFormat.DateTime;
					break;
				case DateAttribute.DateEnum.DateTimeSeconds:
					dataFormat = Configuration.DateFormat.DateTimeSeconds;
					break;
				case DateAttribute.DateEnum.Time:
					dataFormat = Configuration.DateFormat.Time;
					break;
				default:
					return "Unrecognized date field type.";
			}

			if (value == null || (value is string && (value.Equals("__:__") || value.Equals(""))))
				return string.Empty;

			if (ftype == DateAttribute.DateEnum.Time)
			{
				if (value is string)
				{
					string[] auxVal = ((string)value).Split(':');
					int h = Convert.ToInt32(auxVal[0]);
					int m = Convert.ToInt32(auxVal[1]);
					return new DateTime(1, 1, 1, h, m, 0).ToString(dataFormat, CultureInfo.InvariantCulture);
				}
				else return value.ToString();
			}
			else
			{
				// DBEdit with field of the Date type. (TableDBEdit<A>.ToString()) - Only in the Show mode.
				if ((value is string || value is DateTime) == false)
					value = value.ToString();

				if (value is string)
				{
					DateTime data;
					if (DateTime.TryParse((string)value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out data))
						return data.ToString(dataFormat, CultureInfo.InvariantCulture);
					else return string.Empty;
				}
				else
					return ((DateTime)value).ToString(dataFormat, CultureInfo.InvariantCulture);
			}
		}

		public static string FormatDate<TModel, TValue>(Expression<Func<TModel, TValue>> expression, TModel model)
		{
			var member = FindFirstPropetyInfoMember(expression.Body);

			bool hasDateAttribute = member == null ? false : Attribute.IsDefined(member, typeof(DateAttribute));

			DateAttribute.DateEnum ftype = DateAttribute.DateEnum.Undefined;

			if (hasDateAttribute)
			{
				var attr = Attribute.GetCustomAttribute(member, typeof(DateAttribute));
				ftype = DateAttribute.ConvertToDateAttribute(attr);
			}

			object value = expression.Compile().Invoke(model);

			if ((value as DateTime?) == null || (value as DateTime?) == DateTime.MinValue)
				return "";

			return FormatDateValue(ftype, value);
		}
	}

	public static class Helpers
	{
		/// <summary>
		/// Obtains the languaged matched text according to the resources
		/// </summary>
		/// <param name="str">The id from the resource</param>
		/// <returns>The language aware text</returns>
		public static string GetTextFromResources(string str)
		{
			if (string.IsNullOrEmpty(str))
				return "";
			return Resources.Resources.ResourceManager.GetString(str);
		}

		public static IList<SelectListItem> ToSelectList<T>(this IEnumerable<T> itemsToMap, Expression<Func<T, object>> textProperty, Func<T, object> valueProperty, Predicate<T> isSelected)
		{
			var result = new List<SelectListItem>();

			foreach (var item in itemsToMap)
			{
				object prop_value = textProperty.Compile()(item);
				var propertyName = string.Empty;

				propertyName = HtmlHelpers.FindFirstPropetyInfoMember(textProperty).Name;

				if (!string.IsNullOrEmpty(propertyName))
				{
					Type modelType = item.GetType();
					PropertyInfo fieldProperty = modelType.GetProperty(propertyName);

					if (fieldProperty.PropertyType == typeof(DateTime?))
						prop_value = HtmlHelpers.FormatDate(textProperty, item);
				}

				result.Add(new SelectListItem
				{
					Value = Convert.ToString(valueProperty(item)),
					Text = Convert.ToString(prop_value),
					Selected = isSelected(item)
				});
			}
			return result;
		}

		/// <summary>
		/// Get the client ip address from web server variables
		/// </summary>
		/// <param name="serverVariables">HttpRequest.ServerVariables</param>
		/// <returns></returns>
		private static string GetClientIpAddress(NameValueCollection serverVariables)
		{
			//This is better than Request.UserHostAddress() because of proxy and stuff
			string ip = serverVariables["HTTP_X_FORWARDED_FOR"];
			if (!string.IsNullOrEmpty(ip))
			{
				string[] addresses = ip.Split(',');
				if (addresses.Length != 0)
					return addresses[0];
			}

			return serverVariables["REMOTE_ADDR"];
		}

		/// <summary>
		/// Generates a ticket that can be used by the client-side to access the specified resource.
		/// </summary>
		/// <param name="user">The user for whom this ticket is created.</param>
		/// <param name="table">The table where the resource is located.</param>
		/// <param name="fieldName">The name of the field in the table that contains the resource.</param>
		/// <param name="primaryKeyField">The primary key field name of the table that contains resource.</param>
		/// <param name="keyValue">The primary key value of the record associated with the resource.</param>
		/// <param name="resourceName">Optional. The name of the resource.</param>
		/// <returns>A ticket that provide access to the specified resource in the specified table field.</returns>
		public static string GetFileTicket(User user, Quidgest.Persistence.AreaRef table, string fieldName, string primaryKeyField, string keyValue, string resourceName = null)
		{
			ResourceQuery versionResource = new(resourceName, table.Alias, fieldName, primaryKeyField, keyValue);
			return QResources.CreateTicketEncryptedBase64(user.Name, user.Location, versionResource);
		}
	}
}
