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
using GenioMVC.ViewModels.Conta;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR INCLUDE_CONTROLLER CONTA]/

namespace GenioMVC.Controllers
{
	public partial class ContaController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_CONTACT_CANCEL = new("CONTACT59247", "Contact_Cancel", "Conta") { vueRouteName = "form-CONTACT", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_CONTACT_SHOW = new("CONTACT59247", "Contact_Show", "Conta") { vueRouteName = "form-CONTACT", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_CONTACT_NEW = new("CONTACT59247", "Contact_New", "Conta") { vueRouteName = "form-CONTACT", mode = "NEW" };
		private static readonly NavigationLocation ACTION_CONTACT_EDIT = new("CONTACT59247", "Contact_Edit", "Conta") { vueRouteName = "form-CONTACT", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_CONTACT_DUPLICATE = new("CONTACT59247", "Contact_Duplicate", "Conta") { vueRouteName = "form-CONTACT", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_CONTACT_DELETE = new("CONTACT59247", "Contact_Delete", "Conta") { vueRouteName = "form-CONTACT", mode = "DELETE" };

		#endregion

		#region Contact private

		private void FormHistoryLimits_Contact()
		{

		}

		#endregion

		#region Contact_Show

// USE /[MANUAL FOR CONTROLLER_SHOW CONTACT]/

		[HttpPost]
		public ActionResult Contact_Show_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Contact_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Contact_Show_GET",
				AreaName = "conta",
				Location = ACTION_CONTACT_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Contact();
// USE /[MANUAL FOR BEFORE_LOAD_SHOW CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_SHOW CONTACT]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Contact_New

// USE /[MANUAL FOR CONTROLLER_NEW_GET CONTACT]/
		[HttpPost]
		public ActionResult Contact_New_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			Contact_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Contact_New_GET",
				AreaName = "conta",
				FormName = "CONTACT",
				Location = ACTION_CONTACT_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Contact();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW CONTACT]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Conta/Contact_New
// USE /[MANUAL FOR CONTROLLER_NEW_POST CONTACT]/
		[HttpPost]
		public ActionResult Contact_New([FromBody]Contact_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Contact_New",
				ViewName = "Contact",
				AreaName = "conta",
				Location = ACTION_CONTACT_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_NEW CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_NEW CONTACT]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW_EX CONTACT]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW_EX CONTACT]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Contact_Edit

// USE /[MANUAL FOR CONTROLLER_EDIT_GET CONTACT]/
		[HttpPost]
		public ActionResult Contact_Edit_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Contact_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Contact_Edit_GET",
				AreaName = "conta",
				FormName = "CONTACT",
				Location = ACTION_CONTACT_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Contact();
// USE /[MANUAL FOR BEFORE_LOAD_EDIT CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT CONTACT]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Conta/Contact_Edit
// USE /[MANUAL FOR CONTROLLER_EDIT_POST CONTACT]/
		[HttpPost]
		public ActionResult Contact_Edit([FromBody]Contact_ViewModel model, [FromQuery]bool redirect)
		{
			EventSink eventSink = new()
			{
				MethodName = "Contact_Edit",
				ViewName = "Contact",
				AreaName = "conta",
				Location = ACTION_CONTACT_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_EDIT CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_EDIT CONTACT]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_EDIT_EX CONTACT]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT_EX CONTACT]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Contact_Delete

// USE /[MANUAL FOR CONTROLLER_DELETE_GET CONTACT]/
		[HttpPost]
		public ActionResult Contact_Delete_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Contact_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Contact_Delete_GET",
				AreaName = "conta",
				FormName = "CONTACT",
				Location = ACTION_CONTACT_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Contact();
// USE /[MANUAL FOR BEFORE_LOAD_DELETE CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DELETE CONTACT]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Conta/Contact_Delete
// USE /[MANUAL FOR CONTROLLER_DELETE_POST CONTACT]/
		[HttpPost]
		public ActionResult Contact_Delete([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Contact_ViewModel model = new(UserContext.Current, id);
			model.MapFromModel();

			EventSink eventSink = new()
			{
				MethodName = "Contact_Delete",
				ViewName = "Contact",
				AreaName = "conta",
				Location = ACTION_CONTACT_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_DESTROY_DELETE CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_DESTROY_DELETE CONTACT]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Contact_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("CONTACT");
		}

		#endregion

		#region Contact_Duplicate

// USE /[MANUAL FOR CONTROLLER_DUPLICATE_GET CONTACT]/

		[HttpPost]
		public ActionResult Contact_Duplicate_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;

			Contact_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Contact_Duplicate_GET",
				AreaName = "conta",
				FormName = "CONTACT",
				Location = ACTION_CONTACT_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE CONTACT]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Conta/Contact_Duplicate
// USE /[MANUAL FOR CONTROLLER_DUPLICATE_POST CONTACT]/
		[HttpPost]
		public ActionResult Contact_Duplicate([FromBody]Contact_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Contact_Duplicate",
				ViewName = "Contact",
				AreaName = "conta",
				Location = ACTION_CONTACT_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_DUPLICATE CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_DUPLICATE CONTACT]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE_EX CONTACT]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE_EX CONTACT]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Contact_Cancel

		//
		// GET: /Conta/Contact_Cancel
// USE /[MANUAL FOR CONTROLLER_CANCEL_GET CONTACT]/
		public ActionResult Contact_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					GenioMVC.Models.Conta model = new(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("conta");

// USE /[MANUAL FOR BEFORE_CANCEL CONTACT]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL FOR AFTER_CANCEL CONTACT]/

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

				Navigation.SetValue("ForcePrimaryRead_conta", "true", true);
			}

			Navigation.ClearValue("conta");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		public class Contact_PropeValTitleModel : RequestLookupModel
		{
			public Contact_ViewModel Model { get; set; }
		}

		//
		// GET: /Conta/Contact_PropeValTitle
		// POST: /Conta/Contact_PropeValTitle
		[ActionName("Contact_PropeValTitle")]
		public ActionResult Contact_PropeValTitle([FromBody] Contact_PropeValTitleModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_prope")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_prope");
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

			Models.Conta parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Contact_PropeValTitle_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(requestModel.TableConfiguration);

			model.setModes(Request.Query["m"].ToString());
			// Map received value to field - The 'multiple values' type limit
			model.ValVisit_date = Navigation.GetValue<DateTime?>("conta.visit_date");
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		// POST: /Conta/Contact_SaveEdit
		[HttpPost]
		public ActionResult Contact_SaveEdit([FromBody] Contact_ViewModel model)
		{
			EventSink eventSink = new()
			{
				MethodName = "Contact_SaveEdit",
				ViewName = "Contact",
				AreaName = "conta",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_APPLY_EDIT CONTACT]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_APPLY_EDIT CONTACT]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}

		public class ContactDocumValidateTickets : RequestDocumValidateTickets
		{
			public Contact_ViewModel Model { get; set; }
		}

		/// <summary>
		/// Checks if the model is valid and, if so, updates the specified tickets with write permissions
		/// </summary>
		/// <param name="requestModel">The request model with a list of tickets and the form model</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult UpdateFilesTicketsContact([FromBody] ContactDocumValidateTickets requestModel)
		{
			requestModel.Model.Init(UserContext.Current);
			return UpdateFilesTickets(requestModel.Tickets, requestModel.Model, requestModel.IsApply);
		}
	}
}
