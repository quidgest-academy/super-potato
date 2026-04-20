using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenioMVC;

/// <summary>
/// VueJs is sending variant data (bool, string, numeric) values and expecting them to be received as string
/// This converter provides that backwards compatibility for dictionaries received in that way.
/// The correct implementation would be to use actual object classes instead of a variant dictionary.
/// </summary>
public class VariantToObjectDictionaryConverter : JsonConverter<IDictionary<string, object>>
{
	public override bool CanConvert(Type typeToConvert)
	{
		return typeToConvert.IsAssignableTo(typeof(IDictionary<string, object>));
	}

	public override IDictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");

		var dictionary = Activator.CreateInstance(typeToConvert) as IDictionary<string, object>;
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

			dictionary.Add(propertyName!, ExtractValue(ref reader, options));
		}

		return dictionary;
	}

	public override void Write(Utf8JsonWriter writer, IDictionary<string, object> value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value, options);
	}

	private object? ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.String:
				if (reader.TryGetDateTime(out var date))
					return date;
				return reader.GetString();
			case JsonTokenType.False:
				return false;
			case JsonTokenType.True:
				return true;
			case JsonTokenType.Null:
				return null;
			case JsonTokenType.Number:
				if (reader.TryGetInt64(out var result))
					return result;
				return reader.GetDecimal();
			case JsonTokenType.StartObject:
				//This should not be supported, but some places in VueJs are still sending object properties
				return System.Text.Json.Nodes.JsonNode.Parse(ref reader).ToString();
			case JsonTokenType.StartArray:
				var list = new List<object?>();
				while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
					list.Add(ExtractValue(ref reader, options));
				return list;
			default:
				throw new JsonException($"'{reader.TokenType}' is not supported");
		}
	}
}
