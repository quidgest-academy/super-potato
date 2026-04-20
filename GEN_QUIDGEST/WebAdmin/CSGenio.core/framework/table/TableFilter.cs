using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSGenio.core.framework.table;

/// <summary>
/// Defines the types of table filters available in the system.
/// </summary>
public enum FilterType
{
	/// <summary>
	/// Group filter that applies predefined filtering criteria based on key-value pairs.
	/// </summary>
	GROUP,

	/// <summary>
	/// Active filter that filters records based on their temporal status.
	/// </summary>
	ACTIVE,

	/// <summary>
	/// Column filter that applies filtering criteria to specific table fields.
	/// </summary>
	COLUMN
}

/// <summary>
/// Defines the binary logical operators for combining filters.
/// </summary>
public enum FilterBinaryOperator
{
	/// <summary>
	/// AND operator - all conditions must be true.
	/// </summary>
	AND,

	/// <summary>
	/// OR operator - at least one condition must be true.
	/// </summary>
	OR
}

/// <summary>
/// Defines the types of values that can be used in filters.
/// </summary>
public enum GlobalFilterType
{
	/// <summary>
	/// String value type.
	/// </summary>
	STRING,

	/// <summary>
	/// Boolean value type.
	/// </summary>
	BOOLEAN,

	/// <summary>
	/// Numeric value type (integer or decimal).
	/// </summary>
	NUMERIC,

	/// <summary>
	/// List of string values.
	/// </summary>
	STRING_LIST,

	/// <summary>
	/// List of boolean values.
	/// </summary>
	BOOLEAN_LIST,

	/// <summary>
	/// List of numeric values.
	/// </summary>
	NUMERIC_LIST
}

#region Converters

/// <summary>
/// Custom JSON converter for FilterBinaryOperator that serializes to boolean.
/// OR is serialized as true, AND is serialized as false.
/// </summary>
internal class FilterBinaryOperatorConverter : JsonConverter<FilterBinaryOperator>
{
	/// <summary>
	/// Reads a JSON boolean value and converts it to a FilterBinaryOperator enum value.
	/// </summary>
	/// <param name="reader">The JSON reader.</param>
	/// <param name="typeToConvert">The type being converted (should be <see cref="FilterBinaryOperator"/>).</param>
	/// <param name="options">The JSON serializer options.</param>
	/// <returns>FilterBinaryOperator.OR if the boolean is true; FilterBinaryOperator.AND if false.</returns>
	/// <exception cref="JsonException">Thrown if the JSON token is not a boolean.</exception>
	public override FilterBinaryOperator Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
			throw new JsonException($"Unexpected token {reader.TokenType} when parsing FilterBinaryOperator. Expected boolean.");

		return reader.GetBoolean() ? FilterBinaryOperator.OR : FilterBinaryOperator.AND;
	}

	/// <summary>
	/// Writes a FilterBinaryOperator enum value as a JSON boolean.
	/// </summary>
	/// <param name="writer">The JSON writer.</param>
	/// <param name="value">The FilterBinaryOperator value to serialize.</param>
	/// <param name="options">The JSON serializer options.</param>
	public override void Write(Utf8JsonWriter writer, FilterBinaryOperator value, JsonSerializerOptions options)
	{
		writer.WriteBooleanValue(value == FilterBinaryOperator.OR);
	}
}

/// <summary>
/// Custom JSON converter that deserializes <see cref="List{Object}"/> while preserving inferred types.
/// </summary>
/// <remarks>
/// This converter handles deserialization of heterogeneous lists containing various types including:
/// - Null values
/// - Booleans
/// - Numbers (integers and decimals)
/// - Strings
/// - Nested arrays
/// - Objects (deserialized as dictionaries)
///
/// Without this converter, System.Text.Json would deserialize all values as <see cref="JsonElement"/>,
/// losing type information and causing casting exceptions.
/// </remarks>
internal class ObjectToInferredTypesConverter : JsonConverter<List<object>>
{
	/// <summary>
	/// Reads a JSON array and deserializes it to a list of objects with inferred types.
	/// </summary>
	/// <param name="reader">The JSON reader.</param>
	/// <param name="typeToConvert">The type being converted (should be <see cref="List{Object}"/>).</param>
	/// <param name="options">The JSON serializer options.</param>
	/// <returns>A list of objects with their types inferred from the JSON tokens.</returns>
	/// <exception cref="JsonException">Thrown if the JSON does not start with an array token or contains unexpected tokens.</exception>
	public override List<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
			throw new JsonException("Expected array");
		return ReadArrayElements(ref reader, options);
	}

	/// <summary>
	/// Reads JSON array elements and deserializes them to a list of objects with inferred types.
	/// </summary>
	/// <param name="reader">The JSON reader positioned at or after an array start token.</param>
	/// <param name="options">The JSON serializer options.</param>
	/// <returns>A list of objects representing the array contents with inferred types.</returns>
	/// <exception cref="JsonException">Thrown if the array contains unexpected tokens.</exception>
	private List<object> ReadArrayElements(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		List<object> list = [];
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
				break;

			object value = DeserializeValue(ref reader, options);
			list.Add(value);
		}
		return list;
	}

	/// <summary>
	/// Deserializes a single JSON token to an object with inferred type.
	/// </summary>
	/// <param name="reader">The JSON reader positioned at the token to deserialize.</param>
	/// <param name="options">The JSON serializer options.</param>
	/// <returns>An object with its type inferred from the JSON token.</returns>
	/// <exception cref="JsonException">Thrown if the token type is unexpected.</exception>
	private object DeserializeValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		return reader.TokenType switch
		{
			JsonTokenType.Null => null,
			JsonTokenType.True => true,
			JsonTokenType.False => false,
			JsonTokenType.Number => reader.TryGetInt64(out long l) ? (object)l : reader.GetDouble(),
			JsonTokenType.String => reader.GetString(),
			JsonTokenType.StartArray => ReadArrayElements(ref reader, options),
			JsonTokenType.StartObject => JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options),
			_ => throw new JsonException($"Unexpected token: {reader.TokenType}")
		};
	}

	/// <summary>
	/// Writes a list of objects to JSON.
	/// </summary>
	/// <param name="writer">The JSON writer.</param>
	/// <param name="value">The list of objects to serialize.</param>
	/// <param name="options">The JSON serializer options.</param>
	public override void Write(Utf8JsonWriter writer, List<object> value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value, options);
	}
}

#endregion

/// <summary>
/// Defines the contract for polymorphic filter values that can be strings, booleans or numbers.
/// </summary>
/// <remarks>
/// Filter values use polymorphic serialization with a "type" discriminator to preserve type information
/// during JSON deserialization. This allows heterogeneous collections of values to maintain their original types
/// (string, boolean, number, etc.) rather than being deserialized as generic JsonElement objects.
/// </remarks>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StringGlobalFilter), nameof(GlobalFilterType.STRING))]
[JsonDerivedType(typeof(BooleanGlobalFilter), nameof(GlobalFilterType.BOOLEAN))]
[JsonDerivedType(typeof(NumericGlobalFilter), nameof(GlobalFilterType.NUMERIC))]
[JsonDerivedType(typeof(StringListGlobalFilter), nameof(GlobalFilterType.STRING_LIST))]
[JsonDerivedType(typeof(BooleanListGlobalFilter), nameof(GlobalFilterType.BOOLEAN_LIST))]
[JsonDerivedType(typeof(NumericListGlobalFilter), nameof(GlobalFilterType.NUMERIC_LIST))]
public interface IGlobalFilter
{
	/// <summary>
	/// Gets the type identifier for this filter value.
	/// </summary>
	/// <value>A GlobalFilterType enum value identifying the value's data type.</value>
	GlobalFilterType Type { get; }

	/// <summary>
	/// Gets the underlying value as an object.
	/// </summary>
	/// <value>The filter value that can be cast to the appropriate type based on the Type property.</value>
	object Value { get; }

	/// <summary>
	/// Gets or sets the list of possible array values for this filter.
	/// </summary>
	/// <remarks>
	/// This property contains the collection of possible values that can be used when the filter
	/// is applied to an array/enumeration field. The list type matches the filter's value type:
	/// - List&lt;string&gt; for StringGlobalFilter
	/// - List&lt;bool&gt; for BooleanGlobalFilter
	/// - List&lt;decimal&gt; for NumericGlobalFilter
	/// </remarks>
	/// <value>A list of possible values; an empty list if no array values are specified.</value>
	object Array { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the filter is applied only to the current table.
	/// </summary>
	/// <remarks>
	/// When true, the filter is unique to a single table and can be saved along with the table configuration.
	/// When false, the filter is shared across multiple tables and cannot be safely saved in the configuration,
	/// as it could create incoherences in filtered results across tables.
	/// </remarks>
	/// <value>True if the filter is unique to the current table; otherwise, false.</value>
	bool IsUnique { get; set; }

	/// <summary>
	/// Checks if a given value exists in the Array property.
	/// </summary>
	/// <param name="value">The value to check for in the Array property.</param>
	/// <returns>True if the value exists in the Array property; otherwise, false.</returns>
	bool IsArrayOption(object value);
}

/// <summary>
/// Abstract base class for filter values that provides common functionality for all filter value types.
/// </summary>
/// <param name="type">The GlobalFilterType enum value for this filter value implementation.</param>
/// <remarks>
/// This base class handles the common properties and JSON serialization for all filter value types,
/// while derived classes implement specific value storage and the Value property.
/// </remarks>
public abstract class GlobalFilter(GlobalFilterType type) : IGlobalFilter
{
	/// <inheritdoc/>
	[JsonIgnore]
	public GlobalFilterType Type { get; } = type;

	/// <inheritdoc/>
	public abstract object Value { get; }

	/// <inheritdoc/>
	public abstract object Array { get; set; }

	/// <inheritdoc/>
	[JsonPropertyName("isUnique")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public bool IsUnique { get; set; } = true;

	/// <inheritdoc/>
	public virtual bool IsArrayOption(object value)
	{
		if (Array is not IEnumerable enumerable)
			return false;

		foreach (object item in enumerable)
			if (Equals(item, value))
				return true;

		return false;
	}
}

/// <summary>
/// Represents a string-typed filter value.
/// </summary>
public class StringGlobalFilter() : GlobalFilter(GlobalFilterType.STRING)
{
	/// <summary>
	/// Gets or sets the string value.
	/// </summary>
	/// <value>The string data to use in filter comparisons.</value>
	[JsonPropertyName("value")]
	public string StringValue { get; set; }

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Value => StringValue;

	/// <summary>
	/// Gets or sets the list of possible string array values.
	/// </summary>
	/// <value>A collection of string values that can be used for array filtering.</value>
	[JsonPropertyName("array")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public List<string> StringArray { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Array
	{
		get => StringArray;
		set => StringArray = value as List<string> ?? [];
	}
}

/// <summary>
/// Represents a boolean-typed filter value.
/// </summary>
public class BooleanGlobalFilter() : GlobalFilter(GlobalFilterType.BOOLEAN)
{
	/// <summary>
	/// Gets or sets the boolean value.
	/// </summary>
	/// <value>The boolean data to use in filter comparisons.</value>
	[JsonPropertyName("value")]
	public bool BooleanValue { get; set; }

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Value => BooleanValue;

	/// <summary>
	/// Gets or sets the list of possible boolean array values.
	/// </summary>
	/// <value>A collection of boolean values that can be used for array filtering.</value>
	[JsonPropertyName("array")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public List<bool> BooleanArray { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Array
	{
		get => BooleanArray;
		set => BooleanArray = value as List<bool> ?? [];
	}
}

/// <summary>
/// Represents a numeric-typed filter value (integer or decimal).
/// </summary>
public class NumericGlobalFilter() : GlobalFilter(GlobalFilterType.NUMERIC)
{
	/// <summary>
	/// Gets or sets the numeric value.
	/// </summary>
	/// <value>The numeric data (as decimal to support both integers and decimals) to use in filter comparisons.</value>
	[JsonPropertyName("value")]
	public decimal NumericValue { get; set; }

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Value => NumericValue;

	/// <summary>
	/// Gets or sets the list of possible numeric array values.
	/// </summary>
	/// <value>A collection of decimal values that can be used for array filtering.</value>
	[JsonPropertyName("array")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public List<decimal> NumericArray { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Array
	{
		get => NumericArray;
		set => NumericArray = value as List<decimal> ?? [];
	}
}

/// <summary>
/// Represents a list of string values filter value.
/// </summary>
public class StringListGlobalFilter() : GlobalFilter(GlobalFilterType.STRING_LIST)
{
	/// <summary>
	/// Gets or sets the list of string values.
	/// </summary>
	/// <value>A collection of string values to use in filter comparisons.</value>
	[JsonPropertyName("value")]
	public List<string> StringListValue { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Value => StringListValue;

	/// <summary>
	/// Gets or sets the list of possible string array values.
	/// </summary>
	/// <value>A collection of string values that can be used for array filtering.</value>
	[JsonPropertyName("array")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public List<string> StringArray { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Array
	{
		get => StringArray;
		set => StringArray = value as List<string> ?? [];
	}
}

/// <summary>
/// Represents a list of boolean values filter value.
/// </summary>
public class BooleanListGlobalFilter() : GlobalFilter(GlobalFilterType.BOOLEAN_LIST)
{
	/// <summary>
	/// Gets or sets the list of boolean values.
	/// </summary>
	/// <value>A collection of boolean values to use in filter comparisons.</value>
	[JsonPropertyName("value")]
	public List<bool> BooleanListValue { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Value => BooleanListValue;

	/// <summary>
	/// Gets or sets the list of possible boolean array values.
	/// </summary>
	/// <value>A collection of boolean values that can be used for array filtering.</value>
	[JsonPropertyName("array")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public List<bool> BooleanArray { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Array
	{
		get => BooleanArray;
		set => BooleanArray = value as List<bool> ?? [];
	}
}

/// <summary>
/// Represents a list of numeric values filter value.
/// </summary>
public class NumericListGlobalFilter() : GlobalFilter(GlobalFilterType.NUMERIC_LIST)
{
	/// <summary>
	/// Gets or sets the list of numeric values.
	/// </summary>
	/// <value>A collection of numeric values (as decimal to support both integers and decimals) to use in filter comparisons.</value>
	[JsonPropertyName("value")]
	public List<decimal> NumericListValue { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Value => NumericListValue;

	/// <summary>
	/// Gets or sets the list of possible numeric array values.
	/// </summary>
	/// <value>A collection of decimal values that can be used for array filtering.</value>
	[JsonPropertyName("array")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public List<decimal> NumericArray { get; set; } = [];

	/// <inheritdoc/>
	[JsonIgnore]
	public override object Array
	{
		get => NumericArray;
		set => NumericArray = value as List<decimal> ?? [];
	}
}

/// <summary>
/// Defines the contract for table filters that can be applied to table data.
/// Supports hierarchical filtering with sub-filters and logical operators.
/// </summary>
/// <remarks>
/// Table filters provide a flexible system for filtering table data with support for:
/// - Different filter types (group, active, column-based)
/// - Hierarchical sub-filters for complex filtering logic
/// - Logical operators (AND/OR) for combining filters
/// - Active/inactive state management
/// </remarks>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(GroupFilter), nameof(FilterType.GROUP))]
[JsonDerivedType(typeof(ActiveFilter), nameof(FilterType.ACTIVE))]
[JsonDerivedType(typeof(ColumnFilter), nameof(FilterType.COLUMN))]
public interface ITableFilter
{
	/// <summary>
	/// Gets the type identifier for this filter.
	/// </summary>
	/// <value>A FilterType enum value identifying the filter type.</value>
	FilterType Type { get; }

	/// <summary>
	/// Gets or sets the display name for this filter.
	/// </summary>
	/// <value>The human-readable name of the filter, used for display purposes.</value>
	string Name { get; set; }

	/// <summary>
	/// Gets or sets the binary logical operator used when combining this filter with other filters.
	/// </summary>
	/// <value>AND to use AND logic (default); OR to use OR logic.</value>
	FilterBinaryOperator BinaryOperator { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this filter is currently active.
	/// </summary>
	/// <value>True if the filter should be applied to table data; false if it should be ignored.</value>
	bool IsActive { get; set; }

	/// <summary>
	/// Gets or sets the collection of sub-filters that belong to this filter.
	/// </summary>
	/// <value>A list of child filters that can be combined with this filter using logical operators.</value>
	List<ITableFilter> SubFilters { get; set; }

	/// <summary>
	/// Creates a deep copy of this filter instance.
	/// </summary>
	/// <returns>A new filter instance that is a complete copy of this filter, including all sub-filters.</returns>
	ITableFilter Clone();
}

/// <summary>
/// Abstract base class for table filters that provides common functionality for all filter types.
/// </summary>
/// <param name="type">The FilterType enum value for this filter implementation.</param>
/// <remarks>
/// This base class handles the common properties and JSON serialization for all filter types,
/// while derived classes implement specific filtering logic and the Clone method.
/// </remarks>
public abstract class TableFilter(FilterType type) : ITableFilter
{
	/// <inheritdoc/>
	[JsonIgnore]
	public FilterType Type { get; } = type;

	/// <inheritdoc/>
	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	/// <inheritdoc/>
	[JsonPropertyName("useOr")]
	[JsonConverter(typeof(FilterBinaryOperatorConverter))]
	public FilterBinaryOperator BinaryOperator { get; set; } = FilterBinaryOperator.AND;

	/// <inheritdoc/>
	[JsonPropertyName("active")]
	public bool IsActive { get; set; } = true;

	/// <inheritdoc/>
	[JsonPropertyName("subFilters")]
	public List<ITableFilter> SubFilters { get; set; } = [];

	/// <inheritdoc/>
	public abstract ITableFilter Clone();
}

/// <summary>
/// Represents a group filter that applies predefined filtering criteria based on key-value pairs.
/// </summary>
/// <remarks>
/// Group filters are typically used for system-defined filtering options that don't change
/// based on user input. They use a key-value structure where the key identifies the filter
/// type and the value contains the filtering criteria.
/// </remarks>
public class GroupFilter() : TableFilter(FilterType.GROUP)
{
	/// <summary>
	/// Gets or sets the key that identifies the type of group filter.
	/// </summary>
	/// <value>The filter key used to identify which group filter logic to apply.</value>
	[JsonPropertyName("key")]
	public string Key { get; set; }

	/// <summary>
	/// Gets or sets the value that defines the filtering criteria for this group filter.
	/// </summary>
	/// <value>The filter value that specifies what data should be included or excluded.</value>
	[JsonPropertyName("value")]
	public string Value { get; set; } = "";

	/// <summary>
	/// Creates a deep copy of this group filter instance.
	/// </summary>
	/// <returns>A new GroupFilter instance with all properties copied, including sub-filters.</returns>
	public override ITableFilter Clone()
	{
		return new GroupFilter()
		{
			Name = Name,
			BinaryOperator = BinaryOperator,
			IsActive = IsActive,
			SubFilters = [.. SubFilters.Select(f => f.Clone())],
			Key = Key,
			Value = Value
		};
	}
}

/// <summary>
/// Represents an active filter that filters records based on their temporal status (current, previous, upcoming).
/// </summary>
/// <remarks>
/// Active filters are commonly used for time-sensitive data where records can be categorized
/// as current (active now), previous (expired/inactive), or upcoming (future-dated).
/// The Date property provides the reference point for these temporal comparisons.
/// </remarks>
public class ActiveFilter() : TableFilter(FilterType.ACTIVE)
{
	/// <summary>
	/// Gets or sets the reference date used for determining temporal status.
	/// </summary>
	/// <value>The date string used as the baseline for current/previous/upcoming comparisons.</value>
	[JsonPropertyName("date")]
	public string Date { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether current/active records should be included.
	/// </summary>
	/// <value>True to include records that are currently active; otherwise, false.</value>
	[JsonPropertyName("current")]
	public bool Current { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether previous/expired records should be included.
	/// </summary>
	/// <value>True to include records that are no longer active; otherwise, false.</value>
	[JsonPropertyName("previous")]
	public bool Previous { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether upcoming/future records should be included.
	/// </summary>
	/// <value>True to include records that will become active in the future; otherwise, false.</value>
	[JsonPropertyName("upcoming")]
	public bool Upcoming { get; set; }

	/// <summary>
	/// Creates a deep copy of this active filter instance.
	/// </summary>
	/// <returns>A new ActiveFilter instance with all properties copied, including sub-filters.</returns>
	public override ITableFilter Clone()
	{
		return new ActiveFilter()
		{
			Name = Name,
			BinaryOperator = BinaryOperator,
			IsActive = IsActive,
			SubFilters = [.. SubFilters.Select(f => f.Clone())],
			Date = Date,
			Current = Current,
			Previous = Previous,
			Upcoming = Upcoming
		};
	}
}

/// <summary>
/// Represents a column-based filter that applies filtering criteria to specific table fields.
/// </summary>
/// <remarks>
/// Column filters provide flexible filtering capabilities for table columns using:
/// - Field specification (or empty for all columns)
/// - Comparison operators (equals, contains, greater than, etc.)
/// - Multiple values for comparison
/// This is the most common type of user-defined filter for table data.
/// </remarks>
public class ColumnFilter() : TableFilter(FilterType.COLUMN)
{
	/// <summary>
	/// Gets or sets the field name to apply this filter to.
	/// </summary>
	/// <value>
	/// The name of the table column/field to filter. If empty or null,
	/// the filter values should be searched across all columns.
	/// </value>
	[JsonPropertyName("field")]
	public string Field { get; set; }

	/// <summary>
	/// Gets or sets the comparison operator used for filtering.
	/// </summary>
	/// <value>
	/// The operator string that defines how values are compared
	/// (e.g., "equals", "contains", "greater_than", "less_than").
	/// </value>
	[JsonPropertyName("operator")]
	public string Operator { get; set; }

	/// <summary>
	/// Gets or sets the collection of values to compare against when applying this filter.
	/// </summary>
	/// <value>
	/// A list of objects representing the values to use in the comparison operation.
	/// Multiple values typically use OR logic within this single filter.
	/// </value>
	[JsonPropertyName("values")]
	[JsonConverter(typeof(ObjectToInferredTypesConverter))]
	public List<object> Values { get; set; } = [];

	/// <summary>
	/// Creates a deep copy of this column filter instance.
	/// </summary>
	/// <returns>A new ColumnFilter instance with all properties copied, including sub-filters and values.</returns>
	/// <remarks>
	/// The Values collection is deep-cloned using JSON serialization to ensure complete independence
	/// from the original filter instance.
	/// </remarks>
	public override ITableFilter Clone()
	{
		return new ColumnFilter()
		{
			Name = Name,
			BinaryOperator = BinaryOperator,
			IsActive = IsActive,
			SubFilters = [.. SubFilters.Select(f => f.Clone())],
			Field = Field,
			Operator = Operator,
			Values = [.. Values]
		};
	}
}
