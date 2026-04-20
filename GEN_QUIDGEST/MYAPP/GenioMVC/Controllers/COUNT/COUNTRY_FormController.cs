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
using GenioMVC.ViewModels.Count;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR INCLUDE_CONTROLLER COUNT]/

namespace GenioMVC.Controllers
{
	public partial class CountController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_COUNTRY_CANCEL = new("COUNTRY64133", "Country_Cancel", "Count") { vueRouteName = "form-COUNTRY", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_COUNTRY_SHOW = new("COUNTRY64133", "Country_Show", "Count") { vueRouteName = "form-COUNTRY", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_COUNTRY_NEW = new("COUNTRY64133", "Country_New", "Count") { vueRouteName = "form-COUNTRY", mode = "NEW" };
		private static readonly NavigationLocation ACTION_COUNTRY_EDIT = new("COUNTRY64133", "Country_Edit", "Count") { vueRouteName = "form-COUNTRY", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_COUNTRY_DUPLICATE = new("COUNTRY64133", "Country_Duplicate", "Count") { vueRouteName = "form-COUNTRY", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_COUNTRY_DELETE = new("COUNTRY64133", "Country_Delete", "Count") { vueRouteName = "form-COUNTRY", mode = "DELETE" };

		#endregion

		#region Country private

		private void FormHistoryLimits_Country()
		{

		}

		#endregion

		#region Country_Show

// USE /[MANUAL FOR CONTROLLER_SHOW COUNTRY]/

		[HttpPost]
		public ActionResult Country_Show_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Country_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Country_Show_GET",
				AreaName = "count",
				Location = ACTION_COUNTRY_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Country();
// USE /[MANUAL FOR BEFORE_LOAD_SHOW COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_SHOW COUNTRY]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Country_New

// USE /[MANUAL FOR CONTROLLER_NEW_GET COUNTRY]/
		[HttpPost]
		public ActionResult Country_New_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			Country_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Country_New_GET",
				AreaName = "count",
				FormName = "COUNTRY",
				Location = ACTION_COUNTRY_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Country();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW COUNTRY]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Count/Country_New
// USE /[MANUAL FOR CONTROLLER_NEW_POST COUNTRY]/
		[HttpPost]
		public ActionResult Country_New([FromBody]Country_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Country_New",
				ViewName = "Country",
				AreaName = "count",
				Location = ACTION_COUNTRY_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_NEW COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_NEW COUNTRY]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW_EX COUNTRY]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW_EX COUNTRY]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Country_Edit

// USE /[MANUAL FOR CONTROLLER_EDIT_GET COUNTRY]/
		[HttpPost]
		public ActionResult Country_Edit_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Country_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Country_Edit_GET",
				AreaName = "count",
				FormName = "COUNTRY",
				Location = ACTION_COUNTRY_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Country();
// USE /[MANUAL FOR BEFORE_LOAD_EDIT COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT COUNTRY]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Count/Country_Edit
// USE /[MANUAL FOR CONTROLLER_EDIT_POST COUNTRY]/
		[HttpPost]
		public ActionResult Country_Edit([FromBody]Country_ViewModel model, [FromQuery]bool redirect)
		{
			EventSink eventSink = new()
			{
				MethodName = "Country_Edit",
				ViewName = "Country",
				AreaName = "count",
				Location = ACTION_COUNTRY_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_EDIT COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_EDIT COUNTRY]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_EDIT_EX COUNTRY]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT_EX COUNTRY]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Country_Delete

// USE /[MANUAL FOR CONTROLLER_DELETE_GET COUNTRY]/
		[HttpPost]
		public ActionResult Country_Delete_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Country_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Country_Delete_GET",
				AreaName = "count",
				FormName = "COUNTRY",
				Location = ACTION_COUNTRY_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Country();
// USE /[MANUAL FOR BEFORE_LOAD_DELETE COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DELETE COUNTRY]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Count/Country_Delete
// USE /[MANUAL FOR CONTROLLER_DELETE_POST COUNTRY]/
		[HttpPost]
		public ActionResult Country_Delete([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Country_ViewModel model = new(UserContext.Current, id);
			model.MapFromModel();

			EventSink eventSink = new()
			{
				MethodName = "Country_Delete",
				ViewName = "Country",
				AreaName = "count",
				Location = ACTION_COUNTRY_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_DESTROY_DELETE COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_DESTROY_DELETE COUNTRY]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Country_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("COUNTRY");
		}

		#endregion

		#region Country_Duplicate

// USE /[MANUAL FOR CONTROLLER_DUPLICATE_GET COUNTRY]/

		[HttpPost]
		public ActionResult Country_Duplicate_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;

			Country_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Country_Duplicate_GET",
				AreaName = "count",
				FormName = "COUNTRY",
				Location = ACTION_COUNTRY_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE COUNTRY]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Count/Country_Duplicate
// USE /[MANUAL FOR CONTROLLER_DUPLICATE_POST COUNTRY]/
		[HttpPost]
		public ActionResult Country_Duplicate([FromBody]Country_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Country_Duplicate",
				ViewName = "Country",
				AreaName = "count",
				Location = ACTION_COUNTRY_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_DUPLICATE COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_DUPLICATE COUNTRY]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE_EX COUNTRY]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE_EX COUNTRY]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Country_Cancel

		//
		// GET: /Count/Country_Cancel
// USE /[MANUAL FOR CONTROLLER_CANCEL_GET COUNTRY]/
		public ActionResult Country_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					GenioMVC.Models.Count model = new(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("count");

// USE /[MANUAL FOR BEFORE_CANCEL COUNTRY]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL FOR AFTER_CANCEL COUNTRY]/

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

				Navigation.SetValue("ForcePrimaryRead_count", "true", true);
			}

			Navigation.ClearValue("count");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		// POST: /Count/Country_SaveEdit
		[HttpPost]
		public ActionResult Country_SaveEdit([FromBody] Country_ViewModel model)
		{
			EventSink eventSink = new()
			{
				MethodName = "Country_SaveEdit",
				ViewName = "Country",
				AreaName = "count",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_APPLY_EDIT COUNTRY]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_APPLY_EDIT COUNTRY]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}

		public class CountryDocumValidateTickets : RequestDocumValidateTickets
		{
			public Country_ViewModel Model { get; set; }
		}

		/// <summary>
		/// Checks if the model is valid and, if so, updates the specified tickets with write permissions
		/// </summary>
		/// <param name="requestModel">The request model with a list of tickets and the form model</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult UpdateFilesTicketsCountry([FromBody] CountryDocumValidateTickets requestModel)
		{
			requestModel.Model.Init(UserContext.Current);
			return UpdateFilesTickets(requestModel.Tickets, requestModel.Model, requestModel.IsApply);
		}
	}
}
