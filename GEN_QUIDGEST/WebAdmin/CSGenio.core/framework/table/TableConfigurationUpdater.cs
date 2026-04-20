using System;
using System.Collections.Generic;
using System.Text.Json;

using CSGenio.business;
using CSGenio.core.di;
using CSGenio.framework;
using CSGenio.persistence;

namespace CSGenio.core.framework.table;

/// <summary>
/// Interface defining the contract for table configuration version updates.
/// Implementations handle migrating table configurations when the user settings version changes.
/// </summary>
public interface ITableConfigurationUpdate
{
	/// <summary>
	/// Gets the version number that this update targets.
	/// </summary>
	/// <value>The target version number for this update implementation.</value>
	int Version { get; }

	/// <summary>
	/// Applies version-specific changes to a table configuration.
	/// </summary>
	/// <param name="sp">The persistent support instance for database operations.</param>
	/// <param name="user">The user who owns the configuration.</param>
	/// <param name="uuid">The unique identifier of the table configuration.</param>
	/// <param name="configName">The name of the configuration to update.</param>
	/// <returns>The updated table configuration, or null if the configuration is already at a higher version.</returns>
	ITableConfiguration GetChangedConfig(PersistentSupport sp, User user, string uuid, string configName);
}

/// <summary>
/// Abstract base class for table configuration updates that provides common functionality
/// for version-specific configuration migrations.
/// </summary>
/// <param name="version">The target version number for this update implementation.</param>
public abstract class TableConfigurationUpdate(int version) : ITableConfigurationUpdate
{
	/// <inheritdoc/>
	public int Version { get; } = version;

	/// <inheritdoc/>
	public abstract ITableConfiguration GetChangedConfig(PersistentSupport sp, User user, string uuid, string configName);
}

/// <summary>
/// Provides static methods for updating table configurations to match current user settings versions.
/// Handles version migration, database persistence, and maintains backward compatibility
/// through a registry of version-specific update implementations.
/// </summary>
/// <remarks>
/// This updater automatically applies necessary changes when table configurations
/// are loaded with versions older than the current system version. It includes
/// transaction handling and error management for database operations.
/// </remarks>
public static class TableConfigurationUpdater
{
	/// <summary>
	/// Registry of all available table configuration update implementations,
	/// ordered by version for sequential application.
	/// </summary>
	private static readonly List<ITableConfigurationUpdate> tableConfigUpdates =
	[
		new legacy.v1.TableConfigurationUpdate(),
		new legacy.v2.TableConfigurationUpdate()
	];

	/// <summary>
	/// Updates a table configuration record in the database with the current configuration data.
	/// </summary>
	/// <param name="sp">The persistent support instance for database operations.</param>
	/// <param name="configRecord">The database record to update.</param>
	/// <param name="tableConfig">The table configuration containing the data to save.</param>
	/// <remarks>
	/// This method serializes the table configuration to JSON, updates the version number,
	/// and persists the changes. A transaction should be handled externally before calling this method.
	/// </remarks>
	/// <exception cref="JsonException">Thrown when the table configuration cannot be serialized to JSON.</exception>
	private static void UpdateTableConfigurationDbRecord(PersistentSupport sp, CSGenioAtblcfg configRecord, ITableConfiguration tableConfig)
	{
		configRecord.ValConfig = tableConfig.SerializeAsJson();

		// Update version
		configRecord.ValUsrsetv = tableConfig.Version;

		// Save to the database
		configRecord.update(sp);
	}

	/// <summary>
	/// Determines whether a table configuration requires a version update.
	/// </summary>
	/// <param name="tableConfig">The table configuration to check for version compatibility.</param>
	/// <returns>
	/// True if the configuration's version is older than the current user settings version
	/// and therefore needs updating; otherwise, false.
	/// </returns>
	/// <remarks>
	/// This method performs a simple version comparison to determine if migration is needed.
	/// It's useful for checking update requirements without actually applying the changes.
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="tableConfig"/> is null.</exception>
	public static bool NeedsVersionChange(TableConfiguration tableConfig)
	{
		return tableConfig.Version < Configuration.UserSettingsVersion;
	}

	/// <summary>
	/// Updates a table configuration object and its corresponding database record if necessary.
	/// </summary>
	/// <param name="sp">The persistent support instance for database operations.</param>
	/// <param name="user">The user who owns the configuration.</param>
	/// <param name="configRecord">The database record associated with the configuration.</param>
	/// <returns>True if all operations completed successfully; otherwise, false.</returns>
	/// <remarks>
	/// This is the main entry point for table configuration updates. It applies version changes
	/// and conditionally saves the updated configuration to the database if changes were made
	/// and the system is not in maintenance mode. This ensures configurations stay current
	/// with system updates while respecting maintenance restrictions.
	///
	/// The method performs the following operations:
	/// 1. Checks if the system is in maintenance mode or if the version is already current
	/// 2. Opens a database transaction
	/// 3. Iterates through all available configuration updates
	/// 4. Applies each update sequentially if needed
	/// 5. Commits the transaction on success or rolls back on failure
	/// </remarks>
	/// <exception cref="ArgumentNullException">Thrown when any of the parameters are null.</exception>
	public static bool UpdateTableConfiguration(PersistentSupport sp, User user, CSGenioAtblcfg configRecord)
	{
		// Shouldn't be done while in maintenance mode or if the version is already up to date
		if (Maintenance.Current.IsActive || configRecord.ValUsrsetv >= Configuration.UserSettingsVersion)
			return true;

		try
		{
			sp.openTransaction();

			// Update table configuration with load options accounting for version changes
			foreach (var configUpdate in tableConfigUpdates)
			{
				ITableConfiguration tableConfig = configUpdate.GetChangedConfig(
					sp,
					user,
					configRecord.ValUuid,
					configRecord.ValName);

				// Save updated table configuration to the database. The configuration being null means
				// it's already at a version higher than the current update and, therefore, couldn't be parsed
				if (tableConfig != null)
					UpdateTableConfigurationDbRecord(sp, configRecord, tableConfig);
			}

			sp.closeTransaction();

			return true;
		}
		catch (Exception e)
		{
			sp.rollbackTransaction();
			GenioDI.Log.Error($"Table Configuration Updater (UpdateTableConfiguration) - {e.Message}");
			return false;
		}
	}
}
