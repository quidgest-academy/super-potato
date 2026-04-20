using System.Text.Json.Serialization.Metadata;
using System.Text.Json;

namespace GenioMVC;

/// <summary>
/// Implement this interface in serializable classes where you wish to have conditional serialization.
/// The specific conditional logic should be implemented there.
/// Each conditional property should be marked with ShouldSerializeAttribute that adds a tag to that property.
/// Group tags or make them unique according to your serialization needs.
/// </summary>
public interface IConditionalSerializer
{
	public bool ShouldSerialize(string tag);
}

/// <summary>
/// Tag conditionally serializable properties with this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ShouldSerializeAttribute : Attribute
{
	public ShouldSerializeAttribute(string tag)
	{
		Tag = tag;
	}

	public string Tag { get; set; }
}

/// <summary>
/// Compatibility support for the same functionality offered by ShouldSerialize properties of Newtonsoft.Json
/// Based on:
/// https://gist.github.com/krwq/c61f33faccc708bfa569b3c8aebb45d6
/// </summary>
public class ConditionalJsonInfoResolver : DefaultJsonTypeInfoResolver
{
	public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		//Get the default json type resolution
		JsonTypeInfo ti = base.GetTypeInfo(type, options);

		if (typeof(IConditionalSerializer).IsAssignableFrom(type))
		{
			foreach (var pi in ti.Properties)
			{
				//We look for properties marked with the ShouldSerialize attribute and use the Tag defined in it to setup the callback function
				// provided by the IConditionalSerializer interface.
				var ssa = pi.AttributeProvider?.GetCustomAttributes(typeof(ShouldSerializeAttribute), false).FirstOrDefault() as ShouldSerializeAttribute;
				if (ssa != null)
					pi.ShouldSerialize = (obj, val) => ((IConditionalSerializer)obj).ShouldSerialize(ssa.Tag);
			}
		}

		return ti;
	}
}
