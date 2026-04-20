using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GenioMVC.Helpers.Cav
{
    [JsonConverter(typeof(SpecialListJsonConverter))]
	public class SpecialList : List<string>
	{
		public LineType Type { get; set; }

		public string[] Items
		{
			get
			{
				return this?.ToArray();
			}
		}
	}

}