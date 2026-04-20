using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSGenio.business;
using CSGenio.core.di;
using Quidgest.Persistence;

namespace CSGenio.core.framework.table;

/// <summary>
/// Defines the level of table structure customization and persistence allowed to users.
/// </summary>
public enum TableManagementMode
{
	/// <summary>
	/// No modifications allowed. The table structure is fixed and read-only.
	/// </summary>
	None,

	/// <summary>
	/// Modifications are allowed during the session but are discarded upon exit.
	/// </summary>
	NonPersistent,

	/// <summary>
	/// Modifications are automatically persisted to a single, user-specific table configuration.
	/// </summary>
	PersistOne,

	/// <summary>
	/// Full management capabilities including creation, modification, and deletion of multiple named table configurations.
	/// </summary>
	PersistMany
}

public interface ITableConfiguration
{
	/// <summary>
	/// Gets or sets the name identifier of this table configuration.
	/// </summary>
	string Name { get; set; }

	/// <summary>
	/// Gets or sets the version number of this configuration (not serialized to JSON).
	/// </summary>
	int Version { get; set; }

	/// <summary>
	/// Gets or sets the unique identifier for the table/view (not serialized to JSON).
	/// </summary>
	string Uuid { get; set; }

	/// <summary>
	/// Serializes the table configuration to a JSON string representation.
	/// This method converts the configuration object into a JSON format that can be
	/// stored persistently (e.g., in a database) or transmitted over a network.
	/// Only the properties intended for persistence are included in the serialization.
	/// </summary>
	/// <returns>
	/// A JSON string containing the serialized table configuration data.
	/// </returns>
	string SerializeAsJson();
}

/// <summary>
/// Represents the saved/persistent portion of table configuration that can be stored and restored.
/// Contains column settings, filters, and display preferences.
/// </summary>
public class PersistedTableConfiguration
{
	/// <summary>
	/// Gets or sets the default column name to use for search operations.
	/// </summary>
	[JsonPropertyName("defaultSearchColumn")]
	public string DefaultSearchColumn { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether line breaks are enabled in table cells.
	/// </summary>
	[JsonPropertyName("lineBreak")]
	public bool LineBreak { get; set; }

	/// <summary>
	/// Gets or sets the number of rows to display per page in the table.
	/// </summary>
	[JsonPropertyName("rowsPerPage")]
	public int RowsPerPage { get; set; }

	/// <summary>
	/// Gets or sets the currently active view mode for the table display.
	/// </summary>
	[JsonPropertyName("activeViewMode")]
	public string ActiveViewMode { get; set; }

	/// <summary>
	/// Gets or sets the configuration for each column including visibility, order, and exportability.
	/// </summary>
	[JsonPropertyName("columnConfiguration")]
	public List<ColumnConfiguration> ColumnConfigurations { get; set; } = [];

	/// <summary>
	/// Gets or sets the list of filters applied to the table.
	/// </summary>
	[JsonPropertyName("filters")]
	public List<ITableFilter> Filters { get; set; } = [];

	/// <summary>
	/// Gets or sets form field global filters that are external to the table.
	/// </summary>
	[JsonPropertyName("globalFilters")]
	public Dictionary<string, IGlobalFilter> GlobalFilters { get; set; } = [];
}

/// <summary>
/// Represents the complete table configuration including both persistent settings and runtime state.
/// Extends <see cref="PersistedTableConfiguration"/> with additional properties for current session state.
/// </summary>
public class TableConfiguration : PersistedTableConfiguration, ITableConfiguration
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
	/// Gets or sets the current page number in paginated table results.
	/// Defaults to 1.
	/// </summary>
	[JsonPropertyName("page")]
	public int Page { get; set; } = 1;

	/// <summary>
	/// Gets or sets the list of currently selected row identifiers.
	/// </summary>
	[JsonPropertyName("selectedRows")]
	public List<string> SelectedRows { get; set; } = [];

	/// <summary>
	/// Gets a collection of all column-based search filters.
	/// This includes advanced filters, column filters, and search bar filters.
	/// Computed at runtime and not serialized to JSON.
	/// </summary>
	[JsonIgnore]
	public IReadOnlyList<ColumnFilter> SearchFilters => [.. Filters.OfType<ColumnFilter>()
		.SelectMany<ColumnFilter, ColumnFilter>(f => [f, .. f.SubFilters.OfType<ColumnFilter>()])];

	/// <summary>
	/// Gets a collection of active filters applied to the table.
	/// Computed at runtime and not serialized to JSON.
	/// </summary>
	[JsonIgnore]
	public IReadOnlyList<ActiveFilter> ActiveFilters => [.. Filters.OfType<ActiveFilter>()];

	/// <summary>
	/// Gets a dictionary of group filters applied to the table.
	/// Keys are the filter field names and values are their corresponding filter values.
	/// Computed at runtime and not serialized to JSON.
	/// </summary>
	[JsonIgnore]
	public IReadOnlyDictionary<string, string> GroupFilters => Filters.OfType<GroupFilter>().ToDictionary(f => f.Key, f => f.Value);

	/// <summary>
	/// Gets the configuration of the first visible column, based on the column configuration list.
	/// Computed at runtime and not serialized to JSON.
	/// </summary>
	[JsonIgnore]
	public ColumnConfiguration FirstVisibleColumnConfig => ColumnConfigurations?.FirstOrDefault(column => column.Visibility == 1);

	/// <summary>
	/// Gets the first visible column as a <see cref="FieldRef"/>.
	/// </summary>
	/// <param name="mainTableName">Name of the main table.</param>
	/// <returns>
	/// A <see cref="FieldRef"/> representing the first visible column, or <c>null</c> if no visible columns exist.
	/// </returns>
	public FieldRef GetFirstVisibleColumn(string mainTableName)
	{
		return ColumnConfiguration.GetFieldRef(mainTableName, FirstVisibleColumnConfig?.Name);
	}

	/// <summary>
	/// Determines the number of rows per page to use based on the table's configuration and provided options.
	/// </summary>
	/// <param name="defaultRowsPerPage">Default number of rows per page.</param>
	/// <param name="rowsPerPageOptionsString">Rows per page options as a string of values separated by commas.</param>
	/// <returns>
	/// The number of rows per page to use, falling back to the default if the configured value is not valid.
	/// </returns>
	public int DetermineRowsPerPage(int defaultRowsPerPage, string rowsPerPageOptionsString)
	{
		List<int> rowsPerPageOptions = [];

		// Split string into array of string values
		string[] optionsStrArr = string.IsNullOrEmpty(rowsPerPageOptionsString) ? [] : rowsPerPageOptionsString.Split(',');

		// Convert string values to integers and add to list
		foreach (string str in optionsStrArr)
			if (int.TryParse(str, out int res))
				rowsPerPageOptions.Add(res);

		// If rows per page is the default or a value in the defined options, use it
		if (RowsPerPage == defaultRowsPerPage ||
			(rowsPerPageOptions != null && rowsPerPageOptions.Contains(RowsPerPage)))
			return RowsPerPage;

		// If not, use the default
		return defaultRowsPerPage;
	}

	/// <summary>
	/// Gets the visible column names in the format <c>TABLE_NAME.COLUMN_NAME</c>,
	/// based on the column configuration.
	/// </summary>
	/// <param name="mainTableName">Name of the main table.</param>
	/// <returns>
	/// A list of visible column names in the format <c>tablename.columnname</c>.
	/// Returns an empty list if no columns are visible.
	/// </returns>
	public List<string> GetVisibleColumnNames(string mainTableName)
	{
		if (ColumnConfigurations.Count == 0)
			return [];

		List<string> visibleColumnNames = [];

		foreach (ColumnConfiguration columnConfig in ColumnConfigurations)
		{
			if (columnConfig.Visibility == 1)
			{
				string tableName = ColumnConfiguration.GetTableName(mainTableName, columnConfig.Name);
				string columnName = ColumnConfiguration.GetColumnName(columnConfig.Name);

				if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
					continue;

				visibleColumnNames.Add(tableName + "." + columnName);
			}
		}

		return visibleColumnNames;
	}

	/// <summary>
	/// Gets all the valid search filters from the column configuration.
	/// Filters out conditions that reference non-searchable or non-visible columns.
	/// </summary>
	/// <param name="mainTableName">Name of the main table.</param>
	/// <param name="searchableColumnNames">Names of the columns that can be searched.</param>
	/// <returns>
	/// A list of valid <see cref="ColumnFilter"/> objects that reference searchable and visible columns.
	/// </returns>
	public List<ColumnFilter> GetValidSearchFilters(string mainTableName, List<string> searchableColumnNames)
	{
		// Clone the current search filters so the originals are not changed
		// The original data must be kept so the invalid filters can be included if they become valid again
		List<ColumnFilter> clonedSearchFilters = [.. SearchFilters.Select(f => f.Clone() as ColumnFilter)];
		List<ColumnFilter> validSearchFilters = [];

		List<string> customVisibleColumnNames = GetVisibleColumnNames(mainTableName);
		List<string> searchableVisibleColumnNames = customVisibleColumnNames.Count > 0
			? searchableColumnNames.Select(x => x.ToLowerInvariant()).Where(customVisibleColumnNames.Contains).ToList()
			: searchableColumnNames.Select(x => x.ToLowerInvariant()).ToList();

		// Only include filters that use visible / searchable fields
		foreach (ColumnFilter filter in clonedSearchFilters)
			if (string.IsNullOrWhiteSpace(filter.Field) || searchableVisibleColumnNames.Contains(filter.Field.ToLowerInvariant()))
				validSearchFilters.Add(filter);

		return validSearchFilters;
	}

	/// <inheritdoc/>
	public virtual string SerializeAsJson()
	{
		// Create a temporary copy with only unique global filters for persistence
		PersistedTableConfiguration tempConfig = new()
		{
			DefaultSearchColumn = DefaultSearchColumn,
			LineBreak = LineBreak,
			RowsPerPage = RowsPerPage,
			ActiveViewMode = ActiveViewMode,
			ColumnConfigurations = ColumnConfigurations,
			Filters = Filters,
			GlobalFilters = GlobalFilters.Where(kvp => kvp.Value.IsUnique).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
		};

		// Serialize only the properties that should be saved and ignore null values
		JsonSerializerOptions serializerOptions = new()
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};
		return JsonSerializer.Serialize(tempConfig, serializerOptions);
	}

	/// <summary>
	/// Deserializes a JSON-encoded table configuration string into a TableConfiguration object.
	/// This method handles the parsing of user preferences and settings that were previously
	/// serialized and stored (e.g., in a database or configuration file).
	/// </summary>
	/// <param name="encodedConfig">
	/// A JSON string containing the serialized table configuration data including
	/// column settings, filters, view preferences, and other table state information.
	/// </param>
	/// <returns>
	/// A <see cref="PersistedTableConfiguration"/> object populated with the deserialized data.
	/// If deserialization fails, returns a new empty PersistedTableConfiguration instance.
	/// </returns>
	/// <remarks>
	/// The method uses relaxed JSON serialization options to handle:
	/// - Case-insensitive property matching for better compatibility
	/// - Number-to-string conversion for filter values that may be stored as different types
	///
	/// Any deserialization errors are logged and a default configuration is returned
	/// to ensure the application continues functioning even with corrupted configuration data.
	/// </remarks>
	public static PersistedTableConfiguration ParseTableConfigData(string encodedConfig)
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
			GenioDI.Log.Error($"Table Configuration (ParseTableConfigData) - {e.Message}");
			tableConfig = new TableConfiguration();
		}

		return tableConfig;
	}

	/// <summary>
	/// Creates a TableConfiguration object from a database configuration record.
	/// This method combines the JSON configuration data with metadata stored in separate fields
	/// of the database record to create a complete table configuration instance.
	/// </summary>
	/// <param name="configRecord">
	/// A <see cref="CSGenioAtblcfg"/> database record containing:
	/// - ValConfig: JSON-serialized configuration data
	/// - ValName: Human-readable name/identifier for the configuration
	/// - ValUsrsetv: Version number for tracking configuration changes
	/// - ValUuid: Unique identifier for the associated table/view
	/// </param>
	/// <returns>
	/// A fully populated <see cref="TableConfiguration"/> object with both the deserialized
	/// JSON data and the metadata from the database record fields.
	/// </returns>
	/// <remarks>
	/// This method is typically used when loading saved table configurations from the database.
	/// It leverages the JSON parsing method for the configuration data while adding the
	/// non-serialized metadata properties that are stored separately in the database schema.
	///
	/// The resulting configuration can be used to restore a user's previous table state
	/// including column arrangements, filters, pagination settings, and view preferences.
	/// </remarks>
	public static TableConfiguration ParseTableConfigData(CSGenioAtblcfg configRecord)
	{
		// Parse to object
		TableConfiguration tableConfig = ParseTableConfigData(configRecord.ValConfig) as TableConfiguration;
		// Add configuration name
		tableConfig.Name = configRecord.ValName;
		// Add configuration version
		tableConfig.Version = configRecord.ValUsrsetv;
		// Add table identifier
		tableConfig.Uuid = configRecord.ValUuid;

		return tableConfig;
	}
}
