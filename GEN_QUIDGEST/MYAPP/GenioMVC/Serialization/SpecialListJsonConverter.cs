using System.Text.Json;
using System.Text.Json.Serialization;

using GenioMVC.Helpers.Cav;

namespace GenioMVC;

/// <summary>
/// This object was being serialized as a list because it inherited from a generic List.
/// This converter ensures the items and type properties are correctly serialized.
/// Deserialization of this object is implemented but seems to never be called.
/// </summary>
public class SpecialListJsonConverter : JsonConverter<SpecialList>
{
	public override SpecialList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
			throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");

		SpecialList res = new SpecialList();
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
				return res;

			if (reader.TokenType != JsonTokenType.PropertyName)
				throw new JsonException("JsonTokenType was not PropertyName");

			var propertyName = reader.GetString();

			if (propertyName == "items")
			{
				var items = JsonSerializer.Deserialize<List<string>>(ref reader, options);
				res.AddRange(items);
			}

			if (propertyName == "type")
			{
				var itp = reader.GetInt32();
				res.Type = (LineType)itp;
			}
		}

		return res;
	}

	public override void Write(Utf8JsonWriter writer, SpecialList value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WriteNumber("type", (int)value.Type);
		writer.WritePropertyName("items");
		JsonSerializer.Serialize(writer, (IList<string>)value, options);
		writer.WriteEndObject();
	}
}
