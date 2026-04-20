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
using GenioMVC.ViewModels.Photo;
using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR INCLUDE_CONTROLLER PHOTO]/

namespace GenioMVC.Controllers
{
	public partial class PhotoController : ControllerBase
	{
		#region NavigationLocation Names

		private static readonly NavigationLocation ACTION_ALBUM_CANCEL = new("PHOTO_ALBUM31473", "Album_Cancel", "Photo") { vueRouteName = "form-ALBUM", mode = "CANCEL" };
		private static readonly NavigationLocation ACTION_ALBUM_SHOW = new("PHOTO_ALBUM31473", "Album_Show", "Photo") { vueRouteName = "form-ALBUM", mode = "SHOW" };
		private static readonly NavigationLocation ACTION_ALBUM_NEW = new("PHOTO_ALBUM31473", "Album_New", "Photo") { vueRouteName = "form-ALBUM", mode = "NEW" };
		private static readonly NavigationLocation ACTION_ALBUM_EDIT = new("PHOTO_ALBUM31473", "Album_Edit", "Photo") { vueRouteName = "form-ALBUM", mode = "EDIT" };
		private static readonly NavigationLocation ACTION_ALBUM_DUPLICATE = new("PHOTO_ALBUM31473", "Album_Duplicate", "Photo") { vueRouteName = "form-ALBUM", mode = "DUPLICATE" };
		private static readonly NavigationLocation ACTION_ALBUM_DELETE = new("PHOTO_ALBUM31473", "Album_Delete", "Photo") { vueRouteName = "form-ALBUM", mode = "DELETE" };

		#endregion

		#region Album private

		private void FormHistoryLimits_Album()
		{

		}

		#endregion

		#region Album_Show

// USE /[MANUAL FOR CONTROLLER_SHOW ALBUM]/

		[HttpPost]
		public ActionResult Album_Show_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Album_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Album_Show_GET",
				AreaName = "photo",
				Location = ACTION_ALBUM_SHOW,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Album();
// USE /[MANUAL FOR BEFORE_LOAD_SHOW ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_SHOW ALBUM]/
				}
			};

			return GenericHandleGetFormShow(eventSink, model, id);
		}

		#endregion

		#region Album_New

// USE /[MANUAL FOR CONTROLLER_NEW_GET ALBUM]/
		[HttpPost]
		public ActionResult Album_New_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;
			var prefillValues = requestModel.PrefillValues;

			Album_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Album_New_GET",
				AreaName = "photo",
				FormName = "ALBUM",
				Location = ACTION_ALBUM_NEW,
				BeforeAll = (sink, sp) =>
				{
					FormHistoryLimits_Album();
				},
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW ALBUM]/
				}
			};

			return GenericHandleGetFormNew(eventSink, model, id, isNewLocation, prefillValues);
		}

		//
		// POST: /Photo/Album_New
// USE /[MANUAL FOR CONTROLLER_NEW_POST ALBUM]/
		[HttpPost]
		public ActionResult Album_New([FromBody]Album_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Album_New",
				ViewName = "Album",
				AreaName = "photo",
				Location = ACTION_ALBUM_NEW,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_NEW ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_NEW ALBUM]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_NEW_EX ALBUM]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_NEW_EX ALBUM]/
				}
			};

			return GenericHandlePostFormNew(eventSink, model);
		}

		#endregion

		#region Album_Edit

// USE /[MANUAL FOR CONTROLLER_EDIT_GET ALBUM]/
		[HttpPost]
		public ActionResult Album_Edit_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Album_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Album_Edit_GET",
				AreaName = "photo",
				FormName = "ALBUM",
				Location = ACTION_ALBUM_EDIT,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Album();
// USE /[MANUAL FOR BEFORE_LOAD_EDIT ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT ALBUM]/
				}
			};

			return GenericHandleGetFormEdit(eventSink, model, id);
		}

		//
		// POST: /Photo/Album_Edit
// USE /[MANUAL FOR CONTROLLER_EDIT_POST ALBUM]/
		[HttpPost]
		public ActionResult Album_Edit([FromBody]Album_ViewModel model, [FromQuery]bool redirect)
		{
			EventSink eventSink = new()
			{
				MethodName = "Album_Edit",
				ViewName = "Album",
				AreaName = "photo",
				Location = ACTION_ALBUM_EDIT,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_EDIT ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_EDIT ALBUM]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_EDIT_EX ALBUM]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_EDIT_EX ALBUM]/
				}
			};

			return GenericHandlePostFormEdit(eventSink, model);
		}

		#endregion

		#region Album_Delete

// USE /[MANUAL FOR CONTROLLER_DELETE_GET ALBUM]/
		[HttpPost]
		public ActionResult Album_Delete_GET([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Album_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Album_Delete_GET",
				AreaName = "photo",
				FormName = "ALBUM",
				Location = ACTION_ALBUM_DELETE,
				BeforeOp = (sink, sp) =>
				{
					FormHistoryLimits_Album();
// USE /[MANUAL FOR BEFORE_LOAD_DELETE ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DELETE ALBUM]/
				}
			};

			return GenericHandleGetFormDelete(eventSink, model, id);
		}

		//
		// POST: /Photo/Album_Delete
// USE /[MANUAL FOR CONTROLLER_DELETE_POST ALBUM]/
		[HttpPost]
		public ActionResult Album_Delete([FromBody] RequestIdModel requestModel)
		{
			string id = requestModel.Id;
			Album_ViewModel model = new(UserContext.Current, id);
			model.MapFromModel();

			EventSink eventSink = new()
			{
				MethodName = "Album_Delete",
				ViewName = "Album",
				AreaName = "photo",
				Location = ACTION_ALBUM_DELETE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_DESTROY_DELETE ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_DESTROY_DELETE ALBUM]/
				}
			};

			return GenericHandlePostFormDelete(eventSink, model);
		}

		public ActionResult Album_Delete_Redirect()
		{
			//FOR: FORM MENU GO BACK
			return RedirectToFormMenuGoBack("ALBUM");
		}

		#endregion

		#region Album_Duplicate

// USE /[MANUAL FOR CONTROLLER_DUPLICATE_GET ALBUM]/

		[HttpPost]
		public ActionResult Album_Duplicate_GET([FromBody] RequestNewGetModel requestModel)
		{
			string id = requestModel.Id;
			bool isNewLocation = requestModel.IsNewLocation;

			Album_ViewModel model = new(UserContext.Current);
			EventSink eventSink = new()
			{
				MethodName = "Album_Duplicate_GET",
				AreaName = "photo",
				FormName = "ALBUM",
				Location = ACTION_ALBUM_DUPLICATE,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE ALBUM]/
				}
			};

			return GenericHandleGetFormDuplicate(eventSink, model, id, isNewLocation);
		}

		//
		// POST: /Photo/Album_Duplicate
// USE /[MANUAL FOR CONTROLLER_DUPLICATE_POST ALBUM]/
		[HttpPost]
		public ActionResult Album_Duplicate([FromBody]Album_ViewModel model, [FromQuery]bool redirect = true)
		{
			EventSink eventSink = new()
			{
				MethodName = "Album_Duplicate",
				ViewName = "Album",
				AreaName = "photo",
				Location = ACTION_ALBUM_DUPLICATE,
				Redirect = redirect,
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_SAVE_DUPLICATE ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_SAVE_DUPLICATE ALBUM]/
				},
				BeforeException = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_LOAD_DUPLICATE_EX ALBUM]/
				},
				AfterException = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_LOAD_DUPLICATE_EX ALBUM]/
				}
			};

			return GenericHandlePostFormDuplicate(eventSink, model);
		}

		#endregion

		#region Album_Cancel

		//
		// GET: /Photo/Album_Cancel
// USE /[MANUAL FOR CONTROLLER_CANCEL_GET ALBUM]/
		public ActionResult Album_Cancel()
		{
			if (Navigation.CurrentLevel.FormMode == FormMode.New || Navigation.CurrentLevel.FormMode == FormMode.Duplicate)
			{
				PersistentSupport sp = UserContext.Current.PersistentSupport;
				try
				{
					GenioMVC.Models.Photo model = new(UserContext.Current);
					model.klass.QPrimaryKey = Navigation.GetStrValue("photo");

// USE /[MANUAL FOR BEFORE_CANCEL ALBUM]/

					sp.openTransaction();
					model.Destroy();
					sp.closeTransaction();

// USE /[MANUAL FOR AFTER_CANCEL ALBUM]/

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

				Navigation.SetValue("ForcePrimaryRead_photo", "true", true);
			}

			Navigation.ClearValue("photo");

			return JsonOK(new { Success = true, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		#endregion


		public class Album_PropeValTitleModel : RequestLookupModel
		{
			public Album_ViewModel Model { get; set; }
		}

		//
		// GET: /Photo/Album_PropeValTitle
		// POST: /Photo/Album_PropeValTitle
		[ActionName("Album_PropeValTitle")]
		public ActionResult Album_PropeValTitle([FromBody] Album_PropeValTitleModel requestModel)
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

			Models.Photo parentCtx = requestModel.Model == null ? null : new(m_userContext);
			requestModel.Model?.Init(m_userContext);
			requestModel.Model?.MapToModel(parentCtx);
			Album_PropeValTitle_ViewModel model = new(m_userContext, parentCtx);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(requestModel.TableConfiguration);

			model.setModes(Request.Query["m"].ToString());
			model.Load(tableConfig, requestValues, Request.IsAjaxRequest());

			return JsonOK(model);
		}

		// POST: /Photo/Album_SaveEdit
		[HttpPost]
		public ActionResult Album_SaveEdit([FromBody] Album_ViewModel model)
		{
			EventSink eventSink = new()
			{
				MethodName = "Album_SaveEdit",
				ViewName = "Album",
				AreaName = "photo",
				BeforeOp = (sink, sp) =>
				{
// USE /[MANUAL FOR BEFORE_APPLY_EDIT ALBUM]/
				},
				AfterOp = (sink, sp) =>
				{
// USE /[MANUAL FOR AFTER_APPLY_EDIT ALBUM]/
				}
			};

			return GenericHandlePostFormApply(eventSink, model);
		}

		public class AlbumDocumValidateTickets : RequestDocumValidateTickets
		{
			public Album_ViewModel Model { get; set; }
		}

		/// <summary>
		/// Checks if the model is valid and, if so, updates the specified tickets with write permissions
		/// </summary>
		/// <param name="requestModel">The request model with a list of tickets and the form model</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult UpdateFilesTicketsAlbum([FromBody] AlbumDocumValidateTickets requestModel)
		{
			requestModel.Model.Init(UserContext.Current);
			return UpdateFilesTickets(requestModel.Tickets, requestModel.Model, requestModel.IsApply);
		}
	}
}
