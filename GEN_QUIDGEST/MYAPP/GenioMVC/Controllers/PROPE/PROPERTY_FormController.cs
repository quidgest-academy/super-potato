using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Dynamic;

using CSGenio.business;
using CSGenio.core.persistence;
using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.reporting;
using GenioMVC.Helpers;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioMVC.Resources;
using GenioMVC.ViewModels;
using GenioMVC.ViewModels.Prope;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR INCLUDE_CONTROLLER PROPE]/

namespace GenioMVC.Controllers
{
	public partial class PropeController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_PROPERTY_CANCEL = new("PROPERTY43977", "Property_Cancel", "Prope") { vueRouteName = "form-PROPERTY", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_PROPERTY_SHOW = new("PROPERTY43977", "Property_Show", "Prope") { vueRouteName = "form-PROPERTY", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_PROPERTY_NEW = new("PROPERTY43977", "Property_New", "Prope") { vueRouteName = "form-PROPERTY", mode = "NEW" };
		private static readonly NavigationLocation ACTION_PROPERTY_EDIT = new("PROPERTY43977", "Property_Edit", "Prope") { vueRouteName = "form-PROPERTY", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_PROPERTY_DUPLICATE = new("PROPERTY43977", "Property_Duplicate", "Prope") { vueRouteName = "form-PROPERTY", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_PROPERTY_DELETE = new("PROPERTY43977", "Property_Delete", "Prope") { vueRouteName = "form-PROPERTY", mode = "DELETE" };

		#endregion

		#region Property private

		private void FormHistoryLimits_Property()
		{

		}

		#endregion

		#region Property_Show

// USE /[MANUAL FOR CONTROLLER_SHOW PROPERTY]/

		[HttpPost]
		public ActionResult Property_Show_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Property_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Property_Show_GET",
				AreaName = "prope",
				Location = ACTION_PROPERTY_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Property();
// USE /[MANUAL FOR BEFORE_LOAD_SHOW PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_SHOW PROPERTY]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Property_New

// USE /[MANUAL FOR CONTROLLER_NEW_GET PROPERTY]/
		[HttpPost]
		public ActionResult Property_New_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			Property_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Property_New_GET",
				AreaName = "prope",
				FormName = "PROPERTY",
				Location = ACTION_PROPERTY_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Property();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW PROPERTY]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Prope/Property_New
// USE /[MANUAL FOR CONTROLLER_NEW_POST PROPERTY]/
		[HttpPost]
		public ActionResult Property_New([FromBody]Property_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Property_New",
				ViewName = "Property",
				AreaName = "prope",
				Location = ACTION_PROPERTY_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_NEW PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_NEW PROPERTY]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW_EX PROPERTY]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW_EX PROPERTY]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Property_Edit

// USE /[MANUAL FOR CONTROLLER_EDIT_GET PROPERTY]/
		[HttpPost]
		public ActionResult Property_Edit_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Property_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Property_Edit_GET",
				AreaName = "prope",
				FormName = "PROPERTY",
				Location = ACTION_PROPERTY_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Property();
// USE /[MANUAL FOR BEFORE_LOAD_EDIT PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT PROPERTY]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Prope/Property_Edit
// USE /[MANUAL FOR CONTROLLER_EDIT_POST PROPERTY]/
		[HttpPost]
		public ActionResult Property_Edit([FromBody]Property_ViewModel model, [FromQuery]bool redirect)
		{
			EventSink eventSink = new()
			{
				MethodName = "Property_Edit",
				ViewName = "Property",
				AreaName = "prope",
				Location = ACTION_PROPERTY_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_EDIT PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_EDIT PROPERTY]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_EDIT_EX PROPERTY]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT_EX PROPERTY]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Property_Delete

// USE /[MANUAL FOR CONTROLLER_DELETE_GET PROPERTY]/
		[HttpPost]
		public ActionResult Property_Delete_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Property_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Property_Delete_GET",
				AreaName = "prope",
				FormName = "PROPERTY",
				Location = ACTION_PROPERTY_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Property();
// USE /[MANUAL FOR BEFORE_LOAD_DELETE PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DELETE PROPERTY]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Prope/Property_Delete
// USE /[MANUAL FOR CONTROLLER_DELETE_POST PROPERTY]/
		[HttpPost]
		public ActionResult Property_Delete([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Property_ViewModel model = new(UserContext.Current, id);
			model.MapFromModel();

			EventSink eventSink = new()
			{
				MethodName = "Property_Delete",
				ViewName = "Property",
				AreaName = "prope",
				Location = ACTION_PROPERTY_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_DESTROY_DELETE PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_DESTROY_DELETE PROPERTY]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Property_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("PROPERTY");
		}

		#endregion

		#region Property_Duplicate

// USE /[MANUAL FOR CONTROLLER_DUPLICATE_GET PROPERTY]/

		[HttpPost]
		public ActionResult Property_Duplicate_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;

			Property_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Property_Duplicate_GET",
				AreaName = "prope",
				FormName = "PROPERTY",
				Location = ACTION_PROPERTY_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE PROPERTY]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Prope/Property_Duplicate
// USE /[MANUAL FOR CONTROLLER_DUPLICATE_POST PROPERTY]/
		[HttpPost]
		public ActionResult Property_Duplicate([FromBody]Property_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Property_Duplicate",
				ViewName = "Property",
				AreaName = "prope",
				Location = ACTION_PROPERTY_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_DUPLICATE PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_DUPLICATE PROPERTY]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE_EX PROPERTY]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE_EX PROPERTY]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Property_Cancel

		//
		// GET: /Prope/Property_Cancel
// USE /[MANUAL FOR CONTROLLER_CANCEL_GET PROPERTY]/
		public ActionResult Property_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					GenioMVC.Models.Prope model = new(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("prope");

// USE /[MANUAL FOR BEFORE_CANCEL PROPERTY]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL FOR AFTER_CANCEL PROPERTY]/

				}
				catch (Exception e)
				{
					sp.rollbackTransaction();
					sp.closeConnection();

					var exceptionUserMessage = Resources.Resources.PEDIMOS_DESCULPA__OC63848;
					if (e is GenioException && (e as GenioException).UserMessage != null)
						exceptionUserMessage = Translations.Get((e as GenioException).UserMessage, UserContext.Current.User.Language);
					return JsonERROR(exceptionUserMessage);
				}

				Navigation.SetValue("ForcePrimaryRead_prope", "true", true);
			}

			Navigation.ClearValue("prope");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		public class Property_CityValCityModel : RequestLookupModel
		{
			public Property_ViewModel Model { get; set; }
		}

		//
		// GET: /Prope/Property_CityValCity
		// POST: /Prope/Property_CityValCity
		[ActionName("Property_CityValCity")]
		public ActionResult Property_CityValCity([FromBody] Property_CityValCityModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_city")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_city");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			NameValueCollection requestValues = [];
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			IsStateReadonly = true;

			Models.Prope parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Property_CityValCity_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(requestModel.TableConfiguration);

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		public class Property_AgentValNameModel : RequestLookupModel
		{
			public Property_ViewModel Model { get; set; }
		}

		//
		// GET: /Prope/Property_AgentValName
		// POST: /Prope/Property_AgentValName
		[ActionName("Property_AgentValName")]
		public ActionResult Property_AgentValName([FromBody] Property_AgentValNameModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_agent")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_agent");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			NameValueCollection requestValues = [];
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			IsStateReadonly = true;

			Models.Prope parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Property_AgentValName_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(requestModel.TableConfiguration);

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		public class Property_ValField001Model : RequestLookupModel
		{
			public Property_ViewModel Model { get; set; }
		}

		//
		// GET: /Prope/Property_ValField001
		// POST: /Prope/Property_ValField001
		[ActionName("Property_ValField001")]
		public ActionResult Property_ValField001([FromBody] Property_ValField001Model requestModel)
		{
			var queryParams = requestModel.QueryParams;

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_photo")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_photo");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			NameValueCollection requestValues = [];
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			Models.Prope parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Property_ValField001_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(
				requestModel.TableConfiguration,
				requestModel.UserTableConfigName,
				requestModel.LoadDefaultView);

			// Determine rows per page
			tableConfig.RowsPerPage = tableConfig.DetermineRowsPerPage(4, "");

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		public class Property_ValField002Model : RequestLookupModel
		{
			public Property_ViewModel Model { get; set; }
		}

		//
		// GET: /Prope/Property_ValField002
		// POST: /Prope/Property_ValField002
		[ActionName("Property_ValField002")]
		public ActionResult Property_ValField002([FromBody] Property_ValField002Model requestModel)
		{
			var queryParams = requestModel.QueryParams;

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_conta")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_conta");
				UserContext.Current.SetPersistenceReadOnly(false);
			}

			NameValueCollection requestValues = [];
			if (queryParams != null)
			{
				// Add to request values
				foreach (var kv in queryParams)
					requestValues.Add(kv.Key, kv.Value);
			}

			Models.Prope parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Property_ValField002_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(
				requestModel.TableConfiguration,
				requestModel.UserTableConfigName,
				requestModel.LoadDefaultView);

			// Determine rows per page
			tableConfig.RowsPerPage = tableConfig.DetermineRowsPerPage(4, "");

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		// POST: /Prope/Property_SaveEdit
		[HttpPost]
		public ActionResult Property_SaveEdit([FromBody] Property_ViewModel model)
		{
			EventSink eventSink = new()
			{
				MethodName = "Property_SaveEdit",
				ViewName = "Property",
				AreaName = "prope",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_APPLY_EDIT PROPERTY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_APPLY_EDIT PROPERTY]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}

		public class PropertyDocumValidateTickets : RequestDocumValidateTickets
		{
			public Property_ViewModel Model { get; set; }
		}

		/// <summary>
		/// Checks if the model is valid and, if so, updates the specified tickets with write permissions
		/// </summary>
		/// <param name="requestModel">The request model with a list of tickets and the form model</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult UpdateFilesTicketsProperty([FromBody] PropertyDocumValidateTickets requestModel)
		{
			requestModel.Model.Init(UserContext.Current);
			return UpdateFilesTickets(requestModel.Tickets, requestModel.Model, requestModel.IsApply);
		}
	}
}
