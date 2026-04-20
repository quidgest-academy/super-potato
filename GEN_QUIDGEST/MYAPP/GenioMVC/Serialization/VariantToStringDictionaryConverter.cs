using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenioMVC;

/// <summary>
/// VueJs is sending variant data (bool, string, numeric) values and expecting them to be received as string
/// This converter provides that backwards compatibility for dictionaries received in that way.
/// The correct implementation would be to use actual object classes instead of a variant dictionary.
/// </summary>
public class VariantToStringDictionaryConverter : JsonConverter<Dictionary<string, string>>
{
	public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");

		var dictionary = new Dictionary<string, string>();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				return dictionary;

			if (reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException("JsonTokenType was not PropertyName");

			var propertyName = reader.GetString();

			if (string.IsNullOrWhiteSpace(propertyName))
				throw new JsonException("Failed to get property name");

			reader.Read();

			dictionary.Add(propertyName!, ExtractValue(ref reader));
		}

		return dictionary;
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, (IDictionary<string, string>)value, options);
	}

	private string ExtractValue(ref Utf8JsonReader reader)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.String:
				return reader.GetString() ?? "";
			case JsonTokenType.False:
				return "false";
			case JsonTokenType.True:
				return "true";
			case JsonTokenType.Null:
				return "";
			case JsonTokenType.Number:
				if (reader.TryGetInt64(out var result))
					return result.ToString(System.Globalization.CultureInfo.InvariantCulture);
				else
					return reader.GetDecimal().ToString(System.Globalization.CultureInfo.InvariantCulture);
			case JsonTokenType.StartObject:
				//This should not be supported, but some places in VueJs are still sending object properties
				return System.Text.Json.Nodes.JsonNode.Parse(ref reader).ToString();
			default:
				throw new JsonException($"'{reader.TokenType}' is not supported");
		}
	}
}
