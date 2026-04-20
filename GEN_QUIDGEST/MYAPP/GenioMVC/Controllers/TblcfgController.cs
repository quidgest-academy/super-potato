using CSGenio.business;
using CSGenio.core.framework.table;
using CSGenio.framework;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers.Tblcfg;

/// <summary>
/// Web API controller for managing table configuration operations.
/// Provides endpoints for saving, loading, selecting, deleting, renaming, and copying
/// user-specific table view configurations through HTTP POST requests.
/// </summary>
/// <remarks>
/// This controller acts as a web API layer over the TableConfigurationManager,
/// handling HTTP requests and responses while delegating business logic to the manager.
/// All endpoints return JSON responses and include proper error handling.
/// </remarks>
/// <param name="userContextService">Service providing user context information for the current session.</param>
public class TblcfgController(UserContextService userContextService) : ControllerBase(userContextService)
{
	#region Models

	/// <summary>
	/// Base request model containing the fundamental identifiers needed for table configuration operations.
	/// </summary>
	public class RequestConfigModel
	{
		/// <summary>
		/// Gets or sets the unique identifier of the table whose configuration is being managed.
		/// </summary>
		public string Uuid { get; set; }

		/// <summary>
		/// Gets or sets the name of the table configuration.
		/// </summary>
		public string ConfigName { get; set; }
	}

	/// <summary>
	/// Request model that extends the base model with selection status for configuration operations.
	/// </summary>
	public class RequestConfigSelectedModel : RequestConfigModel
	{
		/// <summary>
		/// Gets or sets whether this configuration should be marked as default.
		/// Values: 1 = set as default, 0 = not default, -1 = no change to default status.
		/// </summary>
		public int IsSelected { get; set; } = -1;
	}

	/// <summary>
	/// Request model for saving table configuration data.
	/// Contains all the information needed to persist a table configuration.
	/// </summary>
	public class RequestConfigSaveModel : RequestConfigSelectedModel
	{
		/// <summary>
		/// Gets or sets the serialized configuration data to be saved.
		/// This typically contains JSON-serialized table settings including column visibility, filters, etc.
		/// </summary>
		public TableConfiguration Data { get; set; }
	}

	/// <summary>
	/// Request model for renaming an existing table configuration.
	/// Contains both the new name and the current name of the configuration to rename.
	/// </summary>
	public class RequestConfigRenameModel : RequestConfigSelectedModel
	{
		/// <summary>
		/// Gets or sets the current name of the configuration that should be renamed.
		/// This is used to identify the existing configuration in the database.
		/// </summary>
		public string RenameFromName { get; set; }
	}

	/// <summary>
	/// Request model for copying an existing table configuration to create a new one.
	/// Contains both the new configuration name and the source configuration to copy from.
	/// </summary>
	public class RequestConfigCopyModel : RequestConfigSelectedModel
	{
		/// <summary>
		/// Gets or sets the name of the existing configuration to copy from.
		/// This configuration will serve as a template for creating the new configuration.
		/// </summary>
		public string CopyFromName { get; set; }
	}

	/// <summary>
	/// Represents a single configuration entry in a batch configuration operation.
	/// Used to manage multiple configurations in a single request (create, update, delete, copy, rename).
	/// </summary>
	public class RequestConfigEntryModel
	{
		/// <summary>
		/// Gets or sets the new or current name of the configuration.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the original name of the configuration (if being renamed).
		/// </summary>
		/// <remarks>
		/// When populated, indicates this configuration already exists and may be renamed or have its default status updated.
		/// When empty, combined with <see cref="BasedOn"/>, indicates a new configuration being created via copy.
		/// </remarks>
		public string OldName { get; set; }

		/// <summary>
		/// Gets or sets whether this configuration should be marked as default.
		/// Values: 1 = set as default, 0 = not default, -1 = no change to default status.
		/// </summary>
		public int IsSelected { get; set; } = -1;

		/// <summary>
		/// Gets or sets the name of the source configuration to copy from.
		/// </summary>
		/// <remarks>
		/// This configuration entry represents a copy operation where a new configuration with the name
		/// specified in <see cref="Name"/> will be created by copying this source configuration.
		/// </remarks>
		public string BasedOn { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating whether this configuration should be deleted.
		/// </summary>
		/// <remarks>
		/// Deletions are processed after all other operations to ensure that configurations
		/// are not deleted while they are being referenced by other operations in the same batch.
		/// </remarks>
		public bool Deleted { get; set; }
	}

	/// <summary>
	/// Request model for batch saving of multiple table configurations.
	/// Allows create, update, rename, copy, and delete operations in a single request.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This model enables efficient batch operations on multiple configurations. The processing order is:
	/// <list type="number">
	/// <item><description>Rename existing configurations (where Name differs from OldName)</description></item>
	/// <item><description>Update default status for existing configurations</description></item>
	/// <item><description>Copy configurations to create new ones</description></item>
	/// <item><description>Delete configurations marked for deletion</description></item>
	/// </list>
	/// </para>
	/// <para>
	/// This order ensures that configurations are not accidentally deleted before being referenced as
	/// sources for copy operations or before their new names are used.
	/// </para>
	/// </remarks>
	public class RequestSaveConfigListModel
	{
		/// <summary>
		/// Gets or sets the unique identifier of the table whose configurations are being managed.
		/// All configuration entries in <see cref="ConfigList"/> apply to this table.
		/// </summary>
		public string Uuid { get; set; }

		/// <summary>
		/// Gets or sets the list of configuration entries to process.
		/// Each entry represents a configuration to be created, updated, renamed, copied, or deleted.
		/// </summary>
		public List<RequestConfigEntryModel> ConfigList { get; set; } = [];
	}

	#endregion

	/// <summary>
	/// Handles exceptions by extracting appropriate error messages and returning JSON error responses.
	/// </summary>
	/// <param name="e">The exception that occurred during the operation.</param>
	/// <param name="data">Additional data about the error.</param>
	/// <returns>A JSON error response containing the user-friendly error message.</returns>
	/// <remarks>
	/// Business exceptions are handled specially to extract user-friendly messages,
	/// while other exceptions use their standard message property.
	/// </remarks>
	private JsonResult HandleException(Exception e, object data = null)
	{
		string message = e is BusinessException
			? (e as BusinessException).UserMessage
			: e.Message;
		return JsonERROR(message, data);
	}

	/// <summary>
	/// Retrieves a specific table configuration for the current user.
	/// </summary>
	/// <param name="requestModel">The request model containing the table UUID and configuration name.</param>
	/// <returns>
	/// A JSON response containing the configuration data and name if found,
	/// or an error response if the configuration doesn't exist.
	/// </returns>
	/// <remarks>
	/// This endpoint fetches saved table configurations from the database.
	/// If the specified configuration is not found, it returns a localized error message.
	/// </remarks>
	[HttpPost]
	public ActionResult GetConfig([FromBody] RequestConfigModel requestModel)
	{
		User user = m_userContext.User;
		string uuid = requestModel.Uuid;
		string configName = requestModel.ConfigName;

		// Get saved configuration
		TableConfiguration config = TableConfigurationManager.GetConfig(user, uuid, configName);

		return config == null
			? JsonERROR(string.Format(Translations.Get("A vista com o nome '{0}' não existe.", user.Language), configName))
			: JsonOK(new { config });
	}

	/// <summary>
	/// Saves a table configuration for the current user.
	/// </summary>
	/// <param name="requestModel">The request model containing all configuration data to save.</param>
	/// <returns>
	/// A JSON success response if the configuration was saved successfully,
	/// or a JSON error response if the operation failed.
	/// </returns>
	/// <remarks>
	/// This endpoint can create new configurations or update existing ones.
	/// If the configuration is marked as default, any previous default configurations
	/// for the same table will be automatically cleared.
	/// The system must not be in maintenance mode for this operation to succeed.
	/// </remarks>
	[HttpPost]
	public ActionResult SaveConfig([FromBody] RequestConfigSaveModel requestModel)
	{
		try
		{
			TableConfigurationManager.SaveConfig(
				m_userContext.User,
				requestModel.Uuid,
				requestModel.ConfigName,
				requestModel.IsSelected,
				requestModel.Data);
			return JsonOK();
		}
		catch (Exception e)
		{
			return HandleException(e);
		}
	}

	/// <summary>
	/// Saves or updates multiple table configurations in a single batch operation.
	/// Efficiently handles create, rename, copy, delete, and default-view flag changes.
	/// </summary>
	/// <param name="requestModel">The request model containing the table UUID and list of configuration entries to process.</param>
	/// <returns>
	/// A JSON success response if all operations completed successfully,
	/// or a JSON error response if any operation failed.
	/// </returns>
	/// <remarks>
	/// This endpoint is optimized for scenarios where multiple configuration changes need to be applied together,
	/// reducing the number of round trips to the server.
	/// <para>
	/// Processing sequence (to avoid conflicts):
	/// <list type="number">
	/// <item><description>Rename operations: Updates configuration names where <see cref="RequestConfigEntryModel.Name"/> != <see cref="RequestConfigEntryModel.OldName"/></description></item>
	/// <item><description>Default selection updates: For renamed configs where only default status changed</description></item>
	/// <item><description>Copy operations: Creates new configurations based on <see cref="RequestConfigEntryModel.BasedOn"/></description></item>
	/// <item><description>Deletion operations: Deletes all configurations marked with <see cref="RequestConfigEntryModel.Deleted"/> = true</description></item>
	/// </list>
	/// </para>
	/// </remarks>
	[HttpPost]
	public ActionResult SaveConfigList([FromBody] RequestSaveConfigListModel requestModel)
	{
		List<string> errors = [];
		User user = m_userContext.User;

		try
		{
			// Before starting, clear the default view (necessary when the default is set to the base table)
			TableConfigurationManager.SelectConfig(user, requestModel.Uuid, "");
		}
		catch (Exception e)
		{
			string message = e is BusinessException
				? (e as BusinessException).UserMessage
				: e.Message;
			errors.Add(message);
		}

		// Start by processing the creation of new configurations
		foreach (RequestConfigEntryModel config in requestModel.ConfigList)
		{
			try
			{
				// Configurations without an OldName are newly created ones
				if (!string.IsNullOrWhiteSpace(config.Name) && string.IsNullOrWhiteSpace(config.OldName))
					TableConfigurationManager.CopyConfig(user, requestModel.Uuid, config.Name, config.IsSelected, config.BasedOn);
			}
			catch (Exception e)
			{
				string message = e is BusinessException
					? (e as BusinessException).UserMessage
					: e.Message;
				errors.Add(message);
			}
		}

		// Process rename and update operations for existing configurations
		foreach (RequestConfigEntryModel config in requestModel.ConfigList)
		{
			// Skip entries with empty names (there should never be any though)
			if (string.IsNullOrWhiteSpace(config.Name))
				continue;

			try
			{
				// Handle existing configurations (those with an OldName)
				if (!string.IsNullOrWhiteSpace(config.OldName))
				{
					// If names differ, rename the configuration and set the default status in the same operation
					if (config.Name != config.OldName)
						TableConfigurationManager.RenameConfig(user, requestModel.Uuid, config.Name, config.IsSelected, config.OldName);
					// If names are the same but IsSelected is 1, update just the default status
					else if (config.IsSelected == 1)
						TableConfigurationManager.SelectConfig(user, requestModel.Uuid, config.Name);
				}
			}
			catch (Exception e)
			{
				string message = e is BusinessException
					? (e as BusinessException).UserMessage
					: e.Message;
				errors.Add(message);
			}
		}

		// Process deletions only after all other operations, to avoid conflicts with them
		foreach (RequestConfigEntryModel config in requestModel.ConfigList)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(config.Name) && config.Deleted)
					TableConfigurationManager.DeleteConfig(user, requestModel.Uuid, config.Name);
			}
			catch (Exception e)
			{
				string message = e is BusinessException
					? (e as BusinessException).UserMessage
					: e.Message;
				errors.Add(message);
			}
		}

		return errors.Count == 0
			? JsonOK()
			: errors.Count == 1
				? JsonERROR(errors[0])
				: JsonERROR(Translations.Get("Ocorreram alguns erros a gravar as vistas:", user.Language), errors);
	}

	/// <summary>
	/// Selects a table configuration as the default for the current user,
	/// or clears the default selection if no configuration name is provided.
	/// </summary>
	/// <param name="requestModel">The request model containing the table UUID and configuration name to select.</param>
	/// <returns>
	/// A JSON success response if the selection was successful,
	/// or a JSON error response if the operation failed.
	/// </returns>
	/// <remarks>
	/// When a configuration name is provided, it becomes the new default and any previously
	/// selected default is cleared. When the configuration name is null or empty,
	/// all default selections for the table are cleared.
	/// The system must not be in maintenance mode for this operation to succeed.
	/// </remarks>
	[HttpPost]
	public ActionResult SelectConfig([FromBody] RequestConfigModel requestModel)
	{
		try
		{
			TableConfigurationManager.SelectConfig(
				m_userContext.User,
				requestModel.Uuid,
				requestModel.ConfigName);
			return JsonOK();
		}
		catch (Exception e)
		{
			return HandleException(e);
		}
	}

	/// <summary>
	/// Deletes a specific table configuration for the current user.
	/// </summary>
	/// <param name="requestModel">The request model containing the table UUID and configuration name to delete.</param>
	/// <returns>
	/// A JSON success response if the deletion was successful,
	/// or a JSON error response if the operation failed.
	/// </returns>
	/// <remarks>
	/// This endpoint permanently removes the specified configuration from the database.
	/// The system must not be in maintenance mode for this operation to succeed.
	/// </remarks>
	[HttpPost]
	public ActionResult DeleteConfig([FromBody] RequestConfigModel requestModel)
	{
		try
		{
			TableConfigurationManager.DeleteConfig(
				m_userContext.User,
				requestModel.Uuid,
				requestModel.ConfigName);
			return JsonOK();
		}
		catch (Exception e)
		{
			return HandleException(e);
		}
	}

	/// <summary>
	/// Renames an existing table configuration for the current user.
	/// </summary>
	/// <param name="requestModel">The request model containing the new name and the current name of the configuration to rename.</param>
	/// <returns>
	/// A JSON success response if the rename was successful,
	/// or a JSON error response if the operation failed.
	/// </returns>
	/// <remarks>
	/// This endpoint changes the name of an existing configuration and optionally updates
	/// its default status. It validates that the source configuration exists and that
	/// the new name is not already in use.
	/// The system must not be in maintenance mode for this operation to succeed.
	/// </remarks>
	[HttpPost]
	public ActionResult RenameConfig([FromBody] RequestConfigRenameModel requestModel)
	{
		try
		{
			TableConfigurationManager.RenameConfig(
				m_userContext.User,
				requestModel.Uuid,
				requestModel.ConfigName,
				requestModel.IsSelected,
				requestModel.RenameFromName);
			return JsonOK();
		}
		catch (Exception e)
		{
			return HandleException(e);
		}
	}

	/// <summary>
	/// Creates a copy of an existing table configuration with a new name for the current user.
	/// </summary>
	/// <param name="requestModel">The request model containing the new configuration name and the source configuration to copy from.</param>
	/// <returns>
	/// A JSON success response if the copy was successful,
	/// or a JSON error response if the operation failed.
	/// </returns>
	/// <remarks>
	/// This endpoint duplicates an existing configuration with all its settings and data,
	/// creating a new configuration record with a different name. The new configuration
	/// can optionally be marked as default.
	/// It validates that the source configuration exists and that the new name is available.
	/// The system must not be in maintenance mode for this operation to succeed.
	/// </remarks>
	[HttpPost]
	public ActionResult CopyConfig([FromBody] RequestConfigCopyModel requestModel)
	{
		try
		{
			TableConfigurationManager.CopyConfig(
				m_userContext.User,
				requestModel.Uuid,
				requestModel.ConfigName,
				requestModel.IsSelected,
				requestModel.CopyFromName);
			return JsonOK();
		}
		catch (Exception e)
		{
			return HandleException(e);
		}
	}
}
