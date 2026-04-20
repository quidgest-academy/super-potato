using System;
using System.Collections.Generic;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;

namespace CSGenio.core.framework.table.legacy.v1;

/// <summary>
/// Handles table configuration updates from version 1 to version 2.
/// Specifically manages the migration of static filter keys that have changed
/// due to modifications in filter ordering or numbering schemes.
/// </summary>
/// <remarks>
/// This update addresses changes in static filter key values that occurred between
/// version 1 and later versions. It uses cached shift values to correctly translate
/// old filter selections to their new key positions, ensuring user filter preferences
/// are preserved across version upgrades.
/// </remarks>
public class TableConfigurationUpdate() : table.TableConfigurationUpdate(1)
{
	/// <summary>
	/// Generates a cache key for storing version 1 update data for a specific table.
	/// </summary>
	/// <param name="uuid">The unique identifier of the table.</param>
	/// <returns>A cache key string formatted for version 1 table updates.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="uuid"/> is null.</exception>
	private static string GetCacheKey(string uuid) => $"TableUpdateV1_{uuid}";

	/// <summary>
	/// Sets a shift value for a specific filter within a table's update configuration.
	/// This value will be used during the migration process to adjust filter key positions.
	/// </summary>
	/// <param name="uuid">The unique identifier of the table being configured.</param>
	/// <param name="filterId">The identifier of the filter group requiring a shift adjustment.</param>
	/// <param name="filterValue">The shift value representing the difference between old and new key positions.</param>
	/// <remarks>
	/// This method stores shift values in the user cache, which are later retrieved during
	/// the actual migration process. Multiple filters can have different shift values
	/// within the same table configuration.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="uuid"/> or <paramref name="filterId"/> is null.</exception>
	public static void SetFilterShiftValue(string uuid, string filterId, int filterValue)
	{
		string cacheKey = GetCacheKey(uuid);

		if (QCache.Instance.User.Get(cacheKey) is not Dictionary<string, int> keyShiftValues)
			keyShiftValues = [];

		keyShiftValues[filterId] = filterValue;
		QCache.Instance.User.Put(cacheKey, keyShiftValues);
	}

	/// <summary>
	/// Applies version 1 specific changes to a table configuration, primarily updating static filter keys.
	/// </summary>
	/// <param name="sp">The persistent support instance for database operations.</param>
	/// <param name="user">The user who owns the configuration.</param>
	/// <param name="uuid">The unique identifier of the table configuration.</param>
	/// <param name="configName">The name of the configuration to update.</param>
	/// <returns>
	/// The updated table configuration with migrated static filter keys, or null if the configuration
	/// is already at a higher version or no shift values are cached.
	/// </returns>
	/// <remarks>
	/// This method processes each static filter in the configuration by:
	/// 1. Parsing the current filter selection values (stored as character digits)
	/// 2. Applying the appropriate shift value to adjust for key position changes
	/// 3. Reconstructing the filter selection string with the new values
	/// 4. Clearing the cached shift values after successful migration
	///
	/// The migration only occurs if the table configuration version is at or below version 1,
	/// preventing unnecessary processing of already-updated configurations.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
	public override ITableConfiguration GetChangedConfig(PersistentSupport sp, User user, string uuid, string configName)
	{
		CSGenioAtblcfg configRecord = TableUiSettings.GetTableConfigRecord(sp, user, uuid, configName);

		if (configRecord.ValUsrsetv > Version)
			return null;

		string cacheKey = GetCacheKey(uuid);

		if (QCache.Instance.User.Get(cacheKey) is not Dictionary<string, int> keyShiftValues)
			return null;

		Dictionary<string, string> updatedStaticFilters = [];
		v2.TableConfiguration oldTableConfig = v2.TableConfigurationUpdate.ParseTableConfigData(configRecord.ValConfig);

		foreach (var entry in oldTableConfig.StaticFilters)
		{
			List<int> valueList = [];

			// Create an array of which filters are selected
			// with each value changed by the value in keyShiftValues that corresponds to this filter group.
			// This is the difference between the old starting value for the filter "order" field and the new one.
			foreach (char filterKey in entry.Value)
				if (int.TryParse(filterKey.ToString(), out int parsedValue) && keyShiftValues.ContainsKey(entry.Key))
					valueList.Add(parsedValue + keyShiftValues[entry.Key]);

			// Convert the new value of which filters are selected back to a string and add to result
			updatedStaticFilters.Add(entry.Key, string.Join(string.Empty, valueList));
		}

		oldTableConfig.StaticFilters = updatedStaticFilters;

		oldTableConfig.Name = configRecord.ValName;
		oldTableConfig.Version = Version + 1;
		oldTableConfig.Uuid = configRecord.ValUuid;

		QCache.Instance.User.Invalidate(cacheKey);

		return oldTableConfig;
	}
}
