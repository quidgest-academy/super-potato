using System;
using System.Text.Json.Serialization;

using Quidgest.Persistence;

namespace CSGenio.core.framework.table;

/// <summary>
/// Represents configuration settings for a table column including visibility, order, sorting, and exportability.
/// </summary>
/// <remarks>
/// This class defines how a column should be displayed and behaved in table views, including:
/// - Display order and visibility settings
/// - Sorting configuration (order and direction)
/// - Export capabilities
/// - Field name parsing utilities for table.column format
/// </remarks>
public class ColumnConfiguration
{
	/// <summary>
	/// Gets or sets the name of the column in the format [TABLE].Val[COLUMN] or Val[COLUMN].
	/// </summary>
	/// <value>
	/// The full column identifier. Can be in the format "[TABLE].Val[COLUMN]" for fields from related tables,
	/// or "Val[COLUMN]" for fields from the main table.
	/// </value>
	[JsonPropertyName("name")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the display order of the column in the table.
	/// </summary>
	/// <value>The zero-based position where this column should appear in the table display.</value>
	[JsonPropertyName("order")]
	public int Order { get; set; }

	/// <summary>
	/// Gets or sets the sorting order priority for this column.
	/// </summary>
	/// <value>
	/// The sort priority where 0 means no sorting, 1 means primary sort column,
	/// 2 means secondary sort column, etc. Higher numbers indicate lower priority.
	/// </value>
	[JsonPropertyName("sortOrder")]
	public int SortOrder { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the column should be sorted in ascending order.
	/// </summary>
	/// <value>True for ascending sort order; false for descending sort order.</value>
	[JsonPropertyName("sortAsc")]
	public bool SortAsc { get; set; } = true;

	/// <summary>
	/// Gets or sets the visibility status of the column.
	/// </summary>
	/// <value>1 if the column is visible in the table; 0 if the column is hidden.</value>
	[JsonPropertyName("visibility")]
	public int Visibility { get; set; }

	/// <summary>
	/// Gets or sets the exportability status of the column.
	/// </summary>
	/// <value>1 if the column can be exported; 0 if the column should be excluded from exports.</value>
	[JsonPropertyName("exportability")]
	public int Exportability { get; set; }

	/// <summary>
	/// Extracts the table name from a field name in the format [TABLE].Val[COLUMN] or Val[COLUMN].
	/// </summary>
	/// <param name="mainTableName">The name of the main table to use as fallback when no table prefix is found.</param>
	/// <param name="name">The full field name in the format "[TABLE].Val[COLUMN]" or "Val[COLUMN]".</param>
	/// <returns>
	/// The table name in lowercase. Returns the main table name if no table prefix is found.
	/// Returns null if either input parameter is null or empty.
	/// </returns>
	/// <remarks>
	/// This method parses field names to determine which table the field belongs to.
	/// If the field name doesn't contain a table prefix (no dot separator), it assumes
	/// the field belongs to the main table.
	/// </remarks>
	/// <example>
	/// <code>
	/// string tableName = ColumnConfiguration.GetTableName("users", "profiles.ValName");
	/// // Returns: "profiles"
	///
	/// string tableName2 = ColumnConfiguration.GetTableName("users", "ValEmail");
	/// // Returns: "users"
	/// </code>
	/// </example>
	public static string GetTableName(string mainTableName, string name)
	{
		if (string.IsNullOrEmpty(mainTableName) || string.IsNullOrEmpty(name))
			return null;

		int sepIndex = name.IndexOf('.');

		// If the table name is empty, the field is in the main table
		if (sepIndex == -1)
			return mainTableName;

		// Table name is everything before '.'
		return name.Substring(0, sepIndex).ToLowerInvariant();
	}

	/// <summary>
	/// Extracts the column name from a field name in the format [TABLE].Val[COLUMN] or Val[COLUMN].
	/// </summary>
	/// <param name="name">The full field name in the format "[TABLE].Val[COLUMN]" or "Val[COLUMN]".</param>
	/// <returns>
	/// The column name in lowercase, extracted from after "Val" in the field name.
	/// Returns null if the name parameter is null, empty, or doesn't contain the "Val" prefix.
	/// </returns>
	/// <remarks>
	/// This method parses field names to extract the actual column identifier.
	/// It handles both full qualified names with table prefixes and simple field names.
	/// The method looks for either ".Val" or "Val" patterns and extracts everything after them.
	/// </remarks>
	/// <example>
	/// <code>
	/// string columnName = ColumnConfiguration.GetColumnName("profiles.ValName");
	/// // Returns: "name"
	///
	/// string columnName2 = ColumnConfiguration.GetColumnName("ValEmail");
	/// // Returns: "email"
	/// </code>
	/// </example>
	public static string GetColumnName(string name)
	{
		if (string.IsNullOrEmpty(name))
			return null;

		int sepIndex = name.IndexOf(".Val");
		int sepLenth = 4;

		// If the name does not include the table name
		if (sepIndex == -1)
		{
			sepIndex = name.IndexOf("Val");
			sepLenth = 3;
		}

		// If the name is invalid
		if (sepIndex == -1)
			return null;

		// Column name is everything after ".Val" or "Val"
		return name.Substring(sepIndex + sepLenth).ToLowerInvariant();
	}

	/// <summary>
	/// Creates a FieldRef object from a field name and main table name.
	/// </summary>
	/// <param name="mainTableName">The name of the main table to use as fallback when no table prefix is found.</param>
	/// <param name="name">The full field name in the format "[TABLE].Val[COLUMN]" or "Val[COLUMN]".</param>
	/// <returns>
	/// A FieldRef object representing the parsed field with its table and column components.
	/// Returns null if either input parameter is null or empty, or if the field name format is invalid.
	/// </returns>
	/// <remarks>
	/// This method combines the functionality of GetTableName and GetColumnName to create
	/// a complete field reference object. The FieldRef can then be used for database queries
	/// and other operations that require structured field information.
	/// </remarks>
	/// <example>
	/// <code>
	/// FieldRef fieldRef = ColumnConfiguration.GetFieldRef("users", "profiles.ValName");
	/// // Returns: FieldRef with Table="profiles" and Column="name"
	///
	/// FieldRef fieldRef2 = ColumnConfiguration.GetFieldRef("users", "ValEmail");
	/// // Returns: FieldRef with Table="users" and Column="email"
	/// </code>
	/// </example>
	/// <exception cref="ArgumentException">
	/// May be thrown by the FieldRef constructor if the parsed table or column names are invalid.
	/// </exception>
	public static FieldRef GetFieldRef(string mainTableName, string name)
	{
		return string.IsNullOrEmpty(mainTableName) || string.IsNullOrEmpty(name)
			? null
			: new FieldRef(GetTableName(mainTableName, name), GetColumnName(name));
	}
}
