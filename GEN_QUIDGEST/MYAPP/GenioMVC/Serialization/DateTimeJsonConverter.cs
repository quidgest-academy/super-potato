using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenioMVC;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var str = reader.GetString();
		if (string.IsNullOrEmpty(str))
			return DateTime.MinValue;

		if (!reader.TryGetDateTime(out DateTime value))
			value = DateTime.Parse(reader.GetString()!, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
		return value;
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		if (value == DateTime.MinValue)
			writer.WriteStringValue("");
		else
			writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFF"));
	}
}
