using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSGenio.business;
using CSGenio.core.di;
using CSGenio.framework;
using CSGenio.persistence;

namespace CSGenio.core.framework.table.legacy.v2;

/// <summary>
/// JSON converter that handles conversion between arrays and string arrays,
/// ensuring all array elements are converted to strings during deserialization.
/// </summary>
/// <remarks>
/// This converter is specifically designed for table configuration serialization
/// where mixed-type arrays need to be normalized to string arrays.
/// </remarks>
public class ToStringArrayConverter : JsonConverter<string[]>
{
	/// <summary>
	/// Reads JSON and converts an array of mixed types to a string array.
	/// </summary>
	/// <param name="reader">The UTF-8 JSON reader to read from.</param>
	/// <param name="type">The type to convert (string[]).</param>
	/// <param name="options">Serializer options to use during conversion.</param>
	/// <returns>A string array with all elements converted to strings.</returns>
	/// <remarks>
	/// If deserialization fails, an empty array is returned and the error is logged.
	/// All array elements are converted to strings using their ToString() method.
	/// </remarks>
	public override string[] Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
	{
		List<string> stringList = [];
		object[] array;

		try
		{
			// Deserialize to an object array
			array = JsonSerializer.Deserialize<object[]>(JsonElement.ParseValue(ref reader));
		}
		catch (Exception e)
		{
			GenioDI.Log.Error($"Table Configuration Update ToStringArrayConverter(Read) - {e.Message}");
			array = [];
		}

		// Convert all values to strings
		foreach (object item in array)
			stringList.Add(item.ToString());

		return [.. stringList];
	}

	/// <summary>
	/// Writes a string array to JSON format.
	/// </summary>
	/// <param name="writer">The UTF-8 JSON writer to write to.</param>
	/// <param name="value">The string array to serialize.</param>
	/// <param name="options">Serializer options to use during conversion.</param>
	/// <remarks>
	/// If serialization fails, an empty JSON array "[]" is written and the error is logged.
	/// The result is written as raw JSON to maintain proper array formatting.
	/// </remarks>
	public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
	{
		string serializedArray;
		try
		{
			// Serialize array
			serializedArray = JsonSerializer.Serialize(value);
		}
		catch (Exception e)
		{
			GenioDI.Log.Error($"Table Configuration Update ToStringArrayConverter(Write) - {e.Message}");
			serializedArray = "[]";
		}

		// Write value as raw so it is an array of strings
		writer.WriteRawValue(serializedArray);
	}
}

/// <summary>
/// Represents a search filter with a name, active state, and multiple conditions.
/// </summary>
/// <remarks>
/// Search filters are used in table configurations to define user-created filter criteria
/// that can be applied to table data.
/// </remarks>
public class SearchFilter
{
	/// <summary>
	/// Gets or sets the name of the search filter.
	/// </summary>
	/// <value>The display name for this filter.</value>
	[JsonPropertyName("name")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this filter is currently active.
	/// </summary>
	/// <value>True if the filter should be applied to table data; otherwise, false.</value>
	[JsonPropertyName("active")]
	public bool Active { get; set; }

	/// <summary>
	/// Gets or sets the collection of conditions that define this filter's criteria.
	/// </summary>
	/// <value>An array of search filter conditions.</value>
	[JsonPropertyName("conditions")]
	public SearchFilterCondition[] Conditions { get; set; }
}

/// <summary>
/// Represents a single condition within a search filter, defining field, operator, and values.
/// </summary>
/// <remarks>
/// Search filter conditions specify the actual filtering logic by defining which field
/// to filter on, what operator to use, and what values to compare against.
/// </remarks>
public class SearchFilterCondition
{
	/// <summary>
	/// Gets or sets the name of this condition.
	/// </summary>
	/// <value>The display name for this condition.</value>
	[JsonPropertyName("name")]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this condition is active.
	/// </summary>
	/// <value>True if this condition should be included in the filter logic; otherwise, false.</value>
	[JsonPropertyName("active")]
	public bool Active { get; set; }

	/// <summary>
	/// Gets or sets the field name that this condition applies to.
	/// </summary>
	/// <value>The name of the table field/column to filter.</value>
	[JsonPropertyName("field")]
	public string Field { get; set; }

	/// <summary>
	/// Gets or sets the comparison operator for this condition.
	/// </summary>
	/// <value>The operator to use when comparing field values (e.g., "equals", "contains", "greater_than").</value>
	[JsonPropertyName("operator")]
	public string Operator { get; set; }

	/// <summary>
	/// Gets or sets the values to compare against when applying this condition.
	/// </summary>
	/// <value>An array of string values used in the comparison operation.</value>
	[JsonPropertyName("values")]
	[JsonConverter(typeof(ToStringArrayConverter))]
	public string[] Values { get; set; }
}

/// <summary>
/// Represents filter settings for active/inactive/future records in table data.
/// </summary>
/// <remarks>
/// This filter type is commonly used for time-based or status-based filtering
/// where records can be categorized by their temporal state.
/// </remarks>
public class ActiveFilter
{
	/// <summary>
	/// Gets or sets the date reference for determining active/inactive status.
	/// </summary>
	/// <value>The date string used as the reference point for filtering.</value>
	[JsonPropertyName("date")]
	public string Date { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether active records should be included.
	/// </summary>
	/// <value>True to include active records in the results; otherwise, false.</value>
	[JsonPropertyName("active")]
	public bool Active { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether inactive records should be included.
	/// </summary>
	/// <value>True to include inactive records in the results; otherwise, false.</value>
	[JsonPropertyName("inactive")]
	public bool Inactive { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether future records should be included.
	/// </summary>
	/// <value>True to include future-dated records in the results; otherwise, false.</value>
	[JsonPropertyName("future")]
	public bool Future { get; set; }
}

/// <summary>
/// Represents column ordering configuration for table sorting.
/// </summary>
/// <remarks>
/// This class defines which column should be used for sorting and in what direction.
/// It's used in legacy version 2 configurations before being migrated to the newer format.
/// </remarks>
public class ColumnOrderBy
{
	/// <summary>
	/// Gets or sets the name of the column to sort by.
	/// </summary>
	/// <value>The column name used for ordering table data.</value>
	[JsonPropertyName("columnName")]
	public string ColumnName { get; set; }

	/// <summary>
	/// Gets or sets the sort direction for the column.
	/// </summary>
	/// <value>The sort order, typically "asc" for ascending or "desc" for descending.</value>
	[JsonPropertyName("sortOrder")]
	public string SortOrder { get; set; }
}

/// <summary>
/// Legacy version 2 table configuration model used for migrating to newer configuration formats.
/// </summary>
/// <remarks>
/// This class represents the structure of table configurations from version 2,
/// providing compatibility for migration to newer versions. It implements the
/// ITableConfiguration interface to ensure consistent handling during updates.
/// </remarks>
public class TableConfiguration : ITableConfiguration
{
	/// <inheritdoc/>
	[JsonPropertyName("name")]
	public string Name { get; set; }

	/// <inheritdoc/>
	[JsonIgnore]
	public int Version { get; set; }

	/// <inheritdoc/>
	[JsonIgnore]
	public string Uuid { get; set; }

	/// <summary>
	/// Gets or sets the column configuration settings for the table.
	/// </summary>
	/// <value>A list of column configurations defining visibility, order, and other column-specific settings.</value>
	[JsonPropertyName("columnConfiguration")]
	public List<ColumnConfiguration> ColumnConfiguration { get; set; } = [];

	/// <summary>
	/// Gets or sets the advanced search filters configured by users.
	/// </summary>
	/// <value>A list of advanced search filters that can be applied to table data.</value>
	[JsonPropertyName("advancedFilters")]
	public List<SearchFilter> AdvancedFilters { get; set; } = [];

	/// <summary>
	/// Gets or sets the column-specific filters mapped by column name.
	/// </summary>
	/// <value>A dictionary mapping column names to their respective search filters.</value>
	[JsonPropertyName("columnFilters")]
	public Dictionary<string, SearchFilter> ColumnFilters { get; set; } = [];

	/// <summary>
	/// Gets or sets the static filters applied to the table.
	/// </summary>
	/// <value>A dictionary of static filter keys and their corresponding values.</value>
	[JsonPropertyName("staticFilters")]
	public Dictionary<string, string> StaticFilters { get; set; } = [];

	/// <summary>
	/// Gets or sets the active filter configuration for the table.
	/// </summary>
	/// <value>The active filter settings controlling which records are displayed based on their status.</value>
	[JsonPropertyName("activeFilter")]
	public ActiveFilter ActiveFilter { get; set; }

	/// <summary>
	/// Gets or sets the column ordering configuration for the table.
	/// </summary>
	/// <value>The column ordering settings specifying which column to sort by and the sort direction.</value>
	[JsonPropertyName("columnOrderBy")]
	public ColumnOrderBy ColumnOrderBy { get; set; }

	/// <summary>
	/// Gets or sets the default column used for search operations.
	/// </summary>
	/// <value>The name of the column that should be searched by default when no specific column is selected.</value>
	[JsonPropertyName("defaultSearchColumn")]
	public string DefaultSearchColumn { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether line breaks should be applied in table display.
	/// </summary>
	/// <value>True if line breaks should be used in table rendering; otherwise, false.</value>
	[JsonPropertyName("lineBreak")]
	public bool LineBreak { get; set; }

	/// <summary>
	/// Gets or sets the number of rows to display per page in the table.
	/// </summary>
	/// <value>The number of rows per page for table pagination.</value>
	[JsonPropertyName("rowsPerPage")]
	public int RowsPerPage { get; set; }

	/// <summary>
	/// Gets or sets the active view mode for the table display.
	/// </summary>
	/// <value>The current view mode identifier (e.g., "list", "map", "cards").</value>
	[JsonPropertyName("activeViewMode")]
	public string ActiveViewMode { get; set; }

	/// <inheritdoc/>
	public string SerializeAsJson()
	{
		// Serialize only the properties that should be saved and ignore null values
		JsonSerializerOptions serializerOptions = new()
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};
		return JsonSerializer.Serialize(this, serializerOptions);
	}
}

/// <summary>
/// Handles table configuration updates from version 2 to version 3.
/// Migrates legacy v2 configuration format to the current table configuration structure.
/// </summary>
/// <remarks>
/// This update performs a comprehensive migration that includes:
/// - Converting column ordering from ColumnOrderBy to ColumnConfigurations
/// - Migrating ActiveFilter properties to the new format
/// - Consolidating ColumnFilters, AdvancedFilters, and StaticFilters into a unified Filters collection
/// - Updating version numbers and maintaining configuration integrity
/// </remarks>
public class TableConfigurationUpdate() : table.TableConfigurationUpdate(2)
{
	/// <summary>
	/// Parses encoded JSON configuration data into a version 2 table configuration object.
	/// </summary>
	/// <param name="encodedConfig">The JSON-encoded configuration string to parse.</param>
	/// <returns>
	/// A TableConfiguration object representing the parsed data, or a new empty configuration
	/// if parsing fails.
	/// </returns>
	/// <remarks>
	/// This method uses specific JSON serialization options to handle number-to-string conversion,
	/// which is commonly needed for filter values. If parsing fails, the error is logged and
	/// an empty configuration is returned to ensure the migration process can continue.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="encodedConfig"/> is null.</exception>
	public static TableConfiguration ParseTableConfigData(string encodedConfig)
	{
		// Set options to allow converting numbers to strings (used in advanced filters, column filters, searchbar filters)
		JsonSerializerOptions serializationOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
		};

		TableConfiguration tableConfig;

		try
		{
			tableConfig = JsonSerializer.Deserialize<TableConfiguration>(encodedConfig, serializationOptions);
		}
		catch (Exception e)
		{
			GenioDI.Log.Error($"Table Configuration Update (ParseTableConfigData) - {e.Message}");
			tableConfig = new TableConfiguration();
		}

		return tableConfig;
	}

	/// <summary>
	/// Applies version 2 specific changes to migrate a table configuration to version 3 format.
	/// </summary>
	/// <param name="sp">The persistent support instance for database operations.</param>
	/// <param name="user">The user who owns the configuration.</param>
	/// <param name="uuid">The unique identifier of the table configuration.</param>
	/// <param name="configName">The name of the configuration to update.</param>
	/// <returns>
	/// The migrated table configuration in the new format, or null if the configuration
	/// is already at a higher version than this update targets.
	/// </returns>
	/// <remarks>
	/// This method performs a comprehensive migration from version 2 to version 3:
	/// 1. Parses the existing v2 configuration from the database
	/// 2. Creates a new v3 configuration structure
	/// 3. Migrates column ordering from ColumnOrderBy to ColumnConfigurations with SortOrder and SortAsc
	/// 4. Converts ActiveFilter properties (Active/Inactive/Future to Current/Previous/Upcoming)
	/// 5. Consolidates all filter types (Static, Column, Advanced) into a unified Filters collection
	/// 6. Updates the version number to prepare for the next version
	///
	/// The migration ensures that all user preferences and settings are preserved while
	/// adapting to the new configuration schema.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
	public override ITableConfiguration GetChangedConfig(PersistentSupport sp, User user, string uuid, string configName)
	{
		CSGenioAtblcfg configRecord = TableUiSettings.GetTableConfigRecord(sp, user, uuid, configName);

		if (configRecord.ValUsrsetv > Version)
			return null;

		TableConfiguration oldTableConfig = ParseTableConfigData(configRecord.ValConfig);

		// Create a configuration with the new format
		table.TableConfiguration tableConfig = new()
		{
			Name = configRecord.ValName,
			Version = Version + 1,
			Uuid = configRecord.ValUuid,
			DefaultSearchColumn = oldTableConfig.DefaultSearchColumn,
			LineBreak = oldTableConfig.LineBreak,
			RowsPerPage = oldTableConfig.RowsPerPage,
			ActiveViewMode = oldTableConfig.ActiveViewMode,
			ColumnConfigurations = oldTableConfig.ColumnConfiguration
		};

		// Move ColumnOrderBy to ColumnConfigurations (SortOrder + SortAsc)
		if (tableConfig.ColumnConfigurations.Count > 0 && oldTableConfig.ColumnOrderBy != null)
		{
			ColumnConfiguration colConfig = tableConfig.ColumnConfigurations.Find(c => c.Name == oldTableConfig.ColumnOrderBy.ColumnName);
			if (colConfig != null)
			{
				colConfig.SortOrder = 1;
				colConfig.SortAsc = oldTableConfig.ColumnOrderBy.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);
			}
			else
				tableConfig.ColumnConfigurations[0].SortOrder = 1;
		}

		// Map old active filter properties to the new format
		table.ActiveFilter activeFilter = null;

		if (oldTableConfig.ActiveFilter != null)
		{
			activeFilter = new()
			{
				Date = oldTableConfig.ActiveFilter.Date,
				Current = oldTableConfig.ActiveFilter.Active,
				Previous = oldTableConfig.ActiveFilter.Inactive,
				Upcoming = oldTableConfig.ActiveFilter.Future
			};
		}

		// Migrate ColumnFilters, AdvancedFilters, StaticFilters and ActiveFilter to Filters
		if (activeFilter != null)
			tableConfig.Filters.Add(activeFilter);

		foreach (var filter in oldTableConfig.StaticFilters)
		{
			GroupFilter groupFilter = new()
			{
				Key = filter.Key,
				Value = filter.Value
			};
			tableConfig.Filters.Add(groupFilter);
		}

		List<SearchFilter> columnFilters = [.. oldTableConfig.ColumnFilters.Values, .. oldTableConfig.AdvancedFilters];
		foreach (SearchFilter filter in columnFilters)
		{
			foreach (SearchFilterCondition condition in filter.Conditions)
			{
				ColumnFilter columnFilter = new()
				{
					Field = condition.Field,
					Operator = condition.Operator,
					Values = [.. condition.Values]
				};
				tableConfig.Filters.Add(columnFilter);
			}
		}

		return tableConfig;
	}
}
