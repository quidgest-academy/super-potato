using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.reporting;
using CSGenio.core.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioMVC.Resources;
using GenioMVC.ViewModels.Prope;
using Quidgest.Persistence.GenericQuery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// USE /[MANUAL FOR INCLUDE_CONTROLLER PROPE]/

namespace GenioMVC.Controllers
{
	public partial class PropeController : ControllerBase
	{
		private static readonly NavigationLocation ACTION_FOR_MENU_21 = new NavigationLocation("PROPERTIES34868", "FOR_Menu_21", "Prope") { vueRouteName = "menu-FOR_21" };
		private static readonly NavigationLocation ACTION_FOR_MENU_411 = new NavigationLocation("PROPERTIES34868", "FOR_Menu_411", "Prope") { vueRouteName = "menu-FOR_411" };
		private static readonly NavigationLocation ACTION_FOR_MENU_511 = new NavigationLocation("PROPERTIES34868", "FOR_Menu_511", "Prope") { vueRouteName = "menu-FOR_511" };


		//
		// GET: /Prope/FOR_Menu_21
		[ActionName("FOR_Menu_21")]
		[HttpPost]
		public ActionResult FOR_Menu_21([FromBody] RequestMenuModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			FOR_Menu_21_ViewModel model = new(m_userContext);

			CSGenio.core.framework.table.legacy.v1.TableConfigurationUpdate.SetFilterShiftValue(model.Uuid, "filter_FOR_Menu_21_BUILDINGTY", 0);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(
				requestModel.TableConfiguration,
				requestModel.UserTableConfigName,
				requestModel.LoadDefaultView);

			// Determine rows per page
			tableConfig.RowsPerPage = tableConfig.DetermineRowsPerPage(CSGenio.framework.Configuration.NrRegDBedit, "");

			bool isHomePage = RouteData.Values.ContainsKey("isHomePage") ? (bool)RouteData.Values["isHomePage"] : false;
			if (isHomePage)
				Navigation.SetValue("HomePage", "FOR_Menu_21");

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_prope")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_prope");
				UserContext.Current.SetPersistenceReadOnly(false);
			}
			CSGenio.framework.StatusMessage result = model.CheckPermissions(FormMode.List);
			if (result.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(result.Message);

			NameValueCollection querystring = [];
			if (queryParams != null && queryParams.Count > 0)
				querystring.AddRange(queryParams);

			if (!isHomePage &&
				(Navigation.CurrentLevel == null || !ACTION_FOR_MENU_21.IsSameAction(Navigation.CurrentLevel.Location)) &&
				Navigation.CurrentLevel.Location.Action != ACTION_FOR_MENU_21.Action)
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + Navigation.CurrentLevel.Location.ShortDescription());
			else if (isHomePage)
			{
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + ACTION_FOR_MENU_21.ShortDescription());
				Navigation.SetValue("HomePageContainsList", true);
			}



// USE /[MANUAL FOR MENU_GET 21]/

			// Table List Export - check if user is exporting the Qlisting
			if (querystring["ExportList"] != null && Convert.ToBoolean(querystring["ExportList"]) && querystring["ExportType"] != null)
			{
				string exportType = querystring["ExportType"];
				string file = "FOR_Menu_21_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "." + exportType;
				ListingMVC<CSGenioAprope> listing = null;
				CriteriaSet conditions = null;
				List<CSGenio.framework.Exports.QColumn> columns = null;
				model.LoadToExport(out listing, out conditions, out columns, tableConfig, querystring, Request.IsAjaxRequest());

				// Validate export format (Currently, this functionality is only implemented in MVC Razor)
				/*if (querystring["ExportValidate"] == "true")
				{
					bool isValidExport = new CSGenio.framework.Exports(UserContext.Current.User).ExportListValidation(listing, conditions, columns, exportType);
					return Json(new { ValidFormat = isValidExport });
				}*/

				byte[] fileBytes = null;
// USE /[MANUAL FOR OVERRQEXPORT 21]/
				// Protected against cases where it receive zero columns. Otherwise, it will select all columns in the area.
				if (listing.RequestFields.Length == 0)
					return JsonERROR(Resources.Resources.A_EXPORTACAO_NAO_POD03671);
				fileBytes = new CSGenio.framework.Exports(UserContext.Current.User).ExportList(listing, conditions, columns, exportType, file,ACTION_FOR_MENU_21.Name);

				QCache.Instance.ExportFiles.Put(file, fileBytes);
				return Json(GetJsonForDownloadExportFile(file, querystring["ExportType"]));
			}

			if (querystring["ImportList"] != null && Convert.ToBoolean(querystring["ImportList"]) && querystring["ImportType"] != null)
			{
				string importType =  querystring["ImportType"];
				string file = "FOR_Menu_21_Template" + "." + importType;
				List<CSGenio.framework.Exports.QColumn> columns = null;
				model.LoadToExportTemplate(out columns);
				byte[] fileBytes = null;

				fileBytes = new CSGenio.framework.Exports(UserContext.Current.User).ExportTemplate(columns, importType, file,ACTION_FOR_MENU_21.Name);

				QCache.Instance.ExportFiles.Put(file, fileBytes);
				return Json(GetJsonForDownloadExportFile(file, importType));
			}

			try
			{
				model.Load(tableConfig, querystring, Request.IsAjaxRequest());
			}
			catch (Exception e)
			{
				return JsonERROR(HandleException(e), model);
			}


			return JsonOK(model);
		}

		//
		// POST: /Prope/FOR_Menu_21_UploadFile
		[HttpPost]
		public ActionResult FOR_Menu_21_UploadFile(string importType, string qqfile) {
			FOR_Menu_21_ViewModel model = new FOR_Menu_21_ViewModel(UserContext.Current);

			PersistentSupport sp = UserContext.Current.PersistentSupport;
			List<CSGenioAprope> rows = new List<CSGenioAprope>();
			List<String> results = new List<String>();

			try
			{
				var file = Request.Form.Files[0];
				byte[] fileBytes = new byte[file.Length];
				var mem = new MemoryStream(fileBytes);
				file.CopyTo(mem);

				List<CSGenio.framework.Exports.QColumn> columns = null;
				model.LoadToExportTemplate(out columns);

				rows = new CSGenio.framework.Exports( UserContext.Current.User).ImportList<CSGenioAprope>(columns, importType, fileBytes);

				sp.openTransaction();
				int lineNumber = 0;
				foreach (CSGenioAprope importRow in rows)
				{
					try
					{
						lineNumber++;
						importRow.ValidateIfIsNull = true;
						importRow.insertPseud(UserContext.Current.PersistentSupport);
						importRow.change(UserContext.Current.PersistentSupport, (CriteriaSet)null);
					}
					catch (GenioException e)
					{
						string lineNumberMsg = String.Format(Resources.Resources.ERROR_IN_LINE__0__45377 + " ", lineNumber);
						e.UserMessage = lineNumberMsg + e.UserMessage;
						throw;
					}
				}
				sp.closeTransaction();

				results.Add(string.Format(Resources.Resources._0__LINHAS_IMPORTADA15937, rows.Count));

				return Json(new { success = true, lines = results, msg = Resources.Resources.FICHEIRO_IMPORTADO_C51013 });
			}
			catch (GenioException e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();
				CSGenio.framework.Log.Error(e.Message);
				results.Add(e.UserMessage);

				return Json(new { success = false, errors = results, msg = Resources.Resources.ERROR_IMPORTING_FILE09339 });
			}
		}

		//
		// GET: /Prope/FOR_Menu_411
		[ActionName("FOR_Menu_411_Selections")]
		[HttpPost]
		public ActionResult FOR_Menu_411_Selections([FromBody] RequestSelectionsModel requestModel)
		{
			var ids = requestModel.Ids.ToArray();

			Navigation.ClearValue("prope_Selections");
			if (ids != null && ids.Length != 0)
				Navigation.SetValue("prope_Selections", ids);
			return Json(new { Success = true });
		}

		[ActionName("FOR_Menu_411")]
		[HttpPost]
		public ActionResult FOR_Menu_411([FromBody] RequestMenuModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			FOR_Menu_411_ViewModel model = new(m_userContext);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(
				requestModel.TableConfiguration,
				requestModel.UserTableConfigName,
				requestModel.LoadDefaultView);

			// Determine rows per page
			tableConfig.RowsPerPage = tableConfig.DetermineRowsPerPage(CSGenio.framework.Configuration.NrRegDBedit, "");

			bool isHomePage = RouteData.Values.ContainsKey("isHomePage") ? (bool)RouteData.Values["isHomePage"] : false;
			if (isHomePage)
				Navigation.SetValue("HomePage", "FOR_Menu_411");

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_prope")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_prope");
				UserContext.Current.SetPersistenceReadOnly(false);
			}
			CSGenio.framework.StatusMessage result = model.CheckPermissions(FormMode.List);
			if (result.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(result.Message);

			NameValueCollection querystring = [];
			if (queryParams != null && queryParams.Count > 0)
				querystring.AddRange(queryParams);

			if (!isHomePage &&
				(Navigation.CurrentLevel == null || !ACTION_FOR_MENU_411.IsSameAction(Navigation.CurrentLevel.Location)) &&
				Navigation.CurrentLevel.Location.Action != ACTION_FOR_MENU_411.Action)
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + Navigation.CurrentLevel.Location.ShortDescription());
			else if (isHomePage)
			{
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + ACTION_FOR_MENU_411.ShortDescription());
				Navigation.SetValue("HomePageContainsList", true);
			}


			if (!String.IsNullOrEmpty(querystring["agent"]))
				Navigation.SetValue("agent", querystring["agent"]);


// USE /[MANUAL FOR MENU_GET 411]/

			try
			{
				model.Load(tableConfig, querystring, Request.IsAjaxRequest());
			}
			catch (Exception e)
			{
				return JsonERROR(HandleException(e), model);
			}


			return JsonOK(model);
		}

		//
		// GET: /Prope/FOR_Menu_511
		[ActionName("FOR_Menu_511")]
		[HttpPost]
		public ActionResult FOR_Menu_511([FromBody] RequestMenuModel requestModel)
		{
			var queryParams = requestModel.QueryParams;

			FOR_Menu_511_ViewModel model = new(m_userContext);

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(
				requestModel.TableConfiguration,
				requestModel.UserTableConfigName,
				requestModel.LoadDefaultView);

			// Determine rows per page
			tableConfig.RowsPerPage = tableConfig.DetermineRowsPerPage(CSGenio.framework.Configuration.NrRegDBedit, "");

			bool isHomePage = RouteData.Values.ContainsKey("isHomePage") ? (bool)RouteData.Values["isHomePage"] : false;
			if (isHomePage)
				Navigation.SetValue("HomePage", "FOR_Menu_511");

			// If there was a recent operation on this table then force the primary persistence server to be called and ignore the read only feature
			if (string.IsNullOrEmpty(Navigation.GetStrValue("ForcePrimaryRead_prope")))
				UserContext.Current.SetPersistenceReadOnly(true);
			else
			{
				Navigation.DestroyEntry("ForcePrimaryRead_prope");
				UserContext.Current.SetPersistenceReadOnly(false);
			}
			CSGenio.framework.StatusMessage result = model.CheckPermissions(FormMode.List);
			if (result.Status.Equals(CSGenio.framework.Status.E))
				return PermissionError(result.Message);

			NameValueCollection querystring = [];
			if (queryParams != null && queryParams.Count > 0)
				querystring.AddRange(queryParams);

			if (!isHomePage &&
				(Navigation.CurrentLevel == null || !ACTION_FOR_MENU_511.IsSameAction(Navigation.CurrentLevel.Location)) &&
				Navigation.CurrentLevel.Location.Action != ACTION_FOR_MENU_511.Action)
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + Navigation.CurrentLevel.Location.ShortDescription());
			else if (isHomePage)
			{
				CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.MENU01948 + " " + ACTION_FOR_MENU_511.ShortDescription());
				Navigation.SetValue("HomePageContainsList", true);
			}


			Navigation.SetValue("prope.sold", "1");

// USE /[MANUAL FOR MENU_GET 511]/

			try
			{
				model.Load(tableConfig, querystring, Request.IsAjaxRequest());
			}
			catch (Exception e)
			{
				return JsonERROR(HandleException(e), model);
			}


			return JsonOK(model);
		}



	}
}
