using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenioMVC;

public class SelectListJsonConverter : JsonConverter<SelectList>
{
	public class KeyValueItem
	{
		public string key { get; set; } = string.Empty;
		public string value { get; set; } = string.Empty;
	}

	public override SelectList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.StartArray)
		{
			var res = new List<KeyValueItem>();
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndArray)
					break;
				var node = JsonSerializer.Deserialize<KeyValueItem>(ref reader, options) ?? throw new JsonException("Invalid object deserializing SelectList");
				res.Add(node);
			}
			return new SelectList(res, "key", "value");
		}
		throw new JsonException("SelectList requires deserialization from jsonArray");
	}

	public override void Write(Utf8JsonWriter writer, SelectList value, JsonSerializerOptions options)
	{
		var items = value.Select(item => new KeyValueItem() { key = item.Value, value = item.Text });
		JsonSerializer.Serialize(writer, items, options);
	}
}
