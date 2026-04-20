using System;
using System.Collections.Generic;
using System.Linq;

using CSGenio.business;
using CSGenio.core.di;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence.GenericQuery;

namespace CSGenio.core.framework.table;

/// <summary>
/// Provides static methods for managing table configuration settings including saving, loading,
/// selecting, deleting, renaming, and copying user-specific table configurations.
/// </summary>
/// <remarks>
/// This manager handles table view configurations that are persisted in the database and
/// associated with specific users. It supports default configuration management and includes
/// transaction handling with proper error management and cache invalidation.
/// </remarks>
public static class TableConfigurationManager
{
	/// <summary>
	/// Validates that the system is not in maintenance mode before allowing configuration changes.
	/// </summary>
	/// <param name="user">The user attempting to perform the operation.</param>
	/// <param name="methodName">The name of the method being validated for error reporting.</param>
	/// <exception cref="BusinessException">Thrown when the system is in maintenance mode.</exception>
	private static void ValidateMaintenanceStatus(User user, string methodName)
	{
		// Don't allow changes in maintenance mode
		if (Maintenance.Current.IsActive)
			throw new BusinessException(Translations.Get("O Sistema encontra-se em manutenção! Pedimos desculpa pelo incómodo.", user.Language), methodName, $"The system is under maintenance.");
	}

	/// <summary>
	/// Clears the default flag from all existing default configurations for a specific user and table UUID.
	/// </summary>
	/// <param name="user">The user whose default configurations should be cleared.</param>
	/// <param name="sp">The persistent support instance for database operations.</param>
	/// <param name="uuid">The unique identifier of the table whose default configurations should be cleared.</param>
	/// <remarks>
	/// This method is called internally to ensure only one configuration can be marked as default
	/// for a given user and table combination.
	/// </remarks>
	private static void ClearDefaultConfig(User user, PersistentSupport sp, string uuid)
	{
		UpdateQuery query = new UpdateQuery()
			.Update(Area.AreaTBLCFG)
			.Set(CSGenioAtblcfg.FldIsdefault, 0)
			.Where(CriteriaSet.And()
				.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
				.Equal(CSGenioAtblcfg.FldUuid, uuid)
				.Equal(CSGenioAtblcfg.FldIsdefault, 1));

		sp.Execute(query);
	}

	/// <summary>
	/// Retrieves a specific table configuration for a user.
	/// </summary>
	/// <param name="user">The user whose configuration should be retrieved.</param>
	/// <param name="uuid">The unique identifier of the table.</param>
	/// <param name="configName">The name of the configuration to retrieve.</param>
	/// <returns>
	/// The table configuration if found; otherwise, null.
	/// </returns>
	/// <remarks>
	/// This method performs a database search to find the configuration matching the specified
	/// user, table UUID, and configuration name combination.
	/// </remarks>
	public static TableConfiguration GetConfig(User user, string uuid, string configName)
	{
		PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

		// Get saved configuration
		CSGenioAtblcfg configRecord = CSGenioAtblcfg.searchList(
			sp,
			user,
			CriteriaSet.And()
				.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
				.Equal(CSGenioAtblcfg.FldUuid, uuid)
				.Equal(CSGenioAtblcfg.FldName, configName)
			)
			.FirstOrDefault();

		return configRecord == null
			? null
			: TableConfiguration.ParseTableConfigData(configRecord);
	}

	/// <summary>
	/// Saves a table configuration for a user, creating a new record or updating an existing one.
	/// </summary>
	/// <param name="user">The user for whom the configuration should be saved.</param>
	/// <param name="uuid">The unique identifier of the table.</param>
	/// <param name="configName">The name of the configuration.</param>
	/// <param name="isDefault">Whether this configuration should be marked as the default.</param>
	/// <param name="data">The configuration data to save.</param>
	/// <exception cref="BusinessException">Thrown when the system is in maintenance mode.</exception>
	/// <remarks>
	/// This method handles both insert and update operations based on whether the configuration
	/// already exists. If marked as default, it will clear any existing default configurations.
	/// The operation is performed within a transaction and includes cache invalidation.
	/// </remarks>
	public static void SaveConfig(User user, string uuid, string configName, int isDefault, ITableConfiguration data)
	{
		ValidateMaintenanceStatus(user, "SaveConfig");

		PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

		try
		{
			sp.openTransaction();

			// If this should be the default, we must ensure any previous default is cleared
			if (isDefault == 1)
				ClearDefaultConfig(user, sp, uuid);

			// Get saved configuration
			CSGenioAtblcfg userTableConfig = CSGenioAtblcfg.searchList(
				sp,
				user,
				CriteriaSet.And()
					.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
					.Equal(CSGenioAtblcfg.FldUuid, uuid)
					.Equal(CSGenioAtblcfg.FldName, configName)
				)
				.FirstOrDefault();

			// If record doesn't exist, create a new one
			if (userTableConfig == null)
			{
				userTableConfig = new CSGenioAtblcfg(user)
				{
					ValCodpsw = user.Codpsw,
					ValUuid = uuid,
					ValName = configName
				};
				userTableConfig.insertDirect(sp);
			}

			// Set configuration data
			userTableConfig.ValConfig = data.SerializeAsJson();
			userTableConfig.ValDate = DateTime.UtcNow;

			// Set to current version
			userTableConfig.ValUsrsetv = Configuration.UserSettingsVersion;
			if (isDefault != -1)
				userTableConfig.ValIsdefault = isDefault;
			userTableConfig.update(sp);

			sp.closeTransaction();

			// Clear cache
			UserUiSettings.Invalidate(uuid, user);
		}
		catch (Exception e)
		{
			sp.rollbackTransaction();
			sp.closeConnection();

			GenioDI.Log.Error($"Table Configuration (SaveConfig) - {e.Message}");
			throw;
		}
	}

	/// <summary>
	/// Selects a table configuration as the default for a user, or clears the default if no name is provided.
	/// </summary>
	/// <param name="user">The user for whom the configuration should be selected.</param>
	/// <param name="uuid">The unique identifier of the table.</param>
	/// <param name="configName">The name of the configuration to select as default, or null/empty to clear default.</param>
	/// <exception cref="BusinessException">
	/// Thrown when the system is in maintenance mode or when the specified configuration is not found.
	/// </exception>
	/// <remarks>
	/// When a configuration name is provided, this method marks it as the default and clears any
	/// previously marked default configurations. When configName is null or empty, it clears
	/// the default status from all configurations for the specified table and user.
	/// </remarks>
	public static void SelectConfig(User user, string uuid, string configName)
	{
		ValidateMaintenanceStatus(user, "SelectConfig");

		PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

		try
		{
			sp.openTransaction();

			// If clearing what is set as the default configuration
			if (string.IsNullOrEmpty(configName))
			{
				ClearDefaultConfig(user, sp, uuid);

				sp.closeTransaction();
				UserUiSettings.Invalidate(uuid, user);
				return;
			}

			// Get saved configuration
			CSGenioAtblcfg userTableConfig = CSGenioAtblcfg.searchList(
				sp,
				user,
				CriteriaSet.And()
					.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
					.Equal(CSGenioAtblcfg.FldUuid, uuid)
					.Equal(CSGenioAtblcfg.FldName, configName)
				)
				.FirstOrDefault();

			// If record doesn't exist, throw an exception
			if (userTableConfig == null)
				throw new BusinessException(string.Format(Translations.Get("A vista com o nome '{0}' não existe.", user.Language), configName), "SelectConfig", $"Table configuration not found: {configName}.");

			// If this record isn't the default, clear any previous default and set it as default
			if (userTableConfig.ValIsdefault == 0)
			{
				ClearDefaultConfig(user, sp, uuid);

				userTableConfig.ValIsdefault = 1;
				userTableConfig.update(sp);
			}

			sp.closeTransaction();

			// Clear cache
			UserUiSettings.Invalidate(uuid, user);
		}
		catch (Exception e)
		{
			sp.rollbackTransaction();
			sp.closeConnection();

			GenioDI.Log.Error($"Table Configuration (SelectConfig) - {e.Message}");
			throw;
		}
	}

	/// <summary>
	/// Deletes a specific table configuration for a user.
	/// </summary>
	/// <param name="user">The user whose configuration should be deleted.</param>
	/// <param name="uuid">The unique identifier of the table.</param>
	/// <param name="configName">The name of the configuration to delete.</param>
	/// <returns>
	/// True if the deleted configuration was marked as default; otherwise, false.
	/// </returns>
	/// <exception cref="BusinessException">
	/// Thrown when the system is in maintenance mode or when the specified configuration is not found.
	/// </exception>
	/// <remarks>
	/// This method permanently removes the specified configuration from the database.
	/// The return value indicates whether the deleted configuration was the default,
	/// which can be useful for UI updates or further processing.
	/// </remarks>
	public static bool DeleteConfig(User user, string uuid, string configName)
	{
		ValidateMaintenanceStatus(user, "DeleteConfig");

		PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

		try
		{
			sp.openTransaction();

			// Get saved configuration
			CSGenioAtblcfg userTableConfig = CSGenioAtblcfg.searchList(
				sp,
				user,
				CriteriaSet.And()
					.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
					.Equal(CSGenioAtblcfg.FldUuid, uuid)
					.Equal(CSGenioAtblcfg.FldName, configName)
				)
				.FirstOrDefault();

			// If record doesn't exist, throw an exception
			if (userTableConfig == null)
				throw new BusinessException(string.Format(Translations.Get("A vista com o nome '{0}' não existe.", user.Language), configName), "DeleteConfig", $"Table configuration not found: {configName}.");

			bool deletedDefaultView = userTableConfig.ValIsdefault == 1;

			userTableConfig.delete(sp);
			sp.closeTransaction();

			// Clear cache
			UserUiSettings.Invalidate(uuid, user);

			return deletedDefaultView;
		}
		catch (Exception e)
		{
			sp.rollbackTransaction();
			sp.closeConnection();

			GenioDI.Log.Error($"Table Configuration (DeleteConfig) - {e.Message}");
			throw;
		}
	}

	/// <summary>
	/// Renames an existing table configuration for a user.
	/// </summary>
	/// <param name="user">The user whose configuration should be renamed.</param>
	/// <param name="uuid">The unique identifier of the table.</param>
	/// <param name="configName">The new name for the configuration.</param>
	/// <param name="isDefault">Whether the renamed configuration should be marked as default.</param>
	/// <param name="renameFromName">The current name of the configuration to rename.</param>
	/// <exception cref="BusinessException">
	/// Thrown when the system is in maintenance mode, when the source configuration is not found,
	/// or when a configuration with the new name already exists.
	/// </exception>
	/// <remarks>
	/// This method changes the name of an existing configuration and optionally updates its
	/// default status. It validates that the source configuration exists and that the new
	/// name is not already in use by another configuration for the same user and table.
	/// </remarks>
	public static void RenameConfig(User user, string uuid, string configName, int isDefault, string renameFromName)
	{
		ValidateMaintenanceStatus(user, "RenameConfig");

		PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

		try
		{
			sp.openTransaction();

			// Get configurations
			List<CSGenioAtblcfg> userTableConfigs = CSGenioAtblcfg.searchList(
				sp,
				user,
				CriteriaSet.And()
					.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
					.Equal(CSGenioAtblcfg.FldUuid, uuid)
					.SubSet(
						CriteriaSet.Or()
							.Equal(CSGenioAtblcfg.FldName, renameFromName)
							.Equal(CSGenioAtblcfg.FldName, configName)
					)
				);

			// Check if configuration to rename exists
			CSGenioAtblcfg userTableConfigToRename = userTableConfigs.Where(config => config.ValName.Equals(renameFromName)).ToList().FirstOrDefault();

			// If record to rename doesn't exist, throw an exception
			if (userTableConfigToRename == null)
				throw new BusinessException(string.Format(Translations.Get("A vista com o nome '{0}' não existe.", user.Language), renameFromName), "RenameConfig", $"Table configuration not found: {renameFromName}.");

			// Check if saved configuration with new name already exists
			CSGenioAtblcfg userTableConfigWithNewName = userTableConfigs.Where(config => config.ValName.Equals(configName)).ToList().FirstOrDefault();

			// If record already exists, throw an exception
			if (userTableConfigWithNewName != null)
				throw new BusinessException(Translations.Get("Essa vista já existe.", user.Language), "RenameConfig", $"Table configuration already exists: {configName}.");

			// If 'ValIsdefault' is 1, all other configurations should already be marked as non-default, so no need to clear them
			if (isDefault == 1 && userTableConfigToRename.ValIsdefault == 0)
				ClearDefaultConfig(user, sp, uuid);

			if (isDefault != -1)
				userTableConfigToRename.ValIsdefault = isDefault;
			userTableConfigToRename.ValName = configName;
			userTableConfigToRename.update(sp);

			sp.closeTransaction();

			// Clear cache
			UserUiSettings.Invalidate(uuid, user);
		}
		catch (Exception e)
		{
			sp.rollbackTransaction();
			sp.closeConnection();

			GenioDI.Log.Error($"Table Configuration (RenameConfig) - {e.Message}");
			throw;
		}
	}

	/// <summary>
	/// Creates a copy of an existing table configuration with a new name for a user.
	/// </summary>
	/// <param name="user">The user for whom the configuration should be copied.</param>
	/// <param name="uuid">The unique identifier of the table.</param>
	/// <param name="configName">The name for the new copied configuration.</param>
	/// <param name="isDefault">Whether the copied configuration should be marked as default.</param>
	/// <param name="copyFromName">The name of the existing configuration to copy from (if not specified, the base configuration will be used).</param>
	/// <exception cref="BusinessException">
	/// Thrown when the system is in maintenance mode, when the source configuration is not found,
	/// or when a configuration with the new name already exists.
	/// </exception>
	/// <remarks>
	/// This method creates a new configuration record with the same data as an existing one
	/// but with a different name. The new configuration includes the current timestamp and
	/// user settings version. If marked as default, it clears any existing default configurations.
	/// </remarks>
	public static void CopyConfig(User user, string uuid, string configName, int isDefault, string copyFromName)
	{
		ValidateMaintenanceStatus(user, "CopyConfig");

		PersistentSupport sp = PersistentSupport.getPersistentSupport(user.Year, user.Name);

		try
		{
			sp.openTransaction();

			// Get configurations
			List<CSGenioAtblcfg> userTableConfigs = CSGenioAtblcfg.searchList(
				sp,
				user,
				CriteriaSet.And()
					.Equal(CSGenioAtblcfg.FldCodpsw, user.Codpsw)
					.Equal(CSGenioAtblcfg.FldUuid, uuid)
					.SubSet(
						CriteriaSet.Or()
							.Equal(CSGenioAtblcfg.FldName, copyFromName)
							.Equal(CSGenioAtblcfg.FldName, configName)
					)
				);

			// Check if configuration to copy exists
			CSGenioAtblcfg userTableConfigToCopy = userTableConfigs.Where(config => config.ValName.Equals(copyFromName)).ToList().FirstOrDefault();

			// If record to copy doesn't exist, throw an exception
			if (userTableConfigToCopy == null && !string.IsNullOrWhiteSpace(copyFromName))
				throw new BusinessException(string.Format(Translations.Get("A vista com o nome '{0}' não existe.", user.Language), copyFromName), "CopyConfig", $"Table configuration not found: {copyFromName}.");

			// Check for saved configuration
			CSGenioAtblcfg userTableConfig = userTableConfigs.Where(config => config.ValName.Equals(configName)).ToList().FirstOrDefault();

			// If a record already exists, delete it
			userTableConfig?.delete(sp);

			if (isDefault == 1)
				ClearDefaultConfig(user, sp, uuid);

			string config = userTableConfigToCopy?.ValConfig ?? new TableConfiguration().SerializeAsJson();

			// Create new record
			userTableConfig = new CSGenioAtblcfg(user)
			{
				ValCodpsw = user.Codpsw,
				ValUuid = uuid,
				ValName = configName,
				ValIsdefault = isDefault == 1 ? 1 : 0,
				ValConfig = config,
				ValDate = DateTime.UtcNow,
				ValUsrsetv = Configuration.UserSettingsVersion
			};

			// Save record
			userTableConfig.insert(sp);
			sp.closeTransaction();

			// Clear cache
			UserUiSettings.Invalidate(uuid, user);
		}
		catch (Exception e)
		{
			sp.rollbackTransaction();
			sp.closeConnection();

			GenioDI.Log.Error($"Table Configuration (CopyConfig) - {e.Message}");
			throw;
		}
	}
}
