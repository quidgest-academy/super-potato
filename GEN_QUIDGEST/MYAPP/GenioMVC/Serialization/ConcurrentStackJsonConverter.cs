using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GenioMVC;

/// <summary>
/// Stack serialization with inverted order - because the stack is a LIFO (last in-first out) collection
/// It is needed to serialize and deserialize the list of objects in the same order.
/// </summary>
public class ConcurrentStackJsonConverter<T> : JsonConverter<ConcurrentStack<T>>
{
	public override ConcurrentStack<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var lst = JsonSerializer.Deserialize<List<T>>(ref reader, options);
		if (lst != null)
			return new ConcurrentStack<T>(lst);
		else
			return null;
	}

	public override void Write(Utf8JsonWriter writer, ConcurrentStack<T> value, JsonSerializerOptions options)
	{
		var _value = value?.Reverse().ToArray() ?? Enumerable.Empty<T>();
		JsonSerializer.Serialize(writer, _value, options);
	}
}
