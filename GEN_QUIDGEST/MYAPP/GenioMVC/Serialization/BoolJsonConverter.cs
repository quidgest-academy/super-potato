using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenioMVC;

public class BoolJsonConverter : JsonConverter<bool>
{
	public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
			return reader.GetBoolean();
		else if (reader.TokenType == JsonTokenType.Number)
			return reader.GetInt32() == 1;
		else if (reader.TokenType == JsonTokenType.String)
			return reader.GetString() == "1";
		else return false;
	}

	public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
	{
		writer.WriteBooleanValue(value);
	}
}

