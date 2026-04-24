using JsonPropertyName = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
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
using GenioMVC.Helpers;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioMVC.Resources;
using GenioMVC.ViewModels;
using GenioMVC.ViewModels.Prope;
using GenioServer.business;
using CSGenio.core.ai;

using Quidgest.Persistence.GenericQuery;

// USE /[MANUAL FOR INCLUDE_CONTROLLER PROPE]/

namespace GenioMVC.Controllers
{
	public partial class PropeController : ControllerBase
	{
		private IChatbotService _aiService;
		public PropeController(UserContextService userContext, IChatbotService aiService) : base(userContext)
		{
			_aiService = aiService;
		}

// USE /[MANUAL FOR CONTROLLER_NAVIGATION PROPE]/



		protected JsonResult FOR_MenuR_BTN_SELL(CriteriaSet crs, List<Relation> relations, CSGenio.business.Area routineArea)
		{
			try
			{
				using (CSGenio.core.di.GenioDI.MetricsOtlp.RecordTime("manua_exec_time", new System.Diagnostics.TagList([
					new("Name", "CONTROLLER_ROUTINE_BODY"),
					new("Parameter", "BTN_SELL"),
					new("ModuleOrSystem", "FOR")
				]), "ms", "Time to execute the manual code.")) {
//Platform: MVC | Type: CONTROLLER_ROUTINE_BODY | Module: FOR | Parameter: BTN_SELL | File:  | Order: 0
//BEGIN_MANUALCODE_CODMANUA:2d29b629-eb43-4c2d-a0f5-670396c7763d
var sp = m_userContext.PersistentSupport;
var user = m_userContext.User;
var selectedList = CSGenioAprope.searchList(sp, user, crs);
					DateTime today = DateTime.Today;

sp.openConnection();
foreach (var item in selectedList)
{
				sp.Execute(new UpdateQuery()
					.Update(Area.AreaPROPE)
					.Set(CSGenioAprope.FldSold, 1)
					.Set(CSGenioAprope.FldDtsold, today)
					.Where(CriteriaSet.And().Equal(CSGenioAprope.FldCodprope, item.ValCodprope)));				

}
sp.closeConnection();
return Json(new { Success = "sucess", Message = "ok" });
//END_MANUALCODE
				}

			}
			catch (BusinessException ex)
			{
				return Json(new { success = "E", message = ex.UserMessage });
			}
			catch (Exception ex)
			{
				Log.Error("Error in action FOR_MenuR_BTN_SELL: " + ex.Message);
				return Json(new { success = "E", message = Resources.Resources.PEDIMOS_DESCULPA__OC63848 });
			}
		}

		// POST: /Prope/FOR_Menu_411_MenuR_BTN_SELL
		public JsonResult FOR_Menu_411_MenuR_BTN_SELL([FromBody] RequestRoutineMultipleModel requestModel)
		{
			CSGenio.business.Area area = CSGenio.business.Area.createArea("prope", UserContext.Current.User, UserContext.Current.User.CurrentModule);
			ListViewModel model = new FOR_Menu_411_ViewModel(m_userContext);
			NameValueCollection parameters;

			// Fetch and format the parameters
			if (requestModel.QueryParams != null && requestModel.QueryParams.Count() > 0)
				parameters = FormatQueryString(requestModel.QueryParams);
			else
				parameters = this.Navigation.GetValue<NameValueCollection>("requestValuesFOR_Menu_411");

			CSGenio.core.framework.table.TableConfiguration tableConfig = model.GetTableConfig(
				requestModel.TableConfiguration,
				requestModel.UserTableConfigName,
				requestModel.LoadDefaultView);

			// Get CriteriaSet
			CriteriaSet crs = model.BuildCriteriaSet(tableConfig, parameters, out bool hasAllRequiredLimits);

			if (!requestModel.AllSelected || crs == null)
				crs.In("prope", "CODPROPE", requestModel.Ids);

			// Fetch List of Related Areas
			List<string> relatedTables = [];
			QueryUtils.checkConditionsForForeignTables(crs, area, relatedTables);

			/*
			 * This is a list of Relationships that has to be included in the query that will be using the CriteriaSet.
			 * This can be done using QueryUtils.setFromTabDirect()
			 */
			List<CSGenio.framework.Relation> relations = QueryUtils.tablesRelationships(relatedTables, area);

			return FOR_MenuR_BTN_SELL(crs, relations, area);
		}

		private List<string> GetActionIds(CriteriaSet crs, CSGenio.persistence.PersistentSupport sp = null)
		{
			CSGenio.business.Area area = CSGenio.business.Area.createArea<CSGenioAprope>(UserContext.Current.User, UserContext.Current.User.CurrentModule);
			return base.GetActionIds(crs, sp, area);
		}

// USE /[MANUAL FOR MANUAL_CONTROLLER PROPE]/

		[HttpPost]
		public JsonResult ReloadDBEdit([FromBody]RequestReloadDBEditModel requestModel)
		{
			var Identifier = requestModel.Identifier ?? "";
			var qs = new NameValueCollection();
			qs.AddRange(Request.Query);
			// The value of the lookup search field comes in 'Values'
			if (requestModel.Values != null)
				qs.AddRange(requestModel.Values);
			this.IsStateReadonly = true;

			dynamic result = null;
			/*
				Instead of loading the entire record from the database, a record will be created in memory with the keys filled in,
					and additional fields from "Field" type limits will be mapped later.
				This allows us to reduce database queries, as we already have all the necessary information to apply the limits.
			*/
			Models.Prope row = new Models.Prope(UserContext.Current, isEmpty: true);
			row.klass.QPrimaryKey = Navigation.GetStrValue("prope");
			row.LoadKeysFromHistory(Navigation, Navigation.CurrentLevel.Level, false, true, true, true);

			// Only the last reload request is accepted.
			var requestNumber = Request.Headers["ReloadDBEditRequestNumber"];
			if (requestNumber != StringValues.Empty)
				Response.Headers["ReloadDBEditRequestNumber"] = requestNumber.First();

			try
			{
				switch (string.IsNullOrEmpty(Identifier) ? "" : Identifier)
				{
					case "PROPERTYAGENTNAME____":	// Field (DB)
						{
							var model = new Property_ViewModel(UserContext.Current) { editable = false };
							model.MapFromModel(row);
							model.Load_Propertyagentname____(qs);
							result = model.TableAgentName;
						}
						break;
					case "PROPERTYCITY_CITY____":	// Field (DB)
						{
							var model = new Property_ViewModel(UserContext.Current) { editable = false };
							model.MapFromModel(row);
							model.Load_Propertycity_city____(qs);
							result = model.TableCityCity;
						}
						break;
					default:
						break;
				}
			}
			catch (Exception)
			{
				return JsonERROR("On Reload form field: " + Identifier);
			}

			if (result != null)
				return JsonOK(result);
			return JsonERROR("Not found any valid result");
		}

		[HttpPost]
		public JsonResult GetDependants([FromBody]RequestDependantsModel requestModel)
		{
			var Identifier = requestModel.Identifier;
			var Selected = requestModel.Selected;

			ConcurrentDictionary<string, object> values = null;
			this.IsStateReadonly = true;

			try
			{
				// Only the last reload request is accepted.
				var requestNumber = Request.Headers["GetDependantsRequestNumber"];
				if (requestNumber != StringValues.Empty)
					Response.Headers["GetDependantsRequestNumber"] = requestNumber.First();

				UserContext.Current.PersistentSupport.openConnection();
				switch (string.IsNullOrEmpty(Identifier) ? "" : Identifier)
				{
					case "PROPERTYAGENTNAME____":	// Field (DB)
						values = new Property_ViewModel(UserContext.Current).GetDependant_PropertyTableAgentName(Selected);
						break;
					case "PROPERTYCITY_CITY____":	// Field (DB)
						values = new Property_ViewModel(UserContext.Current).GetDependant_PropertyTableCityCity(Selected);
						break;
					default: break;
				}

				if (values == null || !values.Any())
					return JsonERROR("List is empty");

				// Remove DateTime.MinValue
				foreach (KeyValuePair<string, object> field in values)
					if (field.Value is DateTime && (DateTime)field.Value == DateTime.MinValue)
						values.TryUpdate(field.Key, "", DateTime.MinValue);

				// TODO: Sanitize HTML content
				return JsonOK(values);
			}
			catch (Exception)
			{
				return JsonERROR("On Get Dependants - " + Identifier);
			}
			finally
			{
				UserContext.Current.PersistentSupport.closeConnection();
			}
		}





		/// <summary>
		/// Recalculate formulas of the "Property" form. (++, CT, SR, CL and U1)
		/// </summary>
		/// <param name="formData">Current form data</param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult RecalculateFormulas_Property([FromBody]Property_ViewModel formData)
		{
			return GenericRecalculateFormulas(formData, "prope",
				(primaryKey) => Models.Prope.Find(primaryKey, UserContext.Current, "FPROPERTY"),
				(model) => formData.MapToModel(model as Models.Prope)
			);
		}

		/// <summary>
		/// Get "See more..." tree structure
		/// </summary>
		/// <returns></returns>
		public JsonResult GetTreeSeeMore([FromBody]RequestLookupModel requestModel)
		{
			var Identifier = requestModel.Identifier;
			var queryParams = requestModel.QueryParams;

			try
			{
				// We need the request values to apply filters
				var requestValues = new NameValueCollection();
				if (queryParams != null)
					foreach (var kv in queryParams)
						requestValues.Add(kv.Key, kv.Value);

				switch (string.IsNullOrEmpty(Identifier) ? "" : Identifier)
				{
					default:
						break;
				}
			}
			catch (Exception)
			{
				return Json(new { Success = false, Message = "Error" });
			}

			return Json(new { Success = false, Message = "Error" });
		}

		/// <summary>
		/// Gets the necessary tickets to interact with the given document
		/// </summary>
		/// <param name="requestModel">The request model with the table, field and the primary key of the record</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult GetDocumsTickets([FromBody] RequestDocumGetTicketsModel requestModel)
		{
			return base.GetDocumsTickets("PROPE", requestModel.FieldName, requestModel.KeyValue);
		}

		/// <summary>
		/// Gets the versions of the specified document
		/// </summary>
		/// <param name="requestModel">The request model with the ticket</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult GetFileVersions([FromBody] RequestDocumGetModel requestModel)
		{
			return base.GetFileVersions(requestModel.Ticket);
		}

		/// <summary>
		/// Gets the properties of the specified document
		/// </summary>
		/// <param name="requestModel">The request model with the ticket</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult GetFileProperties([FromBody] RequestDocumGetModel requestModel)
		{
			return base.GetFileProperties(requestModel.Ticket);
		}

		/// <summary>
		/// Gets the binary file associated to the specified document
		/// </summary>
		/// <param name="requestModel">The request model with the ticket and view type</param>
		/// <returns>A File object with the content of the document</returns>
		public ActionResult GetFile([FromBody] RequestDocumGetModel requestModel)
		{
			return base.GetFile(requestModel.Ticket, requestModel.ViewType);
		}

		/// <summary>
		/// Changes the state/properties of a given document
		/// </summary>
		/// <param name="requestModel">The request model with a list of changes</param>
		/// <returns>A JSON response with the result of the operation</returns>
		public ActionResult SetFilesState([FromBody] RequestDocumsChangeModel requestModel)
		{
			return base.SetFilesState(requestModel.Documents);
		}
	}
}
