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
using GenioMVC.ViewModels.Agent;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR INCLUDE_CONTROLLER AGENT]/

namespace GenioMVC.Controllers
{
	public partial class AgentController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_AGENT_CANCEL = new("AGENT00994", "Agent_Cancel", "Agent") { vueRouteName = "form-AGENT", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_AGENT_SHOW = new("AGENT00994", "Agent_Show", "Agent") { vueRouteName = "form-AGENT", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_AGENT_NEW = new("AGENT00994", "Agent_New", "Agent") { vueRouteName = "form-AGENT", mode = "NEW" };
		private static readonly NavigationLocation ACTION_AGENT_EDIT = new("AGENT00994", "Agent_Edit", "Agent") { vueRouteName = "form-AGENT", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_AGENT_DUPLICATE = new("AGENT00994", "Agent_Duplicate", "Agent") { vueRouteName = "form-AGENT", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_AGENT_DELETE = new("AGENT00994", "Agent_Delete", "Agent") { vueRouteName = "form-AGENT", mode = "DELETE" };

		#endregion

		#region Agent private

		private void FormHistoryLimits_Agent()
		{

		}

		#endregion

		#region Agent_Show

// USE /[MANUAL FOR CONTROLLER_SHOW AGENT]/

		[HttpPost]
		public ActionResult Agent_Show_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Agent_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Agent_Show_GET",
				AreaName = "agent",
				Location = ACTION_AGENT_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Agent();
// USE /[MANUAL FOR BEFORE_LOAD_SHOW AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_SHOW AGENT]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Agent_New

// USE /[MANUAL FOR CONTROLLER_NEW_GET AGENT]/
		[HttpPost]
		public ActionResult Agent_New_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			Agent_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Agent_New_GET",
				AreaName = "agent",
				FormName = "AGENT",
				Location = ACTION_AGENT_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Agent();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW AGENT]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Agent/Agent_New
// USE /[MANUAL FOR CONTROLLER_NEW_POST AGENT]/
		[HttpPost]
		public ActionResult Agent_New([FromBody]Agent_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Agent_New",
				ViewName = "Agent",
				AreaName = "agent",
				Location = ACTION_AGENT_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_NEW AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_NEW AGENT]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW_EX AGENT]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW_EX AGENT]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Agent_Edit

// USE /[MANUAL FOR CONTROLLER_EDIT_GET AGENT]/
		[HttpPost]
		public ActionResult Agent_Edit_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Agent_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Agent_Edit_GET",
				AreaName = "agent",
				FormName = "AGENT",
				Location = ACTION_AGENT_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Agent();
// USE /[MANUAL FOR BEFORE_LOAD_EDIT AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT AGENT]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Agent/Agent_Edit
// USE /[MANUAL FOR CONTROLLER_EDIT_POST AGENT]/
		[HttpPost]
		public ActionResult Agent_Edit([FromBody]Agent_ViewModel model, [FromQuery]bool redirect)
		{
			EventSink eventSink = new()
			{
				MethodName = "Agent_Edit",
				ViewName = "Agent",
				AreaName = "agent",
				Location = ACTION_AGENT_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_EDIT AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_EDIT AGENT]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_EDIT_EX AGENT]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT_EX AGENT]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Agent_Delete

// USE /[MANUAL FOR CONTROLLER_DELETE_GET AGENT]/
		[HttpPost]
		public ActionResult Agent_Delete_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Agent_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Agent_Delete_GET",
				AreaName = "agent",
				FormName = "AGENT",
				Location = ACTION_AGENT_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Agent();
// USE /[MANUAL FOR BEFORE_LOAD_DELETE AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DELETE AGENT]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Agent/Agent_Delete
// USE /[MANUAL FOR CONTROLLER_DELETE_POST AGENT]/
		[HttpPost]
		public ActionResult Agent_Delete([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Agent_ViewModel model = new(UserContext.Current, id);
			model.MapFromModel();

			EventSink eventSink = new()
			{
				MethodName = "Agent_Delete",
				ViewName = "Agent",
				AreaName = "agent",
				Location = ACTION_AGENT_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_DESTROY_DELETE AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_DESTROY_DELETE AGENT]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Agent_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("AGENT");
		}

		#endregion

		#region Agent_Duplicate

// USE /[MANUAL FOR CONTROLLER_DUPLICATE_GET AGENT]/

		[HttpPost]
		public ActionResult Agent_Duplicate_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;

			Agent_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Agent_Duplicate_GET",
				AreaName = "agent",
				FormName = "AGENT",
				Location = ACTION_AGENT_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE AGENT]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Agent/Agent_Duplicate
// USE /[MANUAL FOR CONTROLLER_DUPLICATE_POST AGENT]/
		[HttpPost]
		public ActionResult Agent_Duplicate([FromBody]Agent_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Agent_Duplicate",
				ViewName = "Agent",
				AreaName = "agent",
				Location = ACTION_AGENT_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_DUPLICATE AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_DUPLICATE AGENT]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE_EX AGENT]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE_EX AGENT]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Agent_Cancel

		//
		// GET: /Agent/Agent_Cancel
// USE /[MANUAL FOR CONTROLLER_CANCEL_GET AGENT]/
		public ActionResult Agent_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					GenioMVC.Models.Agent model = new(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("agent");

// USE /[MANUAL FOR BEFORE_CANCEL AGENT]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL FOR AFTER_CANCEL AGENT]/

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

				Navigation.SetValue("ForcePrimaryRead_agent", "true", true);
			}

			Navigation.ClearValue("agent");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		public class Agent_CbornValCountryModel : RequestLookupModel
		{
			public Agent_ViewModel Model { get; set; }
		}

		//
		// GET: /Agent/Agent_CbornValCountry
		// POST: /Agent/Agent_CbornValCountry
		[ActionName("Agent_CbornValCountry")]
		public ActionResult Agent_CbornValCountry([FromBody] Agent_CbornValCountryModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_cborn")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_cborn");
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

			Models.Agent parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Agent_CbornValCountry_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(requestModel.TableConfiguration);

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		public class Agent_CaddrValCountryModel : RequestLookupModel
		{
			public Agent_ViewModel Model { get; set; }
		}

		//
		// GET: /Agent/Agent_CaddrValCountry
		// POST: /Agent/Agent_CaddrValCountry
		[ActionName("Agent_CaddrValCountry")]
		public ActionResult Agent_CaddrValCountry([FromBody] Agent_CaddrValCountryModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_caddr")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_caddr");
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

			Models.Agent parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Agent_CaddrValCountry_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(requestModel.TableConfiguration);

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		// POST: /Agent/Agent_SaveEdit
		[HttpPost]
		public ActionResult Agent_SaveEdit([FromBody] Agent_ViewModel model)
		{
			EventSink eventSink = new()
			{
				MethodName = "Agent_SaveEdit",
				ViewName = "Agent",
				AreaName = "agent",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_APPLY_EDIT AGENT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_APPLY_EDIT AGENT]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}

		public class AgentDocumValidateTickets : RequestDocumValidateTickets
		{
			public Agent_ViewModel Model { get; set; }
		}

		/// <summary>
		/// Checks if the model is valid and, if so, updates the specified tickets with write permissions
		/// </summary>
		/// <param name="requestModel">The request model with a list of tickets and the form model</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult UpdateFilesTicketsAgent([FromBody] AgentDocumValidateTickets requestModel)
		{
			requestModel.Model.Init(UserContext.Current);
			return UpdateFilesTickets(requestModel.Tickets, requestModel.Model, requestModel.IsApply);
		}
	}
}
