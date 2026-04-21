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
using GenioMVC.ViewModels.Ctax;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR INCLUDE_CONTROLLER CTAX]/

namespace GenioMVC.Controllers
{
	public partial class CtaxController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_CTAX_CANCEL = new("CITY_TAX48454", "Ctax_Cancel", "Ctax") { vueRouteName = "form-CTAX", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_CTAX_SHOW = new("CITY_TAX48454", "Ctax_Show", "Ctax") { vueRouteName = "form-CTAX", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_CTAX_NEW = new("CITY_TAX48454", "Ctax_New", "Ctax") { vueRouteName = "form-CTAX", mode = "NEW" };
		private static readonly NavigationLocation ACTION_CTAX_EDIT = new("CITY_TAX48454", "Ctax_Edit", "Ctax") { vueRouteName = "form-CTAX", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_CTAX_DUPLICATE = new("CITY_TAX48454", "Ctax_Duplicate", "Ctax") { vueRouteName = "form-CTAX", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_CTAX_DELETE = new("CITY_TAX48454", "Ctax_Delete", "Ctax") { vueRouteName = "form-CTAX", mode = "DELETE" };

		#endregion

		#region Ctax private

		private void FormHistoryLimits_Ctax()
		{

		}

		#endregion

		#region Ctax_Show

// USE /[MANUAL FOR CONTROLLER_SHOW CTAX]/

		[HttpPost]
		public ActionResult Ctax_Show_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Ctax_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Ctax_Show_GET",
				AreaName = "ctax",
				Location = ACTION_CTAX_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Ctax();
// USE /[MANUAL FOR BEFORE_LOAD_SHOW CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_SHOW CTAX]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Ctax_New

// USE /[MANUAL FOR CONTROLLER_NEW_GET CTAX]/
		[HttpPost]
		public ActionResult Ctax_New_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			Ctax_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Ctax_New_GET",
				AreaName = "ctax",
				FormName = "CTAX",
				Location = ACTION_CTAX_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Ctax();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW CTAX]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Ctax/Ctax_New
// USE /[MANUAL FOR CONTROLLER_NEW_POST CTAX]/
		[HttpPost]
		public ActionResult Ctax_New([FromBody]Ctax_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Ctax_New",
				ViewName = "Ctax",
				AreaName = "ctax",
				Location = ACTION_CTAX_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_NEW CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_NEW CTAX]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW_EX CTAX]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW_EX CTAX]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Ctax_Edit

// USE /[MANUAL FOR CONTROLLER_EDIT_GET CTAX]/
		[HttpPost]
		public ActionResult Ctax_Edit_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Ctax_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Ctax_Edit_GET",
				AreaName = "ctax",
				FormName = "CTAX",
				Location = ACTION_CTAX_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Ctax();
// USE /[MANUAL FOR BEFORE_LOAD_EDIT CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT CTAX]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Ctax/Ctax_Edit
// USE /[MANUAL FOR CONTROLLER_EDIT_POST CTAX]/
		[HttpPost]
		public ActionResult Ctax_Edit([FromBody]Ctax_ViewModel model, [FromQuery]bool redirect)
		{
			EventSink eventSink = new()
			{
				MethodName = "Ctax_Edit",
				ViewName = "Ctax",
				AreaName = "ctax",
				Location = ACTION_CTAX_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_EDIT CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_EDIT CTAX]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_EDIT_EX CTAX]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT_EX CTAX]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Ctax_Delete

// USE /[MANUAL FOR CONTROLLER_DELETE_GET CTAX]/
		[HttpPost]
		public ActionResult Ctax_Delete_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Ctax_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Ctax_Delete_GET",
				AreaName = "ctax",
				FormName = "CTAX",
				Location = ACTION_CTAX_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Ctax();
// USE /[MANUAL FOR BEFORE_LOAD_DELETE CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DELETE CTAX]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Ctax/Ctax_Delete
// USE /[MANUAL FOR CONTROLLER_DELETE_POST CTAX]/
		[HttpPost]
		public ActionResult Ctax_Delete([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Ctax_ViewModel model = new(UserContext.Current, id);
			model.MapFromModel();

			EventSink eventSink = new()
			{
				MethodName = "Ctax_Delete",
				ViewName = "Ctax",
				AreaName = "ctax",
				Location = ACTION_CTAX_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_DESTROY_DELETE CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_DESTROY_DELETE CTAX]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Ctax_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("CTAX");
		}

		#endregion

		#region Ctax_Duplicate

// USE /[MANUAL FOR CONTROLLER_DUPLICATE_GET CTAX]/

		[HttpPost]
		public ActionResult Ctax_Duplicate_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;

			Ctax_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Ctax_Duplicate_GET",
				AreaName = "ctax",
				FormName = "CTAX",
				Location = ACTION_CTAX_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE CTAX]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Ctax/Ctax_Duplicate
// USE /[MANUAL FOR CONTROLLER_DUPLICATE_POST CTAX]/
		[HttpPost]
		public ActionResult Ctax_Duplicate([FromBody]Ctax_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Ctax_Duplicate",
				ViewName = "Ctax",
				AreaName = "ctax",
				Location = ACTION_CTAX_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_DUPLICATE CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_DUPLICATE CTAX]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE_EX CTAX]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE_EX CTAX]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Ctax_Cancel

		//
		// GET: /Ctax/Ctax_Cancel
// USE /[MANUAL FOR CONTROLLER_CANCEL_GET CTAX]/
		public ActionResult Ctax_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					GenioMVC.Models.Ctax model = new(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("ctax");

// USE /[MANUAL FOR BEFORE_CANCEL CTAX]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL FOR AFTER_CANCEL CTAX]/

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

				Navigation.SetValue("ForcePrimaryRead_ctax", "true", true);
			}

			Navigation.ClearValue("ctax");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		public class Ctax_CityValCityModel : RequestLookupModel
		{
			public Ctax_ViewModel Model { get; set; }
		}

		//
		// GET: /Ctax/Ctax_CityValCity
		// POST: /Ctax/Ctax_CityValCity
		[ActionName("Ctax_CityValCity")]
		public ActionResult Ctax_CityValCity([FromBody] Ctax_CityValCityModel requestModel)
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

			Models.Ctax parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Ctax_CityValCity_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(requestModel.TableConfiguration);

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		// POST: /Ctax/Ctax_SaveEdit
		[HttpPost]
		public ActionResult Ctax_SaveEdit([FromBody] Ctax_ViewModel model)
		{
			EventSink eventSink = new()
			{
				MethodName = "Ctax_SaveEdit",
				ViewName = "Ctax",
				AreaName = "ctax",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_APPLY_EDIT CTAX]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_APPLY_EDIT CTAX]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}

		public class CtaxDocumValidateTickets : RequestDocumValidateTickets
		{
			public Ctax_ViewModel Model { get; set; }
		}

		/// <summary>
		/// Checks if the model is valid and, if so, updates the specified tickets with write permissions
		/// </summary>
		/// <param name="requestModel">The request model with a list of tickets and the form model</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult UpdateFilesTicketsCtax([FromBody] CtaxDocumValidateTickets requestModel)
		{
			requestModel.Model.Init(UserContext.Current);
			return UpdateFilesTickets(requestModel.Tickets, requestModel.Model, requestModel.IsApply);
		}
	}
}
