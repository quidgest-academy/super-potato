using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenioMVC;

/// <summary>
/// The «NameValueCollection» requires its own serializer not to be serialized as if it were an array.
/// </summary>
public class NameValueCollectionJsonConverter : JsonConverter<NameValueCollection>
{
	public override NameValueCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var dic = JsonSerializer.Deserialize<Dictionary<string, string[]>>(ref reader, options);
		var res = new NameValueCollection();
		if (dic != null)
			res.AddRange(dic);
		return res;
	}

	public override void Write(Utf8JsonWriter writer, NameValueCollection value, JsonSerializerOptions options)
	{
		var items = value.AllKeys.ToDictionary(k => k!, k => value.GetValues(k)!);
		JsonSerializer.Serialize(writer, items, options);
	}
}
