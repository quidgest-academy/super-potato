using System.Collections.Generic;
using System.Linq;

using CSGenio.core.framework.table;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.business;

/// <summary>
/// Manages user interface settings for table configurations in VUE applications.
/// This class handles loading, caching, and manipulation of table view configurations
/// including default settings, user-specific configurations, and configuration persistence.
/// </summary>
/// <param name="sp">The persistence support instance for database operations.</param>
/// <param name="user">The user whose table configurations are being managed.</param>
/// <param name="uuid">The unique identifier for the table/view being configured.</param>
/// <param name="key">The cache key used to identify this settings instance.</param>
public class TableUiSettings(PersistentSupport sp, User user, string uuid, string key) : UserUiSettings(key)
{
	private readonly PersistentSupport sp = sp;
	private readonly string uuid = uuid;
	private readonly User user = user;

	/// <summary>
	/// Gets the default table configuration for the current table and user.
	/// This configuration is marked as the default in the database and serves as the
	/// fallback when no specific configuration is requested.
	/// </summary>
	/// <value>The default table configuration, or null if none exists.</value>
	public TableConfiguration DefaultTableConfiguration { get; private set; }

	/// <summary>
	/// Gets the list of names for all available table configurations for the current user and table.
	/// These are user-defined configurations that can be loaded by name.
	/// </summary>
	/// <value>A list of configuration names available to the current user.</value>
	public List<string> UserTableConfigNames { get; private set; }

	/// <summary>
	/// Loads user settings including the default table configuration and all available configuration names.
	/// This method populates the <see cref="DefaultTableConfiguration"/> and <see cref="UserTableConfigNames"/> properties.
	/// </summary>
	/// <remarks>
	/// This method is called during initialization to retrieve all relevant table configuration
	/// data from the database and prepare it for use by the application.
	/// </remarks>
	private void LoadUserSettings()
	{
		DefaultTableConfiguration = GetTableDefaultConfig(sp, user, uuid);
		UserTableConfigNames = GetTableConfigNames(sp, user, uuid);
	}

	/// <summary>
	/// Creates a TableConfiguration object from a database configuration record.
	/// This method handles version updates, parsing, and property assignment for the configuration.
	/// </summary>
	/// <param name="sp">The persistence support instance for database operations.</param>
	/// <param name="user">The user associated with the configuration.</param>
	/// <param name="configRecord">The database record containing the configuration data.</param>
	/// <returns>
	/// A fully populated <see cref="TableConfiguration"/> object with name, version, and UUID set,
	/// or null if the configuration update failed.
	/// </returns>
	/// <remarks>
	/// This method automatically applies any necessary version updates to the configuration
	/// before parsing and returning it. If version updates fail, null is returned.
	/// </remarks>
	private static TableConfiguration GetConfigFromRecord(PersistentSupport sp, User user, CSGenioAtblcfg configRecord)
	{
		// Update the table configuration, if necessary
		return TableConfigurationUpdater.UpdateTableConfiguration(sp, user, configRecord)
			? TableConfiguration.ParseTableConfigData(configRecord)
			: null;
	}

	/// <summary>
	/// Retrieves table UI settings from the application cache.
	/// </summary>
	/// <param name="cacheKey">The unique cache key identifying the settings to retrieve.</param>
	/// <returns>The cached <see cref="TableUiSettings"/> instance, or null if not found in cache.</returns>
	/// <remarks>
	/// This method provides type-safe access to cached table UI settings by casting the result
	/// from the base UserUiSettings cache lookup.
	/// </remarks>
	protected new static TableUiSettings GetFromCache(string cacheKey)
	{
		return UserUiSettings.GetFromCache(cacheKey) as TableUiSettings;
	}

	/// <summary>
	/// Loads table user interface settings from cache or database.
	/// If settings are not found in cache, they are loaded from the database and then cached.
	/// This is the primary method for obtaining a configured <see cref="TableUiSettings"/> instance.
	/// </summary>
	/// <param name="sp">The persistence support instance for database operations.</param>
	/// <param name="user">The user for whom to load settings.</param>
	/// <param name="uuid">The unique identifier for the table/view whose settings should be loaded.</param>
	/// <returns>A fully configured <see cref="TableUiSettings"/> instance with loaded settings.</returns>
	/// <remarks>
	/// This method implements a cache-first strategy: it checks the cache first, and only
	/// loads from the database if no cached version is found. Once loaded, settings are
	/// automatically cached for future use.
	/// </remarks>
	public static TableUiSettings Load(PersistentSupport sp, User user, string uuid)
	{
		string cacheKey = GenerateCacheKey(uuid, user);
		TableUiSettings settings = GetFromCache(cacheKey);

		if (settings == null)
		{
			settings = new TableUiSettings(sp, user, uuid, cacheKey);
			settings.LoadUserSettings();
			settings.CacheSettings();
		}

		return settings;
	}

	/// <summary>
	/// Retrieves a table configuration record from the database by name.
	/// This method returns the raw database record containing the configuration data.
	/// </summary>
	/// <param name="sp">The persistence support instance for database operations.</param>
	/// <param name="user">The user who owns the configuration.</param>
	/// <param name="uuid">The unique identifier for the table/view.</param>
	/// <param name="configName">The name of the specific configuration to retrieve.</param>
	/// <returns>
	/// A <see cref="CSGenioAtblcfg"/> record containing the configuration data,
	/// or null if no matching configuration is found.
	/// </returns>
	/// <remarks>
	/// This method performs a direct database lookup without applying version updates
	/// or parsing the configuration data. Use <see cref="GetTableConfig"/> if you need
	/// a fully processed configuration object.
	/// </remarks>
	public static CSGenioAtblcfg GetTableConfigRecord(PersistentSupport sp, User user, string uuid, string configName)
	{
		// Get saved configuration
		return CSGenioAtblcfg.searchList(
			sp,
			user,
			CriteriaSet.And()
				.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
				.Equal(CSGenioAtblcfg.FldUuid, uuid)
				.Equal(CSGenioAtblcfg.FldName, configName)
			)
			.FirstOrDefault();
	}

	/// <summary>
	/// Retrieves and parses a named table configuration from the database.
	/// This method loads the configuration data and applies any necessary version updates.
	/// </summary>
	/// <param name="sp">The persistence support instance for database operations.</param>
	/// <param name="user">The user who owns the configuration.</param>
	/// <param name="uuid">The unique identifier for the table/view.</param>
	/// <param name="configName">The name of the specific configuration to retrieve.</param>
	/// <returns>
	/// A fully parsed <see cref="TableConfiguration"/> object with name and version information,
	/// or null if the configuration doesn't exist or version updates failed.
	/// </returns>
	/// <remarks>
	/// This method combines database retrieval with configuration processing, including
	/// automatic version updates. It's the recommended way to load named configurations
	/// for application use.
	/// </remarks>
	public static TableConfiguration GetTableConfig(PersistentSupport sp, User user, string uuid, string configName)
	{
		// Get record from the database
		CSGenioAtblcfg configRecord = GetTableConfigRecord(sp, user, uuid, configName);

		return configRecord == null
			? null
			: GetConfigFromRecord(sp, user, configRecord);
	}

	/// <summary>
	/// Retrieves the default table configuration from the database for a specific table and user.
	/// The default configuration is identified by having the 'IsDefault' flag set to 1 in the database.
	/// </summary>
	/// <param name="sp">The persistence support instance for database operations.</param>
	/// <param name="user">The user whose default configuration should be retrieved.</param>
	/// <param name="uuid">The unique identifier for the table/view.</param>
	/// <returns>
	/// The default <see cref="TableConfiguration"/> for the specified table and user,
	/// or null if no default configuration exists or version updates failed.
	/// </returns>
	/// <remarks>
	/// This method automatically applies version updates to outdated configurations.
	/// Only one configuration per user and table can be marked as default in the database.
	/// </remarks>
	public static TableConfiguration GetTableDefaultConfig(PersistentSupport sp, User user, string uuid)
	{
		// Get record from the database
		CSGenioAtblcfg configRecord = CSGenioAtblcfg.searchList(
			sp,
			user,
			CriteriaSet.And()
				.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
				.Equal(CSGenioAtblcfg.FldUuid, uuid)
				.Equal(CSGenioAtblcfg.FldIsdefault, 1)
			)
			.FirstOrDefault();

		return configRecord == null
			? null
			: GetConfigFromRecord(sp, user, configRecord);
	}

	/// <summary>
	/// Retrieves the names of all available table configurations for a specific user and table.
	/// This method returns only the configuration names, not the full configuration data.
	/// The returned list is ordered alphabetically by configuration name.
	/// </summary>
	/// <param name="sp">The persistence support instance for database operations.</param>
	/// <param name="user">The user whose configurations should be retrieved.</param>
	/// <param name="uuid">The unique identifier for the table/view.</param>
	/// <returns>A list of configuration names available to the specified user for the given table, ordered alphabetically.</returns>
	/// <remarks>
	/// This is an efficient way to get a list of available configurations without loading
	/// the full configuration data. Useful for populating configuration selection UI elements.
	/// The results are sorted alphabetically for consistent and predictable ordering.
	/// </remarks>
	public static List<string> GetTableConfigNames(PersistentSupport sp, User user, string uuid)
	{
		return CSGenioAtblcfg.searchList(
			sp,
			user,
			CriteriaSet.And()
				.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
				.Equal(CSGenioAtblcfg.FldUuid, uuid),
			[CSGenioAtblcfg.FldName.Field])
			.Select(c => c.ValName)
			.OrderBy(name => name)
			.ToList();
	}

	/// <summary>
	/// Determines which table configuration to use based on the provided parameters and loading preferences.
	/// This method implements the logic for choosing between current, default, named, or fallback configurations.
	/// </summary>
	/// <param name="currentTableConfig">The currently active table configuration.</param>
	/// <param name="configName">The name of a specific configuration to load.</param>
	/// <param name="loadDefaultView">If true, forces loading of the default configuration.</param>
	/// <param name="defaultTableConfig">A fallback configuration to use if no other configuration is available.</param>
	/// <returns>
	/// The most appropriate <see cref="TableConfiguration"/> based on the following priority:
	/// 1. Default configuration (if <paramref name="loadDefaultView"/> is true)
	/// 2. Named configuration (if <paramref name="configName"/> is provided)
	/// 3. Current configuration (<paramref name="currentTableConfig"/>)
	/// 4. Provided <paramref name="defaultTableConfig"/>
	/// 5. New empty configuration (as final fallback)
	/// </returns>
	/// <remarks>
	/// This method encapsulates the business logic for configuration selection, ensuring
	/// that appropriate fallbacks are used when preferred configurations are not available.
	/// It never returns null - there is always a fallback to a new empty configuration.
	/// </remarks>
	public TableConfiguration DetermineTableConfig(TableConfiguration currentTableConfig = null, string configName = "", bool loadDefaultView = false, TableConfiguration defaultTableConfig = null)
	{
		// Default to the current table configuration
		TableConfiguration tableConfig = currentTableConfig;

		// If loading the default configuration
		if (!string.IsNullOrEmpty(uuid) && loadDefaultView)
			tableConfig = DefaultTableConfiguration;
		// If loading a saved table configuration
		else if (!string.IsNullOrEmpty(uuid) && !string.IsNullOrEmpty(configName))
			tableConfig = GetTableConfig(sp, user, uuid, configName);

		tableConfig ??= defaultTableConfig ?? new TableConfiguration();

		return tableConfig;
	}

	/// <summary>
	/// Gets the names of all available table configurations for the current user and table instance.
	/// This is an instance method that uses the cached configuration names loaded during initialization.
	/// </summary>
	/// <returns>A list of configuration names available to the current user for the current table.</returns>
	/// <remarks>
	/// This method provides access to the cached configuration names without requiring
	/// additional database queries. The names are loaded once during initialization.
	/// </remarks>
	/// <seealso cref="GetTableConfigNames(PersistentSupport, User, string)"/>
	public List<string> GetTableConfigNames()
	{
		return UserTableConfigNames;
	}
}
