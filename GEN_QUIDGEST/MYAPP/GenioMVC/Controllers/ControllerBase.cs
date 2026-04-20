using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using JsonNetResult = Microsoft.AspNetCore.Mvc.JsonResult;

using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Helpers;
using GenioMVC.Models;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.Controllers
{
	public class ControllerExtension(IUserContextService userContextService) : Controller
	{
		protected readonly IUserContextService UserContext = userContextService;
		protected readonly UserContext m_userContext = userContextService.Current;

		/// <summary>
		/// Retrieves server errors to be sent to the client-side.
		/// </summary>
		/// <remarks>
		/// This method collates errors stored in TempData as well as those in the current thread.
		/// Useful for debugging and tracking what errors occurred during a specific request cycle.
		/// </remarks>
		/// <returns>List of error messages.</returns>
		private List<string> GetServerErrorsToClientSide()
		{
			List<string> errors = [];

			// Check if EventTracking is enabled
			if (Configuration.EventTracking)
			{
				// Fetch errors stored in TempData
				if (TempData["ErrorList"] is List<string> cachedErrors)
					errors.AddRange(cachedErrors);

				// Fetch errors from the current thread
				var currentErrors = Log.GetThreadErrors();
				if (currentErrors != null)
					errors.AddRange(currentErrors);
			}

			// Clear the error cache for the current thread
			Log.ClearThreadErrorsCache();

			return errors;
		}

		/// <summary>
		/// Returns the object as a JSON result.
		/// </summary>
		/// <param name="data">The data to be included in the response.</param>
		/// <returns>A JSON result containing the response data.</returns>
		private JsonNetResult _jsonResult(object data)
		{
			return new JsonNetResult(data);
		}

		/// <summary>
		/// Prepares the data object for serialization by invoking its
		/// <c>PrepareContentForClientSide</c> method if it implements
		/// the <see cref="IPreparableForSerialization"/> interface.
		/// </summary>
		/// <param name="data">The data object to prepare for serialization.</param>
		private static void PrepareForSerialization(object data)
		{
			if (data is IPreparableForSerialization model)
				model?.PrepareContentForClientSide();

			// TODO: In the future, we need to consider recursively preparing nested sub-properties
			// that also implement IPreparableForSerialization.
		}

		/// <summary>
		/// Returns the object as a JSON result.
		/// </summary>
		/// <param name="data">The data to be included in the response.</param>
		/// <returns>A JSON result containing the response data.</returns>
		protected JsonNetResult JsonOK(object data = null)
		{
			PrepareForSerialization(data);
			return _jsonResult(
				new
				{
					Success = true,
					Data = data,
					Errors = GetModelErrors(),
					Maintenance = Maintenance.Current,
					NavigationData = GetHistoryToUpdateClientSide(),
					eTracker = GetServerErrorsToClientSide()
				}
			);
		}

		protected JsonNetResult JsonERROR(string errorMsg = null, object data = null)
		{
			var defaultMsg = Resources.Resources.PEDIMOS_DESCULPA__OC63848;

			PrepareForSerialization(data);
			return _jsonResult(
				new
				{
					Success = false,
					Data = data,
					Message = (errorMsg ?? defaultMsg),
					Errors = GetModelErrors(),
					NavigationData = GetHistoryToUpdateClientSide(),
					eTracker = GetServerErrorsToClientSide()
				}
			);
		}

		// O MVC 5+ utiliza a serialização Newtonsoft por default
		// O override dos metodos Json permite controlar e unificar formato do Json devolvido
		// que no caso das datas, nem pode usar serialização normal do MVC 4

		protected new JsonNetResult Json(object data)
		{
			return JsonOK(data);
		}

		/// <summary>
		/// Get list of history entries to be update on the client-side (Vue.js)
		/// </summary>
		/// <returns></returns>
		private NavigationContext.ClientSideHistoryResult GetHistoryToUpdateClientSide()
		{
			if (IsStateReadonly)
				return GetHistoryNoChanges();
			return UserContext.Current.CurrentNavigation.GetHistoryToUpdateClientSide();
		}

		/// <summary>
		/// Get an empty diff of history changes.
		/// This should be used when the original Navigation is considered read-only but we make temporary changes to it
		/// </summary>
		/// <returns></returns>
		private NavigationContext.ClientSideHistoryResult GetHistoryNoChanges()
		{
			return new NavigationContext.ClientSideHistoryResult()
			{
				HistoryDiff = [],
				NavigationId = UserContext.Current.CurrentNavigation.NavigationId
			};
		}

		/// <summary>
		/// Marks this controller context behaving as if its state is readonly
		/// This makes methods discard any changes caused by processing before sending them to the client side
		/// </summary>
		protected bool IsStateReadonly { get; set; } = false;

		protected JsonNetResult PermissionError(string errorMsg = null, object data = null)
		{
			var message = errorMsg ?? Resources.Resources.PEDIMOS_DESCULPA__OC63848;
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Forbidden, message, data, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult NotFoundError(string errorMsg = null, object data = null)
		{
			var message = errorMsg ?? Resources.Resources.PEDIMOS_DESCULPA__OC63848;
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.NotFound, message, data, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult InternalServerError(string errorMsg = null, object data = null)
		{
			var message = errorMsg ?? Resources.Resources.PEDIMOS_DESCULPA__OC63848;
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.InternalServerError, message, data, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		private Dictionary<string, IList<string>> GetModelErrors()
		{
			var errors = new Dictionary<string, IList<string>>();

			if (!ModelState.IsValid)
			{
				var keys = ModelState.Keys.ToList();
				var values = ModelState.Values.ToList();
				for (int i = 0; i < values.Count; i++)
				{
					IList<string> fieldErrors = new List<string>();
					foreach (var err in values[i].Errors)
						fieldErrors.Add(err.ErrorMessage);

					if (fieldErrors.Count > 0)
						errors[keys[i]] = fieldErrors;
				}
			}

			return errors;
		}

		protected JsonNetResult RedirectToFormAction(string formName, string formMode, object routeValues = null, object model = null)
		{
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Redirect, type = "form", formName, formMode, routeValues, Data = model, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult RedirectToMenuAction(string menuId, object routeValues = null)
		{
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Redirect, type = "menu", menuId, routeValues, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult RedirectToVueRoute(string routeName, object routeValues = null)
		{
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Redirect, type = "route", routeName, routeValues, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult RedirectToErrorPage(string message)
		{
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Redirect, type = "erro", message, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult RedirectToMenuCondition(string menuId, object routeValues = null, object model = null)
		{
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Redirect, type = "menu-mc", menuId, routeValues, Data = model, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult RedirectToMenuRoutine(string menuId, string routineName, object routeValues = null, object model = null)
		{
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Redirect, type = "menu-routine", menuId, routineName, routeValues, Data = model, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		protected JsonNetResult RedirectToReport(string controller, string reportAction, string skipsPreview, object routeValues = null, object model = null)
		{
			return _jsonResult(new { statusCode = System.Net.HttpStatusCode.Redirect, type = "report", controller, reportAction, preview = (skipsPreview=="0"), routeValues, Data = model, NavigationData = GetHistoryToUpdateClientSide(), eTracker = GetServerErrorsToClientSide() });
		}

		private string _getRedirectUrlToVue(string page, object queryParameters = null, bool includeCulture = true, bool includeSystemAndModule = false, string module = null)
		{
			var culture = includeCulture ? string.Format("{0}/", CultureInfo.CurrentCulture.Name) : string.Empty;
			module = module ?? UserContext.Current.User.CurrentModule ?? "Public";
			var systemAndModule = includeSystemAndModule ? string.Format("{0}/{1}/", UserContext.Current.User.Year, module) : string.Empty;
			var queryString = string.Empty;

			if (queryParameters != null)
			{
				var properties = from p in queryParameters.GetType().GetProperties()
								 where p.GetValue(queryParameters, null) != null
								 select p.Name + "=" + System.Web.HttpUtility.UrlEncode(p.GetValue(queryParameters, null).ToString());
				queryString = string.Join("&", properties.ToArray());
			}

			return AbsoluteUrlUtils.RelativeToAbsolute(Request, $"{Request.PathBase}/#/{culture}{systemAndModule}{page}?{queryString}");
		}

		protected ActionResult RedirectToVuePage(string page, object queryParameters = null, bool includeCulture = true, bool includeSystemAndModule = false)
		{
			var url = _getRedirectUrlToVue(page, queryParameters, includeCulture, includeSystemAndModule);
			return Redirect(url);
		}

		protected ActionResult RedirectToVueFormPage(string form, string mode = "SHOW", string id = "", object queryParameters = null)
		{
			string formUrl = string.Format("form/{0}/{1}/{2}", form, mode, id),
				url = _getRedirectUrlToVue(formUrl, queryParameters, true, true);
			return Redirect(url);
		}

		public JsonNetResult VueErrorRedirect(string message)
		{
			return RedirectToErrorPage(message);
		}

		public JsonNetResult VueRouteRedirect(string routeName, object routeValues = null)
		{
			return RedirectToVueRoute(routeName, routeValues);
		}

		protected ActionResult ClientSideRedirect(string endpoint, bool captureHash = false)
		{
			endpoint = AbsoluteUrlUtils.RelativeToAbsolute(Request, endpoint);
			endpoint = System.Web.HttpUtility.JavaScriptStringEncode(endpoint);
			string hashscript = "'";
			if (captureHash)
				hashscript = "?' + window.location.hash.substring(1);";
			return Content("<script>window.location='" + endpoint + hashscript + "</script>", "text/html");
		}
	}

	/// <summary>
	/// Base class for the controllers
	/// Also used the NoCache attribute to prevent any attempt of caching the results
	/// </summary>
	[Authorize]
	public class ControllerBase : ControllerExtension
	{
		/// <summary>
		/// Accessor for the current navigation context
		/// </summary>
		protected NavigationContext Navigation
		{
			get
			{
				return UserContext.Current.CurrentNavigation;
			}
		}

		// TODO: Criar um ficheiro próprio !?
		protected class EventSink
		{
			public Dictionary<string, object> m_context = new();

			public Dictionary<string, object> Context { get { return m_context; } }

			public string MethodName { get; set; }

			public string ViewName { get; set; }

			public string FormName { get; set; }

			public string AreaName { get; set; }

			public NavigationLocation Location { get; set; }

			public bool Redirect { get; set; }

			public Action<EventSink, PersistentSupport> BeforeAll { get; set; }

			public Action<EventSink, PersistentSupport> BeforeOp { get; set; }

			public Action<EventSink, PersistentSupport> AfterOp { get; set; }

			public Action<EventSink, PersistentSupport> BeforeException { get; set; }

			public Action<EventSink, PersistentSupport> AfterException { get; set; }
		}

		protected ControllerBase(IUserContextService userContextService) : base(userContextService) { }

		/// <summary>
		/// Validates the provided ICrudViewModel and adds any validation errors to the ModelState.
		/// </summary>
		/// <param name="model">The ICrudViewModel to be validated.</param>
		protected void ValidateModel(ICrudViewModel model)
		{
			var validationResult = model.Validate();

			foreach (var (field, errorMessages) in validationResult.ModelErrors)
				foreach (var errorMessage in errorMessages)
					ModelState.AddModelError(field, errorMessage);
		}

		protected string HandleException(Exception e, string defaultMsg = null)
		{
			// JGF 2020.12.10 Added multi exception check for multiple write condition errors
			if (e is FieldValidationException fvExc)
			{
				foreach (var message in fvExc.StatusMessage.GetErrorList())
					ModelState.AddModelError(message.Origin, message.Message);

				return fvExc.UserMessage;
			}

			string exceptionUserMessage = defaultMsg ?? Resources.Resources.PEDIMOS_DESCULPA__OC63848;
			if (e is GenioException gExc && gExc.UserMessage != null)
				exceptionUserMessage = Translations.Get(gExc.UserMessage, UserContext.Current.User.Language);

			if(e is not GenioException)
				Log.Error(e.Message);

			ModelState.AddModelError("Erro", exceptionUserMessage);
			return exceptionUserMessage;
		}

		protected List<string> GetActionIds(CriteriaSet crs, CSGenio.persistence.PersistentSupport sp, CSGenio.business.Area area)
		{
			if (crs == null)
				return [];

			sp ??= UserContext.Current.PersistentSupport;

			// Fetch List of Related Areas
			List<string> ids = [];

			List<string> relatedTables = [];
			QueryUtils.checkConditionsForForeignTables(crs, area, relatedTables);
			List<CSGenio.framework.Relation> relations = QueryUtils.tablesRelationships(relatedTables, area);
			SelectQuery select = new SelectQuery()
				.Select(area.Alias, area.PrimaryKeyName)
				.From(area.Alias)
				.Where(crs);

			// Insert related area joins in query
			QueryUtils.setFromTabDirect(select, relations, area);

			// Fetch all the IDs
			DataMatrix dm = sp.Execute(select);
			for (int i = 0; i < dm.NumRows; i++)
				ids.Add(dm.GetString(i, 0));

			return ids;
		}

		/// <summary>
		/// Ensures the keys in the navigation belong to the current record
		/// </summary>
		/// <param name="id">The id of the record</param>
		/// <param name="area">The name of the area</param>
		protected void SanitizeHistoryEntries(string id, string area)
		{
			if (id != null && id != Navigation.GetStrValue(area))
				Navigation.CurrentLevel.ClearEntries();
		}

		private StatusMessage Validate(ICrudViewModel model, EventSink sink, FormMode mode, Func<StatusMessage> checkFormConds, string id = null, bool loadModel = true, bool validateModel = true)
		{
			var sp = UserContext.Current.PersistentSupport;
			bool connWasClosed = sp.ConnectionIsClosed;

			try
			{
				// Check table permissions.
				StatusMessage permission = model.CheckPermissions(mode);

				// If the user does not have basic permissions, we will not proceed with validations that position the record, which in turn can make multiple unnecessary requests to the database.
				if (permission.HasError)
					return permission;

				// Read the model from the database, ensuring that the read-only fields have not been changed in the ViewModel.
				// The validation of CRUD conditions should not and cannot trust on values coming from the interface.
				// In addition to making the values of the calculated fields valid, invoking the recalculation of the formulas after mapping also
				// allows for protecting the fields that could not be filled due to the Fill When condition, but came in the ViewModel with a value.
				if (loadModel)
				{
					model.LoadModel(id);

					if (validateModel)
					{
						// Recalculation of the formulas may need the connection to the database to be open (for example, in formulas that use functions that have SQL queries).
						// Therefore, we must open the connection before the 'executeModelFormulas' call.
						if (connWasClosed)
							sp.openConnection();

						model.MapToModel();
						model.ExecuteModelFormulas();

						if (connWasClosed)
							sp.closeConnection();
					}
				}

				// Check form conditions.
				permission.MergeStatusMessage(checkFormConds?.Invoke());

				if (permission.HasError)
					return permission;

				if (loadModel && validateModel)
				{
					ValidateModel(model);

					if (!ModelState.IsValid)
						throw new BusinessException(Resources.Resources.ERRO_AO_GUARDAR_O_RE65182, sink?.MethodName ?? "Validate", "The model isn't valid.");
				}

				return permission;
			}
			catch
			{
				if (connWasClosed)
					sp.closeConnection();
				throw;
			}
		}

		protected StatusMessage Validate(ICrudViewModel model, EventSink sink = null, string id = null, bool loadModel = true)
		{
			FormMode formMode = Navigation.CurrentLevel.FormMode;

			return formMode switch
			{
				FormMode.Show => ValidateView(model, sink, id),
				FormMode.Edit => ValidateEdit(model, sink, id),
				FormMode.Delete => ValidateDelete(model, sink, id),
				FormMode.Duplicate => ValidateDuplicate(model, sink, id, loadModel),
				FormMode.New => ValidateInsert(model, sink, id, loadModel),
				_ => throw new BusinessException(Resources.Resources.OCORREU_UM_ERRO_AO_P53091, "Validate", $"Unsupported form mode: {formMode}.")
			};
		}

		protected StatusMessage ValidateView(ICrudViewModel model, EventSink sink, string id = null)
		{
			return Validate(model, sink, FormMode.Show, model.ViewConditions, id, validateModel: false);
		}

		protected StatusMessage ValidateEdit(ICrudViewModel model, EventSink sink, string id = null)
		{
			return Validate(model, sink, FormMode.Edit, model.UpdateConditions, id);
		}

		protected StatusMessage ValidateDelete(ICrudViewModel model, EventSink sink, string id = null)
		{
			return Validate(model, sink, FormMode.Delete, model.DeleteConditions, id, validateModel: false);
		}

		protected StatusMessage ValidateDuplicate(ICrudViewModel model, EventSink sink, string id = null, bool loadModel = true)
		{
			return Validate(model, sink, FormMode.Duplicate, model.InsertConditions, id, loadModel);
		}

		protected StatusMessage ValidateInsert(ICrudViewModel model, EventSink sink, string id = null, bool loadModel = true)
		{
			return Validate(model, sink, FormMode.New, model.InsertConditions, id, loadModel);
		}

		protected JsonNetResult GenericHandleGetFormShow(EventSink sink, ICrudViewModel model, string id)
		{
			SanitizeHistoryEntries(id, sink.AreaName);

			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			sink.BeforeAll?.Invoke(sink, sp);

			model.setModes(Request.Query["m"]);

			StatusMessage permission = ValidateView(model, sink, id);
			if (permission.HasError)
				return PermissionError(permission.Message);

			CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.FORM54242 + " " + Navigation.CurrentLevel.Location.ShortDescription());

			Navigation.SetValue(sink.AreaName, id);

			//---------------------------------------------
			// USE /[MANUAL BEFORE_LOAD_SHOW]/
			sink.BeforeOp?.Invoke(sink, sp);
			//---------------------------------------------

			try
			{
				if (sink.AreaName == "glob")
				{
					model.LoadGlob(Request.QueryNameValues(), true, Request.IsAjaxRequest());
					Navigation.SetValue(sink.AreaName, model.QPrimaryKey);
				}
				else
					model.Load(Request.QueryNameValues(), true, Request.IsAjaxRequest(), true);
			}
			catch (ModelNotFoundException)
			{
				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}
			catch (Exception e)
			{
				CSGenio.framework.Log.Error(sink.MethodName + " - " + id + " " + e.Message);
				return InternalServerError();
			}

			//---------------------------------------------
			// USE /[MANUAL AFTER_LOAD_SHOW]/
			sink.AfterOp?.Invoke(sink, sp);
			//---------------------------------------------

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			return JsonOK(model);
		}

		protected JsonNetResult GenericHandleGetFormNew(EventSink sink, ICrudViewModel model, string id, bool isNewLocation, Dictionary<string, object> prefillValues = null)
		{
			SanitizeHistoryEntries(id, sink.AreaName);

			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			sink.BeforeAll?.Invoke(sink, sp);

			model.setModes(Request.Query["m"]);

			StatusMessage permission = ValidateInsert(model, sink, id, false);
			if (permission.HasError)
				return PermissionError(permission.Message);

			//FOR: OVERRIDE SKIP IF JUST ONE
			//Allow child form to use "Go Back" to menu list without "skip if only one"
			if (Navigation.OverrideSkipIfJustOne.ContainsKey(sink.FormName))
				Navigation.OverrideSkipIfJustOne[sink.FormName] = true;

			CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.FORM54242 + " " + Navigation.CurrentLevel.Location.ShortDescription());

			try
			{
				if (isNewLocation)
				{
					sp.openTransaction();
					model.New();
					sp.closeTransaction();

					Navigation.SetValue(sink.AreaName, model.QPrimaryKey);

					sp.openConnection();

					//---------------------------------------------
					// USE /[MANUAL BEFORE_LOAD_NEW]/
					sink.BeforeOp?.Invoke(sink, sp);
					//---------------------------------------------

					model.NewLoad();

					// FOR: PREFILL_FORM_VALUES
					// Set property values passed in
					model.PopulateViewModel(prefillValues);
					if (prefillValues?.Count > 0)
						model.MapToModel();

					//---------------------------------------------
					// USE /[MANUAL AFTER_LOAD_SHOW]/
					sink.AfterOp?.Invoke(sink, sp);
					//---------------------------------------------

					sp.closeConnection();
				}
				else
				{
					if (id != null)
						Navigation.SetValue(sink.AreaName, id);
					sp.openConnection();
					model.Load(Request.QueryNameValues(), true, Request.IsAjaxRequest());
					sp.closeConnection();
				}
			}
			catch (ModelNotFoundException)
			{
				sp.rollbackTransaction();
				sp.closeConnection();
				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				CSGenio.framework.Log.Error(sink.MethodName + " - " + e.Message);

				return JsonERROR(HandleException(e));
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			return JsonOK(model);
		}

		protected JsonNetResult GenericHandleGetFormEdit(EventSink sink, ICrudViewModel model, string id)
		{
			SanitizeHistoryEntries(id, sink.AreaName);

			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			sink.BeforeAll?.Invoke(sink, sp);

			model.setModes(Request.Query["m"]);

			// Check table permissions
			StatusMessage permission = model.CheckPermissions(FormMode.Edit);

			// If the user does not have basic permissions, we will not proceed with validations that position the record, which in turn can make multiple unnecessary requests to the database.
			if (permission.HasError)
				return PermissionError(permission.Message);

			CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.FORM54242 + " " + Navigation.CurrentLevel.Location.ShortDescription());

			Navigation.SetValue(sink.AreaName, id);

			//---------------------------------------------
			// USE /[MANUAL BEFORE_LOAD_EDIT]/
			sink.BeforeOp?.Invoke(sink, sp);
			//---------------------------------------------

			try
			{
				sp.openConnection();
				if (sink.AreaName == "glob")
				{
					model.LoadGlob(Request.QueryNameValues(), true, Request.IsAjaxRequest());
					Navigation.SetValue(sink.AreaName, model.QPrimaryKey);
				}
				else
					model.Load(Request.QueryNameValues(), true, Request.IsAjaxRequest(), true);

				sp.closeConnection();
			}
			catch (ModelNotFoundException)
			{
				sp.closeConnection();
				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}
			catch (Exception e)
			{
				sp.closeConnection();
				CSGenio.framework.Log.Error(sink.MethodName + " - " + id + " " + e.Message);
				return InternalServerError();
			}

			//---------------------------------------------
			// USE /[MANUAL AFTER_LOAD_EDIT]/
			sink.AfterOp?.Invoke(sink, sp);
			//---------------------------------------------

			// Check form conditions
			permission.MergeStatusMessage(model.UpdateConditions());

			if (permission.HasError)
				return PermissionError(permission.Message);

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			return JsonOK(model);
		}

		protected JsonNetResult GenericHandleGetFormDelete(EventSink sink, ICrudViewModel model, string id)
		{
			SanitizeHistoryEntries(id, sink.AreaName);

			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			sink.BeforeAll?.Invoke(sink, sp);

			model.setModes(Request.Query["m"]);

			// Check table permissions
			StatusMessage permission = model.CheckPermissions(FormMode.Delete);

			// If the user does not have basic permissions, we will not proceed with validations that position the record, which in turn can make multiple unnecessary requests to the database.
			if (permission.HasError)
				return PermissionError(permission.Message);

			CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.FORM54242 + " " + Navigation.CurrentLevel.Location.ShortDescription());

			Navigation.SetValue(sink.AreaName, id);

			//---------------------------------------------
			// USE /[MANUAL BEFORE_LOAD_DELETE]/
			sink.BeforeOp?.Invoke(sink, sp);
			//---------------------------------------------

			try
			{
				model.Load(Request.QueryNameValues(), false, Request.IsAjaxRequest(), true);
			}
			catch (ModelNotFoundException)
			{
				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}
			catch (Exception e)
			{
				CSGenio.framework.Log.Error(sink.MethodName + " - " + id + " " + e.Message);
				return InternalServerError();
			}

			//---------------------------------------------
			// USE /[MANUAL AFTER_LOAD_DELETE]/
			sink.AfterOp?.Invoke(sink, sp);
			//---------------------------------------------

			// Check form conditions
			permission.MergeStatusMessage(model.DeleteConditions());

			if (permission.HasError)
				return PermissionError(permission.Message);

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			return JsonOK(model);
		}

		protected JsonNetResult GenericHandleGetFormDuplicate(EventSink sink, ICrudViewModel model, string id, bool isNewLocation)
		{
			SanitizeHistoryEntries(id, sink.AreaName);

			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			sink.BeforeAll?.Invoke(sink, sp);

			model.setModes(Request.Query["m"]);

			StatusMessage permission = ValidateDuplicate(model, sink, id, false);
			if (permission.HasError)
				return PermissionError(permission.Message);

			CSGenio.framework.Audit.registAction(UserContext.Current.User, Resources.Resources.FORM54242 + " " + Navigation.CurrentLevel.Location.ShortDescription());

			try
			{
				if (isNewLocation)
				{
					sp.openTransaction();

					//---------------------------------------------
					// USE /[MANUAL BEFORE_LOAD_DUPLICATE]/
					sink.BeforeOp?.Invoke(sink, sp);
					//---------------------------------------------

					model.Duplicate(id);

					//---------------------------------------------
					// USE /[MANUAL AFTER_LOAD_DUPLICATE]/
					sink.AfterOp?.Invoke(sink, sp);
					//---------------------------------------------

					sp.closeTransaction();

					Navigation.SetValue(sink.AreaName, model.QPrimaryKey);
				}
				else
				{
					if (id != null)
						Navigation.SetValue(sink.AreaName, id);
					sp.openConnection();
					model.Load(Request.QueryNameValues(), true, Request.IsAjaxRequest());
					sp.closeConnection();
				}
			}
			catch (ModelNotFoundException)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				return NotFoundError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				return JsonERROR(HandleException(e));
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			return JsonOK(model);
		}

		protected ActionResult GenericHandlePostFormEdit(EventSink sink, ICrudViewModel model)
		{
			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			try
			{
				sink.BeforeAll?.Invoke(sink, sp);

				StatusMessage permission = ValidateEdit(model, sink);
				if (permission.HasError)
					return PermissionError(permission.Message);

				sp.openTransaction();

				//---------------------------------------------
				// USE /[MANUAL BEFORE_SAVE_EDIT]/
				sink.BeforeOp?.Invoke(sink, sp);
				//---------------------------------------------

				model.Save();

				//---------------------------------------------
				// Allow cancellation of operation if there are any warnings
				if(!model.CanSaveWithWarnings && model.flashMessage?.HasWarning == true)
				{
					sp.rollbackTransaction();
					sp.closeConnection();
					return Json(new { Success = false, Operation = "", Warnings = model.flashMessage.WarningMessages, currentNavigationLevel = Navigation.CurrentLevel.Level });
				}

				//---------------------------------------------
				// USE /[MANUAL AFTER_SAVE_EDIT]/
				sink.AfterOp?.Invoke(sink, sp);
				//---------------------------------------------

				if (Navigation.PreviousLevel != null)
				{
					// New insertion in upper table
					if (Navigation.PreviousLevel.FormMode != FormMode.List)
						Navigation.SetValue("RETURN_" + sink.AreaName, Navigation.GetValue(sink.AreaName), true);

					// Position the list in the current registry
					Navigation.SetValue("QMVC_POS_RECORD_" + sink.AreaName, Navigation.GetValue(sink.AreaName), true);
				}

				sp.closeTransaction();

				Navigation.SetValue("ForcePrimaryRead_" + sink.AreaName, "true", true);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				/*
					NOTE: Given that we are not sending back the ViewModel (it doesn't make sense to send it back, to be overwritten with what's in cache,
						as the user should not lose the data already filled just because a field has to have a different value),
						we no longer need the Before and After load. Instead, an OnException can be created.
				*/
				//---------------------------------------------
				// USE /[MANUAL BEFORE_LOAD_EDIT_EX]/
				sink.BeforeException?.Invoke(sink, sp);
				//---------------------------------------------

				//---------------------------------------------
				// USE /[MANUAL AFTER_LOAD_EDIT_EX]/
				sink.AfterException?.Invoke(sink, sp);
				//---------------------------------------------

				return JsonERROR(HandleException(e, Resources.Resources.ERRO_AO_GUARDAR_O_RE65182));
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			IList<string> warningMsgs = model.flashMessage?.WarningMessages ?? [];
			return Json(new { Success = true, Operation = "Edit", Message = Resources.Resources.ALTERACOES_EFETUADAS10166, Warnings = warningMsgs, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		protected ActionResult GenericHandlePostFormApply(EventSink sink, ICrudViewModel model)
		{
			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			try
			{
				sink.BeforeAll?.Invoke(sink, sp);

				StatusMessage permission = ValidateEdit(model, sink);
				if (permission.HasError)
					return PermissionError(permission.Message);

				sp.openTransaction();

				//---------------------------------------------
				// USE /[MANUAL BEFORE_APPLY_EDIT]/
				sink.BeforeOp?.Invoke(sink, sp);
				//---------------------------------------------

				model.Apply();

				//---------------------------------------------
				// USE /[MANUAL AFTER_APPLY_EDIT]/
				sink.AfterOp?.Invoke(sink, sp);
				//---------------------------------------------

				sp.closeTransaction();

				if (CSGenio.framework.Log.IsDebugEnabled)
					CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				return JsonERROR(HandleException(e));
			}

			return Json(new { Success = true, Operation = "Apply", Message = Resources.Resources.ALTERACOES_EFETUADAS10166 });
		}

		protected ActionResult GenericHandlePostFormDelete(EventSink sink, ICrudViewModel model)
		{
			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			try
			{
				sink.BeforeAll?.Invoke(sink, sp);

				StatusMessage permission = ValidateDelete(model, sink);
				if (permission.HasError)
					return PermissionError(permission.Message);

				sp.openTransaction();

				//---------------------------------------------
				// USE /[MANUAL BEFORE_DESTROY_DELETE]/
				sink.BeforeOp?.Invoke(sink, sp);
				//---------------------------------------------

				model.Destroy();

				//---------------------------------------------
				// USE /[MANUAL AFTER_DESTROY_DELETE]/
				sink.AfterOp?.Invoke(sink, sp);
				//---------------------------------------------

				sp.closeTransaction();

				Navigation.SetValue("PreviouslyRemovedRowKey_" + sink.AreaName, model.QPrimaryKey, true);
				Navigation.SetValue("ForcePrimaryRead_" + sink.AreaName, "true", true);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				return JsonERROR(HandleException(e));
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			return Json(new { Success = true, Operation = "Delete", Message = Resources.Resources.REGISTO_APAGADO_COM_64671, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		protected ActionResult GenericHandlePostFormDuplicate(EventSink sink, ICrudViewModel model)
		{
			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			try
			{
				sink.BeforeAll?.Invoke(sink, sp);

				StatusMessage permission = ValidateDuplicate(model, sink);
				if (permission.HasError)
					return PermissionError(permission.Message);

				sp.openTransaction();

				//---------------------------------------------
				// USE /[MANUAL BEFORE_SAVE_DUPLICATE]/
				sink.BeforeOp?.Invoke(sink, sp);
				//---------------------------------------------

				model.Save();

				//---------------------------------------------
				// Allow cancellation of operation if there are any warnings
				if(!model.CanSaveWithWarnings && model.flashMessage?.HasWarning == true)
				{
					sp.rollbackTransaction();
					sp.closeConnection();
					return Json(new { Success = false, Operation = "Dup", Warnings = model.flashMessage.WarningMessages, currentNavigationLevel = Navigation.CurrentLevel.Level });
				}

				//---------------------------------------------
				// USE /[MANUAL AFTER_SAVE_DUPLICATE]/
				sink.AfterOp?.Invoke(sink, sp);
				//---------------------------------------------

				sp.closeTransaction();

				if (Navigation.PreviousLevel != null)
				{
					// Position the list in the current registry
					Navigation.SetValue("QMVC_POS_RECORD_" + sink.AreaName, Navigation.GetValue(sink.AreaName), true);
				}
				Navigation.SetValue("ForcePrimaryRead_" + sink.AreaName, "true", true);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				/*
					NOTE: Given that we are not sending back the ViewModel (it doesn't make sense to send it back, to be overwritten with what's in cache,
						as the user should not lose the data already filled just because a field has to have a different value),
						we no longer need the Before and After load. Instead, an OnException can be created.
				*/
				//---------------------------------------------
				// USE /[MANUAL BEFORE_LOAD_DUPLICATE_EX]/
				sink.BeforeException?.Invoke(sink, sp);
				//---------------------------------------------

				//---------------------------------------------
				// USE /[MANUAL AFTER_LOAD_DUPLICATE_EX]/
				sink.AfterException?.Invoke(sink, sp);
				//---------------------------------------------

				return JsonERROR(HandleException(e));
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			IList<string> warningMsgs = model.flashMessage?.WarningMessages ?? [];
			return Json(new { Success = true, Operation = "Dup", Message = Resources.Resources.REGISTO_CRIADO_COM_S18746, Warnings = warningMsgs, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		protected ActionResult GenericHandlePostFormNew(EventSink sink, ICrudViewModel model)
		{
			long st = DateTime.Now.Ticks;
			var sp = UserContext.Current.PersistentSupport;

			try
			{
				sink.BeforeAll?.Invoke(sink, sp);

				StatusMessage permission = ValidateInsert(model, sink);
				if (permission.HasError)
					return PermissionError(permission.Message);

				sp.openTransaction();

				//---------------------------------------------
				// USE /[MANUAL BEFORE_SAVE_NEW]/
				sink.BeforeOp?.Invoke(sink, sp);
				//---------------------------------------------

				model.Save();

				//---------------------------------------------
				// Allow cancellation of operation if there are any warnings
				if(!model.CanSaveWithWarnings && model.flashMessage?.HasWarning == true)
				{
					sp.rollbackTransaction();
					sp.closeConnection();
					return Json(new { Success = false, Operation = "New", Warnings = model.flashMessage.WarningMessages, currentNavigationLevel = Navigation.CurrentLevel.Level });
				}

				//---------------------------------------------
				// USE /[MANUAL AFTER_SAVE_NEW]/
				sink.AfterOp?.Invoke(sink, sp);
				//---------------------------------------------

				sp.closeTransaction();

				if (Navigation.PreviousLevel != null)
				{
					// New insertion in upper table
					if (Navigation.PreviousLevel.FormMode != FormMode.List)
						Navigation.SetValue("RETURN_" + sink.AreaName, Navigation.GetValue(sink.AreaName), true);

					// Position the list in the current registry
					Navigation.SetValue("QMVC_POS_RECORD_" + sink.AreaName, Navigation.GetValue(sink.AreaName), true);
				}
				Navigation.SetValue("ForcePrimaryRead_" + sink.AreaName, "true", true);
			}
			catch (Exception e)
			{
				sp.rollbackTransaction();
				sp.closeConnection();

				/*
					NOTE: Given that we are not sending back the ViewModel (it doesn't make sense to send it back, to be overwritten with what's in cache,
						as the user should not lose the data already filled just because a field has to have a different value),
						we no longer need the Before and After load. Instead, an OnException can be created.
				*/
				//---------------------------------------------
				// USE /[MANUAL BEFORE_LOAD_NEW_EX]/
				sink.BeforeException?.Invoke(sink, sp);
				//---------------------------------------------

				//---------------------------------------------
				// USE /[MANUAL AFTER_LOAD_NEW_EX]/
				sink.AfterException?.Invoke(sink, sp);
				//---------------------------------------------

				return JsonERROR(HandleException(e, Resources.Resources.ERRO_AO_GUARDAR_O_RE65182));
			}

			if (CSGenio.framework.Log.IsDebugEnabled)
				CSGenio.framework.Log.Debug("Controller success " + (DateTime.Now.Ticks - st) / TimeSpan.TicksPerMillisecond + "ms");

			IList<string> warningMsgs = model.flashMessage?.WarningMessages ?? [];
			return Json(new { Success = true, Operation = "New", Message = Resources.Resources.REGISTO_CRIADO_COM_S18746, Warnings = warningMsgs, currentNavigationLevel = Navigation.CurrentLevel.Level });
		}

		/// <summary>
		/// Builds a RouteValueDictionary with the current route values and additional params
		/// </summary>
		/// <param name="location">The action to redirect to</param>
		/// <param name="additionalRouteValues">Additional Route data</param>
		/// <returns>The redirect result object</returns>
		protected RouteValueDictionary GetRouteValues(NavigationLocation location, object additionalRouteValues = null)
		{
			var values = new RouteValueDictionary(location.RoutedValues);

			if (additionalRouteValues != null)
			{
				var arv = new RouteValueDictionary(additionalRouteValues);

				foreach (var kv in arv)
					if (!values.ContainsKey(kv.Key))
						values.Add(kv.Key, kv.Value);
			}

			return values;
		}

		/// <summary>
		/// Redirects to the action specified in the location
		/// </summary>
		/// <param name="location">The action to redirect to</param>
		/// <param name="additionalRouteValues">Additional Route data</param>
		/// <returns>The redirect result object</returns>
		protected JsonNetResult RedirectToLocation(NavigationLocation location, object additionalRouteValues = null)
		{
			var values = GetRouteValues(location, additionalRouteValues);

			if (!string.IsNullOrEmpty(location.mode) && !values.ContainsKey("mode"))
				values.Add("mode", location.mode);
			return RedirectToVueRoute(location.vueRouteName, values);
		}

		/// <summary>
		/// Redirects to the location based on the form menu's GoBack value.
		/// </summary>
		/// <param name="FormName">The name of the form to get the redirect action of</param>
		/// <returns>The redirect result object</returns>
		/// <remarks>FOR: FORM MENU GO BACK</remarks>
		protected JsonNetResult RedirectToFormMenuGoBack(string FormName)
		{
			return RedirectToLocation(Navigation.CurrentLevel.Location);
		}

		protected bool IsNewLocation(NavigationLocation location)
		{
			return !location.IsSameAction(Navigation.CurrentLevel.Location);
		}

		#region Images

		/// <summary>
		/// Obtains the byte[] image from the corresponding model
		/// </summary>
		/// <param name="ticket">The Resource Query ticket</param>
		/// <param name="formIdentifier">Form Identifier</param>
		/// <param name="height">The image height</param>
		/// <param name="width">The image width</param>
		/// <returns>The image data</returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult GetImage(string ticket, string formIdentifier, int height = -1, int width = -1)
		{
			// If a height and width aren't specified, the original dimensions of the image will be used.
			var sp = m_userContext.PersistentSupport;
			try
			{
				// NOTE: Error messages will not be returned to the user to prevent brute force attacks, as the action is open to the unauthenticated user.
				ArgumentException.ThrowIfNullOrEmpty(ticket, nameof(ticket));

				ResourceQuery resource = GetResourceQueryFromTicket(ticket);

				ArgumentException.ThrowIfNullOrEmpty(resource.KeyValue, nameof(resource.KeyValue));
				ArgumentException.ThrowIfNullOrEmpty(resource.Table, nameof(resource.Table));
				ArgumentException.ThrowIfNullOrEmpty(resource.KeyData, nameof(resource.KeyData));

				var user = m_userContext.User;
				var area = CSGenio.business.Area.createArea(resource.Table.ToLowerInvariant(), user, user.CurrentModule)
					?? throw new BusinessException("Incorrect request.", "GetImage", $"The '{resource.Table}' area does not exist.");

				if (!area.DBFields.ContainsKey(resource.KeyData))
					throw new BusinessException("Incorrect request.", "GetImage", $"Requested field '{resource.KeyData}' does not exist in '{resource.Table}' area.");

				if (!area.AccessRightsToConsult())
					throw new BusinessException("Permissions error.", "GetImage", "Permissions error.");

				// The «returnField» could be used, but we need to apply EPHs to limit users who have access to data (if applied)
				var condition = CriteriaSet.And()
						.Equal(area.Alias, area.PrimaryKeyName, resource.KeyValue);
				// Just for security, apply Permanent History Entries
				var criteriaSetPHE = Listing.CalculateConditionsEphGeneric(area, formIdentifier);
				condition.SubSet(criteriaSetPHE);

				SelectQuery query = new SelectQuery()
					.Select(area.Alias, resource.KeyData)
					.From(area.QSystem, area.TableName, area.Alias)
					.Where(condition);

				sp.openConnection();
				var mx = sp.Execute(query);
				if (mx == null || mx.NumRows < 1)
					throw new BusinessException("Record not found.", "GetImage", "The requested record does not exist or the user does not have access due to limitation by PHE.");

				byte[]? image = mx.GetBinary(0, 0);

				if (image?.Length > 0)
				{
					string imageFormat = ImageResizer.GetImageFormat(image);
					bool isThumbnail = false;

					if (height > 0 && width > 0)
					{
						image = ImageResizer.ResizeImage(image, width, height, true);
						isThumbnail = true;
					}

					ImageModel imageModel = new(image)
					{
						Data = System.Convert.ToBase64String(image),
						DataFormat = imageFormat,
						FileName = "", // TODO: Save the file name and format.
						IsThumbnail = isThumbnail,
						Ticket = ticket
					};

					return JsonOK(imageModel);
				}
			}
			catch (Exception e)
			{
				if (e is not GenioException)
					Log.Error("Error on GetImage - " + e.Message);
				return JsonERROR();
			}
			finally
			{
				sp.closeConnection();
			}

			return JsonOK();
		}

		#endregion

		/// <summary>
		/// Calls the server-side method to convert a given string to a QR code representation
		/// </summary>
		/// <param name="text">The string to convert</param>
		/// <returns>A byte array representing the result of the convertion</returns>
		[ActionName("StringToQRcode")]
		[HttpGet]
		public JsonNetResult StringToQRcode(string text)
		{
			byte[] bytes = GlobalFunctions.StringToQRcode(text);

			if (bytes != null)
				return JsonOK(new { value = Convert.ToBase64String(bytes) });
			return JsonOK(new { value = String.Empty });
		}

		/// <summary>
		/// Obtains an image from disk
		/// </summary>
		/// <param name="s">The Path</param>
		/// <returns>The image</returns>
		protected byte[] GetFile(string s)
		{
			System.IO.FileStream fs = System.IO.File.OpenRead(s);
			byte[] data = new byte[fs.Length];
			int br = fs.Read(data, 0, data.Length);
			if (br != fs.Length)
				throw new System.IO.IOException(s);

			fs.Close();
			fs.Dispose();

			return data;
		}

		/// <summary>
		/// Updates the given tableNN below with the selected values of table B for the given key of table A
		/// </summary>
		/// <param name="current_navigation">History</param>
		/// <param name="table">Table A</param>
		/// <param name="key">The key of a row in table A</param>
		/// <param name="tableNN">The table that makes a N-N relation between A and B</param>
		/// <param name="primaryField">The field name for primaryKey in table A</param>
		/// <param name="otherField">The field name for primaryKey in table B</param>
		/// <param name="selectedIds">The selected keys in table B</param>
		protected void MergeNN(NavigationContext current_navigation, string table, string key, string tableNN, string primaryField, string otherField, string[] selectedIds)
		{
			selectedIds ??= [];

			// Creating the CriteriaSet
			AreaInfo info = CSGenio.business.Area.GetInfoArea(tableNN?.ToLower());
			CriteriaSet criteriaSetAnd = CriteriaSet.And().Equal(info.Alias, primaryField?.ToLower(), key);

			//Call the AllModel for reflection.
			//This code could avoid reflection if its changed to be a generic method and call the generic ModelBase.Where<T> instead.
			Type type = Type.GetType("GenioMVC.Models." + tableNN)!;
			MethodInfo allModelMI = type.GetMethod("AllModel", [typeof(UserContext), typeof(CriteriaSet), typeof(string)])!;
			IEnumerable previous = (IEnumerable)allModelMI.Invoke(null, [UserContext.Current, criteriaSetAnd, null])!;

			// Updates the table NN by removing the rows that were not selected this time
			HashSet<string> previousSelected = [];
			foreach (ModelBase row in previous)
			{
				string otherKey = row.GetValueGeneric("Val" + otherField) as string;
				previousSelected.Add(otherKey);

				if (!selectedIds.Contains(otherKey))
					row.Destroy();
			}

			// Updates the table NN by adding the new rows that were selected this time
			foreach (var id in selectedIds)
			{
				if (!previousSelected.Contains(id))
				{
					// create
					ModelBase row = (ModelBase)Activator.CreateInstance(type, [UserContext.Current, false, null])!;
					row.SetValueGeneric("Val" + primaryField, key);
					row.SetValueGeneric("Val" + otherField, id);
					row.New();
					row.Save();
				}
			}
		}

		#region Documents

		/// <summary>
		/// Creates tickets that can be used by the client-side to handle the specified documents
		/// </summary>
		/// <param name="tableName">The name of the table</param>
		/// <param name="fieldName">The name of the field in the view model</param>
		/// <param name="keyValue">The primary key value</param>
		/// <returns>A JSON with the list of ticket keys</returns>
		[NonAction]
		protected ActionResult GetDocumsTickets(string tableName, string fieldName, string keyValue)
		{
			try
			{
				User user = m_userContext.User;
				ModelBase model = ModelBase.FindGeneric(tableName, keyValue, m_userContext, "");
				GenioMVC.ViewModels.DocumsProperties_ViewModel properties = model?.GetInfoDoc(fieldName);
				List<object> tickets = [];

				if (model != null)
				{
					SortedList<string, string> versions = properties.Versions;
					string docName = properties.Name;

					// All the versions of the file.
					if (versions?.Count > 0)
					{
						string areaName = "docums";
						string keyName = "ValCoddocums";

						foreach (KeyValuePair<string, string> version in versions)
						{
							ResourceQuery versionResource = new(version.Key, areaName, fieldName, keyName, version.Value);
							string versionTicket = QResources.CreateTicketEncryptedBase64(user.Name, user.Location, versionResource, false);
							tickets.Add(new { id = version.Key, ticket = versionTicket });
						}
					}

					// The current version of the file.
					ResourceQuery resource = new(docName ?? "", tableName, fieldName, "", keyValue);
					string ticket = QResources.CreateTicketEncryptedBase64(user.Name, user.Location, resource, false);
					tickets.Add(new { id = "main", ticket });
				}

				return JsonOK(new { tickets, properties });
			}
			catch
			{
				return JsonERROR();
			}
		}

		private bool CanSetFile(ICrudViewModel viewModel, bool isApply = false)
		{
			try
			{
				if (viewModel != null)
				{
					StatusMessage validations = StatusMessage.GetAggregator();
					validations.MergeStatusMessage(Validate(viewModel));
					validations.MergeStatusMessage(viewModel.EvaluateWriteConditions(isApply));
					validations.MergeStatusMessage(viewModel.Validate(isApply));

					return !validations.HasError;
				}

				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Checks if the model is valid and, if so, updates the specified tickets with write permissions
		/// </summary>
		/// <param name="documTickets">A list with the document tickets</param>
		/// <param name="viewModel">The view model of the current form</param>
		/// <param name="isApply">Whether an apply is being performed</param>
		/// <returns>A JSON response with the result of the operation</returns>
		[NonAction]
		protected ActionResult UpdateFilesTickets(List<RequestDocumFieldTicket> documTickets, ICrudViewModel viewModel, bool isApply = false)
		{
			try
			{
				if (!CanSetFile(viewModel, isApply))
					return JsonERROR();

				List<RequestDocumFieldTicket> tickets = [];

				foreach (var ticketInfo in documTickets)
				{
					DocumTicketProperties ticketProps = GetPropertiesFromTicket(ticketInfo.Ticket);

					// If the ticket is already writable, there's no need to do anything.
					if (ticketProps == null || ticketProps.IsWritable)
						continue;

					ResourceQuery resource = ticketProps.Resource as ResourceQuery;

					tickets.Add(new RequestDocumFieldTicket()
					{
						FieldId = ticketInfo.FieldId,
						Ticket = QResources.CreateTicketEncryptedBase64(ticketProps.Username, ticketProps.Location, resource)
					});
				}

				return JsonOK(new { tickets });
			}
			catch
			{
				return JsonERROR();
			}
		}

		/// <summary>
		/// Gets the file versions associated to the specified ticket
		/// </summary>
		/// <param name="ticket">Encryted ticket</param>
		/// <returns>A JSON with all the file versions</returns>
		[NonAction]
		protected ActionResult GetFileVersions(string ticket)
		{
			try
			{
				ResourceQuery recq = GetResourceQueryFromTicket(ticket);

				if (recq == null)
					return PermissionError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);

				var model = ModelBase.FindGeneric(recq.Table, recq.KeyValue, m_userContext, "");
				string? docfk = model.GetValueGeneric(recq.KeyData + "fk") as string;

				NameValueCollection values = [];
				GenioMVC.ViewModels.DocumsVersionsDBEdit_ViewModel documVersions = new(m_userContext, ticket, docfk, recq.Table, recq.KeyData);
				documVersions.Load(Configuration.NrRegDBedit == 0 ? 10 : Configuration.NrRegDBedit, values);

				return JsonOK(documVersions);
			}
			catch
			{
				return JsonERROR();
			}
		}

		/// <summary>
		/// Gets the file properties associated to the specified ticket
		/// </summary>
		/// <param name="ticket">Encryted ticket</param>
		/// <returns>A JSON with the file properties</returns>
		[NonAction]
		protected ActionResult GetFileProperties(string ticket)
		{
			try
			{
				ResourceQuery recq = GetResourceQueryFromTicket(ticket);

				if (recq == null)
					return PermissionError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);

				GenioMVC.ViewModels.DocumsProperties_ViewModel properties;

				if (recq.Table.Equals("DOCUMS", StringComparison.CurrentCultureIgnoreCase))
				{
					GenioMVC.Models.Docums model = GenioMVC.Models.Docums.Find(recq.KeyValue, m_userContext);
					CSGenio.business.DBFile file = model.klass.infoDocum(m_userContext.PersistentSupport, "document", model.ValCoddocums, false);
					properties = new GenioMVC.ViewModels.DocumsProperties_ViewModel(m_userContext, file);
				}
				else
				{
					ModelBase model = ModelBase.FindGeneric(recq.Table, recq.KeyValue, m_userContext, "");
					properties = model.GetInfoDoc(recq.KeyData);
				}

				return JsonOK(properties);
			}
			catch
			{
				return JsonERROR();
			}
		}

		/// <summary>
		/// Performs changes to the specified documents, either to delete or put them in editing state
		/// </summary>
		/// <param name="documents">A list of the changes to perform to each document</param>
		/// <returns>A JSON response with the result of the operation</returns>
		[NonAction]
		protected ActionResult SetFilesState(List<RequestDocumChangeModel> documents)
		{
			try
			{
				foreach (RequestDocumChangeModel docum in documents)
				{
					if (!IsTicketWritable(docum.Ticket))
						continue;

					ResourceQuery recq = GetResourceQueryFromTicket(docum.Ticket);

					if (recq == null)
						return PermissionError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);

					var model = ModelBase.FindGeneric(recq.Table, recq.KeyValue, m_userContext, "");
					GenioMVC.ViewModels.DocumsProperties_ViewModel properties = model.GetInfoDoc(recq.KeyData);

					bool successfulOp = true;

					if (docum.Delete)
					{
						bool success = true;

						success = docum.DeleteType switch
						{
							VersionDeleteAction.LastVersion => model.DeleteLastVersion(recq.KeyData),
							VersionDeleteAction.Historic => model.DeleteHistoricVersions(recq.KeyData, docum.CurrentVersion),
							VersionDeleteAction.All => model.DeleteDocument(recq.KeyData),
							_ => throw new Exception("Mode '" + docum.DeleteType + "' not supported!")
						};

						if (!success)
							successfulOp = false;
					}

					properties = model.GetInfoDoc(recq.KeyData);

					if (docum.Editing != properties.Editing)
					{
						bool success;

						if (docum.Editing)
							success = model.CheckoutVersion(recq.KeyData);
						else
							success = model.SubmitVersion(recq.KeyData, null, null, properties.VersionId, "DESBL", properties.Version);

						if (!success)
							successfulOp = false;
					}

					if (!successfulOp)
						return JsonERROR();
				}

				return JsonOK();
			}
			catch
			{
				return JsonERROR();
			}
		}

		/// <summary>
		/// Adds a new document (IB or ID)
		/// </summary>
		/// <param name="ticket">Encryted ticket</param>
		/// <param name="mode">Submit file action mode</param>
		/// <param name="version">The document version</param>
		/// <param name="extensions">A collection with the allowed extensions</param>
		/// <returns>A JSON response with the result of the operation</returns>
		[NonAction]
		protected ActionResult SetFile(string ticket, VersionSubmitAction mode = VersionSubmitAction.Insert, string version = "1", ICollection<string> extensions = null)
		{
			try
			{
				// In case of fail here, we return an ok response, since the form save/apply request will handle the errors.
				if (!IsTicketWritable(ticket))
					return JsonOK(new { validModel = false });

				ResourceQuery recq = GetResourceQueryFromTicket(ticket);

				if (recq == null)
					return PermissionError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);

				ModelBase model = ModelBase.FindGeneric(recq.Table, recq.KeyValue, m_userContext, "");

				CSGenio.business.DBFile file = null;
				string contentRangeHeader = Request.Headers.ContentRange;

				// Check if this is a chunked upload.
				if (string.IsNullOrEmpty(contentRangeHeader) && mode != VersionSubmitAction.UnlockFile)
				{
					// Not a chunked upload.
					file = GetFileFromRequest(recq.KeyData + "_file", version);

					if (file == null)
						return JsonOK(new { success = false, message = Resources.Resources.FICHEIRO_VAZIO18253 });
				}
				else if (mode != VersionSubmitAction.UnlockFile)
				{
					// Parse the content range header to determine
					// the range of bytes in the current chunk.
					string[] contentRangeParts = contentRangeHeader.Split('/');
					string[] byteRangeParts = contentRangeParts[0].Split('-');
					int startByte = int.Parse(byteRangeParts[0].Replace("bytes ", ""));
					int endByte = int.Parse(byteRangeParts[1]);

					var f = Request.Form.Files[recq.KeyData + "_file"];

					byte[] chunk = StreamToByteArray(f.OpenReadStream(), (int)f.Length);

					// Get the content of any previous chunks from in-memory cache.
					List<byte[]> parts = (List<byte[]>)QCache.Instance.FileUpload.Get(ticket);
					parts ??= [];
					// Combine them with the current chunk.
					parts.Add(chunk);

					// Check if this is the last chunk.
					int totalBytes = int.Parse(contentRangeParts[1]);
					bool isLastChunk = endByte == totalBytes - 1;

					if (isLastChunk)
					{
						byte[] part = JoinByteArrays(parts);

						file = new(
							Path.GetFileName(f.FileName),
							Path.GetExtension(f.FileName).Replace(".", ""),
							version,
							part,
							totalBytes);

						// Remove the temporary partial file.
						QCache.Instance.FileUpload.Invalidate(ticket);
					}
					else
					{
						// Put the partial file into the in-memory cache.
						QCache.Instance.FileUpload.Put(ticket, parts);
						// As long as there is no "success": true/false, it is just a progress response.
						return JsonOK(new { message = "Chunk processed successfully.", startByte, endByte });
					}
				}

				// Ensure the provided file has an allowed extension.
				if (extensions != null &&
					extensions.Count > 0 &&
					!extensions.Select(e => e.ToLower()).Contains(file.Extension.ToLower()))
					return JsonERROR($"{Resources.Resources.EXTENSAO_INVALIDA__E46375} {string.Join(", ", extensions)}.");

				GenioMVC.ViewModels.DocumsProperties_ViewModel properties = model.GetInfoDoc(recq.KeyData);

				if (version != "1")
				{
					string username = GetUsernameFromTicket(ticket);

					if (!properties.Editing)
					{
						model.CheckoutVersion(recq.KeyData);
						properties = model.GetInfoDoc(recq.KeyData);
					}
					else if (properties.Editor != username)
						throw new Exception($"User that checked out this file {properties.Editor} is not the same as the current user");
				}

				// IB type
				bool success = false;

				switch (mode)
				{
					case VersionSubmitAction.Insert:
						success = model.SaveDocument(recq.KeyData, file);
						break;
					case VersionSubmitAction.Submit:
					case VersionSubmitAction.UnlockFile:
						string saveMode = mode == VersionSubmitAction.Submit ? "SUBM" : "DESBL";
						byte[] bytes = file?.File;
						string fName = file?.Name;

						success = model.SubmitVersion(recq.KeyData, bytes, fName, properties.VersionId, saveMode, version);
						break;
					default:
						throw new Exception("Mode '" + mode + "' not supported!");
				}

				if (!success)
					return JsonOK(new { success = false });

				// Needs to update the properties with the info of the newly saved file.
				properties = model.GetInfoDoc(recq.KeyData);
				return JsonOK(new { success = true, message = Resources.Resources.ALTERACOES_EFETUADAS10166, properties });
			}
			catch
			{
				return JsonOK(new { success = false });
			}
		}

		/// <summary>
		/// Download a document (IB or ID)
		/// </summary>
		/// <param name="ticket">The resource ticket</param>
		/// <param name="viewType">DocumentViewTypeMode type that defines if it is a download or a preview</param>
		/// <returns>A document</returns>
		[NonAction]
		protected ActionResult GetFile(string ticket, DocumentViewTypeMode viewType = DocumentViewTypeMode.Print)
		{
			try
			{
				ResourceQuery recq = GetResourceQueryFromTicket(ticket);

				if (recq == null)
					return PermissionError(Resources.Resources.O_REGISTO_PEDIDO_NAO63869);

				bool isFromDocums = recq.Table.Equals("DOCUMS", StringComparison.CurrentCultureIgnoreCase);
				string modelName = (string)RouteData.Values["controller"];
				string modelKey = recq.KeyValue;

				if (isFromDocums)
				{
					GenioMVC.Models.Docums documModel = GenioMVC.Models.Docums.Find(recq.KeyValue, m_userContext);
					modelKey = documModel.ValModelkey;
					if (!string.IsNullOrEmpty(modelKey) && Guid.TryParse(modelKey, out Guid guidKey))
						modelKey = guidKey.ToString();
				}

				ModelBase model = ModelBase.FindGeneric(modelName, modelKey, m_userContext, "");

				CSGenio.business.DBFile file;

				if (isFromDocums)
					file = DbArea.getFileDB(recq.KeyValue, m_userContext.PersistentSupport);
				else
					file = model.FindDocument(recq.KeyData);

				string fileName = file.Name;
				byte[] document = file.File;

				string contentType = "application/octet-stream";

				if (viewType == DocumentViewTypeMode.Preview)
				{
					new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider().TryGetContentType(fileName, out string previewContentType);

					if (!string.IsNullOrWhiteSpace(previewContentType))
					{
						Response.Headers.ContentDisposition = "inline";
						contentType = previewContentType;
					}
				}

				return File(document, contentType, "\"" + fileName + "\"");
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error("GetFile Error: " + ex.Message);
				return JsonERROR();
			}
		}

		/// <summary>
		/// Aux method to get file from httpRequest
		/// </summary>
		/// <param name="request">request</param>
		/// <param name="fldname">document field</param>
		/// <returns>DBFile</returns>
		[NonAction]
		protected CSGenio.business.DBFile GetFileFromRequest(string fldname, string version)
		{
			CSGenio.business.DBFile dbfile = null;

			try
			{
				var file = Request.Form.Files[fldname];

				dbfile = new(
					Path.GetFileName(file.FileName),
					Path.GetExtension(file.FileName).Replace(".", ""),
					version,
					StreamToByteArray(file.OpenReadStream(), (int)file.Length),
					(int)file.Length);
			}
			catch { }

			return dbfile;
		}

		/// <summary>
		/// Gets the properties of the specified ticket
		/// </summary>
		/// <param name="ticket">The ticket</param>
		/// <returns>The ticket properties, or null if something is wrong with the specified ticket</returns>
		protected DocumTicketProperties GetPropertiesFromTicket(string ticket)
		{
			if (string.IsNullOrEmpty(ticket))
				return null;

			object[] objs = QResources.DecryptTicketBase64(ticket);
			string username = objs[0] as string;

			// Validates that the ticket was emitted for the current user.
			if (username != m_userContext.User.Name)
				return null;

			return new DocumTicketProperties
			{
				Username = username,
				Location = objs[1] as string,
				Resource = objs[2] as Resource,
				IsWritable = (bool)objs[3]
			};
		}

		/// <summary>
		/// Gets the username associated to the specified ticket
		/// </summary>
		/// <param name="ticket">The ticket</param>
		/// <returns>The username</returns>
		protected string GetUsernameFromTicket(string ticket)
		{
			DocumTicketProperties ticketProps = GetPropertiesFromTicket(ticket);
			return ticketProps?.Username ?? "";
		}

		/// <summary>
		/// Gets the location associated to the specified ticket
		/// </summary>
		/// <param name="ticket">The ticket</param>
		/// <returns>The location</returns>
		protected string GetLocationFromTicket(string ticket)
		{
			DocumTicketProperties ticketProps = GetPropertiesFromTicket(ticket);
			return ticketProps?.Location ?? "";
		}

		/// <summary>
		/// Gets the resource query associated to the specified ticket
		/// </summary>
		/// <param name="ticket">The ticket</param>
		/// <returns>The resource, or null if the user doesn't have permission to access it</returns>
		protected ResourceQuery GetResourceQueryFromTicket(string ticket)
		{
			DocumTicketProperties ticketProps = GetPropertiesFromTicket(ticket);
			Resource rec = ticketProps?.Resource;

			if (rec is ResourceQuery)
				return rec as ResourceQuery;

			throw new BusinessException(Resources.Resources.OCORREU_UM_ERRO_AO_P53091, "GetResourceQueryFromTicket", "Resource wasn't of type ResourceQuery.");
		}

		/// <summary>
		/// Gets the resource file associated to the specified ticket
		/// </summary>
		/// <param name="ticket">The ticket</param>
		/// <returns>The resource, or null if the user doesn't have permission to access it</returns>
		protected ResourceFile GetResourceFileFromTicket(string ticket)
		{
			DocumTicketProperties ticketProps = GetPropertiesFromTicket(ticket);
			Resource rec = ticketProps?.Resource;

			if (rec is ResourceFile)
				return rec as ResourceFile;

			throw new BusinessException(Resources.Resources.OCORREU_UM_ERRO_AO_P53091, "GetResourceFileFromTicket", "Resource wasn't of type ResourceFile.");
		}

		/// <summary>
		/// Checks if the specified ticket allows write operations
		/// </summary>
		/// <param name="ticket">The ticket</param>
		/// <returns>True if it does, false otherwise</returns>
		protected bool IsTicketWritable(string ticket)
		{
			DocumTicketProperties ticketProps = GetPropertiesFromTicket(ticket);
			return ticketProps?.IsWritable ?? false;
		}

		/// <summary>
		/// Stream to byte[]
		/// </summary>
		/// <param name="input">Stream object</param>
		/// <param name="capacity">The initial size of the internal array in bytes</param>
		/// <returns>byte[]</returns>
		private static byte[] StreamToByteArray(Stream input, int capacity)
		{
			using MemoryStream ms = new(capacity);
			input.CopyTo(ms);
			return ms.ToArray();
		}

		/// <summary>
		/// Joins multiple byte arrays into a single byte array.
		/// </summary>
		/// <param name="parts">A list of byte arrays to be joined.</param>
		/// <returns>A single byte array containing the concatenated elements of the input byte arrays.</returns>
		private static byte[] JoinByteArrays(List<byte[]> parts)
		{
			// Check if the list contains only one element and return it directly to avoid unnecessary processing.
			if (parts.Count == 1)
				return parts[0];

			byte[] part;
			int totalLength = 0;

			// Calculate the total length of the arrays to allocate enough space for all of them.
			foreach (var c in parts)
				totalLength += c.Length;

			// Create a new byte array with the calculated total length using GC.AllocateUninitializedArray
			//  for potential performance benefits in scenarios where the array is immediately filled.
			// This method reduces overhead by eliminating the initialization step for each element in the array.
			part = GC.AllocateUninitializedArray<byte>(totalLength);

			int currentIndex = 0;

			// Copy each byte array to the final array 'part', maintaining the original order.
			foreach (var c in parts)
			{
				// Copy the current byte array to 'part', starting at the current index.
				Buffer.BlockCopy(c, 0, part, currentIndex, c.Length);

				// Update the current index to the next position after the last copied byte.
				currentIndex += c.Length;
			}

			return part;
		}

		/// <summary>
		/// Returns the information required for download exported file
		/// </summary>
		/// <param name="fileId">File ID</param>
		/// <param name="fileType">File type</param>
		/// <returns>JSON</returns>
		protected object GetJsonForDownloadExportFile(string fileId, string fileType)
		{
			return new
			{
				id = fileId,
				type = fileType,
				controller = RouteData.Values["controller"] ?? "Home",
				action = "DownloadExportFile",
				Url = Url.Action("DownloadExportFile", new { id = fileId, type = fileType })
			};
		}

		public class RequestExportFile
		{
			public string Id { get; set; }
			public string Type { get; set; }
		}

		/// <summary>
		/// Returns the exported file to download
		/// </summary>
		/// <param name="id">File ID</param>
		/// <param name="type">File type</param>
		/// <returns>Exported file</returns>
		[AllowAnonymous]
		public FileResult DownloadExportFile([FromBody] RequestExportFile requestModel)
		{
			var id = requestModel.Id;
			var type = requestModel.Type;

			byte[] file = QCache.Instance.ExportFiles.Get(id) as byte[];
			QCache.Instance.ExportFiles.Invalidate(id);

			return type switch
			{
				"pdf" => File(file, "application/pdf", id),
				"xlsx" => File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", id),
				"ods" => File(file, "application/vnd.oasis.opendocument.spreadsheet", id),
				"csv" => File(file, "text/csv", id),
				"xml" => File(file, "text/xml", id),
				_ => File(file, "application/octet-stream", id)
			};
		}

		#endregion

		#region Server-side Function

		public class RequestServerFunctionModel
		{
			public string Func { get; set; }
			public List<object> Args { get; set; }
		}

		[HttpPost]
		public JsonNetResult ExecuteServerFunction([FromBody] RequestServerFunctionModel json)
		{
			var user = m_userContext.User;
			var sp = m_userContext.PersistentSupport;

			try
			{
				if (string.IsNullOrEmpty(json.Func) || json.Args == null)
					throw new BusinessException("Invalid arguments", "ExecuteServerFunction", "Empty argument value");
				if (!user.IsAuthorized(user.CurrentModule))
					throw new BusinessException("Permission denied", "ExecuteServerFunction", "Permission denied");

				var func = json.Func;
				var args = new List<object>();
				foreach (var arg in json.Args)
				{
					if (arg is JsonElement je)
					{
						if (je.ValueKind == JsonValueKind.String)
							args.Add(je.GetString() ?? "");
						else if (je.ValueKind == JsonValueKind.Number)
							args.Add(je.GetDecimal());
						else if (je.ValueKind == JsonValueKind.True)
							args.Add(true);
						else if (je.ValueKind == JsonValueKind.False)
							args.Add(false);
					}
					else
						args.Add(arg);
				}

				// Check if function can be executed from the client-side
				if (!GlobalFunctions.CheckAllowedFunctions(func))
					throw new BusinessException("Execution of this function is not allowed!", "ExecuteServerFunction", string.Format("Execution of '{0}' function is not allowed!", func));

				var inputForLog = " Values| " + Newtonsoft.Json.JsonConvert.SerializeObject(json);
				var funcoesGlobais = new GlobalFunctions(user, user.CurrentModule, sp);
				var typeFuncoesGlobais = funcoesGlobais.GetType();

				// Encontrar metodo
				MethodInfo method;
				try
				{
					method = typeFuncoesGlobais.GetMethod(func); // TODO: Cache ...
				}
				catch
				{
					throw new BusinessException("Invalid arguments", "ExecuteServerFunction", string.Format("Can't find the method '{0}' ", func));
				}

				// Obter parametros do metodo invocado
				var parameters = method.GetParameters();
				// Validate se quantidade de parametros recebidos coresponde a quantidade dos parametros do metodo
				var methodParamCount = parameters.Count();
				if (methodParamCount != args.Count)
					throw new BusinessException("Invalid arguments", "ExecuteServerFunction", "Incoherence of parameters." + inputForLog);

				// Cast dos dados JSON to tipo de dados Csharp
				var parametersInput = new object[methodParamCount];
				for (int p = 0; p < methodParamCount; p++)
				{
					try
					{
						var type = Nullable.GetUnderlyingType(parameters[p].ParameterType) ?? parameters[p].ParameterType;
						if (args[p] == null)
							parametersInput[p] = null;
						else if (type == typeof(bool))
							parametersInput[p] = args[p];
						else if (type == typeof(DateTime) || type == typeof(DateTime?))
							parametersInput[p] = DateTime.Parse(args[p].ToString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
						else
							parametersInput[p] = Convert.ChangeType(args[p], type);
					}
					catch (Exception e)
					{
						throw new BusinessException("Invalid arguments", "ExecuteServerFunction", "Error converting received value. " +  e.Message + inputForLog);
					}
				}

				// Invocar função
				sp.openConnection();
				var data = method.Invoke(funcoesGlobais, parametersInput);
				sp.closeConnection();

				return JsonOK(new { Success = true, Data = data, Message = "" });
			}
			catch (BusinessException e)
			{
				sp.closeConnection();
				return JsonERROR(e.Message, new { func = json.Func, args = json.Args });
			}
			catch (Exception e)
			{
				sp.closeConnection();
				Log.Error(string.Format("Business Exception. [message] Unexpected error [site] ExecuteServerFunction [cause] {0}; Values|{1}", e.Message, Newtonsoft.Json.JsonConvert.SerializeObject(json)));
				return JsonERROR(Resources.Resources.PEDIMOS_DESCULPA__OC63848, new { func = json.Func, args = json.Args });
			}
		}

		#endregion

		[HttpGet]
		public JsonNetResult GetEph(string ephID)
		{
			var value = GlobalFunctions.GetEph(UserContext.Current.User, ephID);
			return Json(new { Success = true, Operation = "GetEph", Value = value });
		}

		[HttpGet]
		public JsonNetResult HasRole(string roleId)
		{
			var value = GlobalFunctions.HasRole(UserContext.Current.User, roleId);
			return Json(new { Success = true, Operation = "HasRole", Value = value });
		}

		[HttpGet]
		public JsonNetResult GetLevelFromRole(decimal level, string roleId)
		{
			var value = GlobalFunctions.GetLevelFromRole(level, roleId);
			return Json(new { Success = true, Operation = "GetLevelFromRole", Value = value });
		}

		[HttpGet]
		public JsonNetResult IsFeatureActive(string feature)
		{
			var value = GlobalFunctions.IsFeatureActive(feature);
			return Json(new { Success = true, Operation = "IsFeatureActive", Value = value });
		}

		/*
		TODO: Implement this feature in Vue applications.
		For now the code is commented.

		// GET /GetMsqInfo/
		// Action for returning the MessageQueues info for a given model
		[HttpGet]
		[ActionName("GetMsqInfo")]
		public JsonNetResult GetMsqInfo(string id, string queueIdList)
		{
			List<System.Collections.Hashtable> infos = new List<System.Collections.Hashtable>();
			string[] queueList = queueIdList.Split(';');

			try
			{
				SelectQuery selQuery = new SelectQuery()
					.Select(CSGenioAmqqueues.FldQueueID)
					.Select(CSGenioAmqqueues.FldMQStatus)
					.Select(CSGenioAmqqueues.FldResposta)
					.Select(CSGenioAmqqueues.FldDataStatus)
					.Select(CSGenioAmqqueues.FldSendnumber)
					.From(CSGenio.business.Area.AreaMQQUEUES)
					.Where(CriteriaSet.And()
						.Equal(CSGenioAmqqueues.FldTabelaCod, id)
						.In(CSGenioAmqqueues.FldQueueID,queueList))
					.OrderBy(CSGenioAmqqueues.FldDataStatus, SortOrder.Ascending);
				selQuery.noLock = true;

				UserContext.Current.PersistentSupport.openConnection();
				DataMatrix ds = UserContext.Current.PersistentSupport.Execute(selQuery);
				UserContext.Current.PersistentSupport.closeConnection();

				for (int k = 0; k < ds.NumRows; k++)
				{
					//Check for Fail status over max retry configuration
					string status = ds.GetString(k, CSGenioAmqqueues.FldMQStatus);
					int sendNumber = ds.GetInteger(k, CSGenioAmqqueues.FldSendnumber);
					int maxsendnumber = Configuration.MessageQueueing.Maxsendnumber;
					MQueueACK statusMQ = (MQueueACK)Enum.Parse(typeof(MQueueACK), status);

					if (statusMQ == MQueueACK.ReplyFAIL && sendNumber >= maxsendnumber)
						statusMQ = MQueueACK.ReplyREJECT;

					System.Collections.Hashtable res = new System.Collections.Hashtable();
					res.Add("QueueID", ds.GetString(k, CSGenioAmqqueues.FldQueueID));
					res.Add("MQStatus", (int)statusMQ);
					res.Add("Resposta", ds.GetString(k, CSGenioAmqqueues.FldResposta));
					res.Add("DataStatus", ds.GetDate(k, CSGenioAmqqueues.FldDataStatus).ToString(Configuration.DateFormat.DateTimeSeconds, System.Globalization.CultureInfo.InvariantCulture));
					infos.Add(res);
				}
			}
			catch (Exception ex)
			{
				UserContext.Current.PersistentSupport.closeConnection();
				return Json(new { Success = false, Operation = "GetMsqInfo", Message = ex.Message });
			}

			return Json(new { Success = true, Operation = "GetMsqInfo", infos = infos });
		}

		// GET /GetMsqInfo/
		// Action for returning the MessageQueues info for a given model
		[HttpGet]
		[ActionName("SendMsqUpdate")]
		public JsonNetResult SendMsqUpdate(string id, string baseArea)
		{
			var sp = UserContext.Current.PersistentSupport;
			try
			{
				var area = CSGenio.business.Area.createArea(baseArea.ToLowerInvariant(), UserContext.Current.User, UserContext.Current.User.CurrentModule) as DbArea;
				if (area != null)
				{
					sp.openTransaction();
					sp.getRecord(area, id);
					// Pass oldValues as null to force re-sending.
					area.insertQueue(sp, "U", null, null);
					sp.closeTransaction();
				}
			}
			catch (Exception ex)
			{
				sp.rollbackTransaction();
				return Json(new { Success = false, Operation = "SendMsqUpdate", Message = ex.Message });
			}
			return Json(new { Success = true, Operation = "SendMsqUpdate", Message = Resources.Resources.FICHA_REENVIADA_PARA21165 });
		}
		*/

		/// <summary>
		/// Created by [CHN] at [2018.12.13]
		/// </summary>
		/// <param name="all_files">string with filename (test.pdf) and byte[] of file</param>
		/// <param name="zipfilename">string for the final zip file</param>
		/// <returns>FileContentResult (that can be sent directly as an ActionResult) with a zip of files</returns>
		public FileContentResult ZipFiles(Dictionary<string, byte[]> all_files, string zipfilename)
		{
			using (var compressedFileStream = new System.IO.MemoryStream())
			{
				using (var zipArchive = new System.IO.Compression.ZipArchive(compressedFileStream, System.IO.Compression.ZipArchiveMode.Create, false))
				{
					foreach (var file in all_files)
					{
						//Fix filename (replaces everything to "_" except letters, numbers and "-")
						string filename = System.Text.RegularExpressions.Regex.Replace(file.Key, "[^\\w\\.-]", "_");
						//Create a zip entry for each attachment
						var zipEntry = zipArchive.CreateEntry(filename);

						//Get the stream of the attachment
						using (var originalFileStream = new System.IO.MemoryStream(file.Value))
						using (var zipEntryStream = zipEntry.Open())
						{
							//Copy the attachment stream to the zip entry stream
							originalFileStream.WriteTo(zipEntryStream);
						}
					}
				}

				return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = zipfilename };
			}
		}

		/// <summary>
		/// Add a eph to the current user module and level and form id
		/// </summary>
		/// <param name="id">eph value</param>
		/// <param name="formId">origin form</param>
		/// <returns>Redirect to Home</returns>
		public ActionResult DefineEphForm([FromBody] RequestInitialEPH requestModel)
		{
			return DefineEphFormValues(new RequestInitialEPHS { SelectedIds = [requestModel.SelectedId], FormId = requestModel.FormId });
		}

		/// <summary>
		/// Add a eph to the current user module and level and form id
		/// </summary>
		/// <param name="id">eph values</param>
		/// <param name="formId">origin form</param>
		/// <returns>A json with the error/success response</returns>
		public ActionResult DefineEphFormValues([FromBody] RequestInitialEPHS requestModel)
		{
			if (SetPHEValues(requestModel.SelectedIds, requestModel.FormId))
				return JsonOK();
			return JsonERROR(Resources.Resources.OCORREU_UM_ERRO_AO_P53091);
		}

		/// <summary>
		/// Add a Permanent History Entry (PHE) to the current user module and level and form id
		/// </summary>
		/// <param name="ids">Permanent History Entry values</param>
		/// <param name="originId">Origin</param>
		/// <returns>A success or fail</returns>
		protected bool SetPHEValues(string[] ids, string originId)
		{
			try
			{
				User user = UserContext.Current.User;
				List<string> modules = [user.CurrentModule];

// USE /[MANUAL FOR BEFORE_FILL_EPH]/

				// Fill in the initial EPH value in the User object and get the values to be cached
				Dictionary<string, InitialEPHCache> initialEPHCache = GenioServer.security.UserFactory.FillEphRuntime(user, modules, ids, originId);

				// If the values of the other initial PHE are in the cache, we merge them.
				var cachedInitialPHE = UserContext.Current.GetInitialEph();
				if (cachedInitialPHE != null)
				{
					foreach (var cachePHE in cachedInitialPHE)
					{
						if (!initialEPHCache.ContainsKey(cachePHE.Key))
							initialEPHCache.Add(cachePHE.Key, cachePHE.Value);
						else
							initialEPHCache[cachePHE.Key].MergeCache(cachePHE.Value);
					}
				}

				// Writes the updated cache to session
				UserContext.Current.SetInitialEph(initialEPHCache);

				UserContext.Current.User = user;

				return true;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				return false;
			}
		}

		protected void DestroySession()
		{
			HttpContext.SignOutAsync().Wait();

			UserContext.Current.Destroy();
			GenioServer.security.GlobalAppSessions.Instance.Remove(HttpContext.Session.Id);
			HttpContext.Session.Clear();

			// log logoff (audit)
			CSGenio.framework.Audit.registLoginOut(UserContext.Current.User, Resources.Resources.SAIDA45792, Resources.Resources.SAIDA_ATRAVES_DA_OPC43152, HttpContext.GetHostName(), HttpContext.GetIpAddress());

			UserContext.Current.Destroy();
		}

		[NonAction]
		protected JsonNetResult GenericRecalculateFormulas(ViewModelBase form_data, string area, Func<string, GenioMVC.Models.ModelBase> find, Action<GenioMVC.Models.ModelBase> map)
		{
			try
			{
				RequestReflectHeader("RecalculateFormulasRequestNumber");

				var primaryKey = Navigation.GetStrValue(area);
				if (form_data == null || GenFunctions.emptyG(primaryKey) == 1)
					return JsonERROR();

				var model = find(primaryKey);
				map(model);

				var recalculatedFormulas = model.RecalculateFormulas();
				var viewModelValues = form_data.ConvertModelToViewModelValues(recalculatedFormulas);

				// TODO: Sanitize HTML content
				return JsonOK(viewModelValues);
			}
			catch (Exception e)
			{
				return JsonERROR(e.Message);
			}
		}

		[NonAction]
		protected void RequestReflectHeader(string header)
		{
			if (Request.Headers.TryGetValue(header, out var values))
				Response.Headers[header] = values;
		}

		[AllowAnonymous]
		public FileContentResult GetCaptcha(string captchaId)
		{
			using (var stream = new MemoryStream())
			{
				var captchaCode = new QCaptcha(40, 250, 6).Generate(stream);
				QCaptcha.SetCaptcha(captchaId, captchaCode, HttpContext.Session);

				return new FileContentResult(stream.ToArray(), "image/jpeg");
			}
		}

		/// <summary>
		/// This method is a temporary measure to stop duplicated code in the controller
		/// implementations. NameValueCollection should be removed!
		/// </summary>
		/// <param name="queryParams">The query parameters</param>
		/// <returns>A collection</returns>
		public NameValueCollection FormatQueryString(Dictionary<string, string> queryParams)
		{
			NameValueCollection qs = [];

			foreach (var elem in queryParams)
				qs.Add(elem.Key.ToString(), (elem.Value != null) ? elem.Value.ToString() : null);

			return qs;
		}

		/// <summary>
		/// Creates a short-lived cookie to store temporary user state
		/// </summary>
		/// <param name="key">The name of the cookie to create</param>
		/// <param name="payload">The serialized payload to store in the cookie</param>
		[NonAction]
		protected void CreateStateCookie(string key, string payload)
		{
			var ticket = QResources.CreatePayloadEncryptedBase64(payload);
			Response.Cookies.Append(Configuration.Program + "_" + key, ticket, new CookieOptions()
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddMinutes(2).UtcDateTime,
				IsEssential = true,
				Path = "/"
			});
		}

		/// <summary>
		/// Consumes the short-lived cookie, retrieves its payload, then deletes the cookie
		/// </summary>
		/// <param name="key">The cookie to retrieve</param>
		/// <returns>The contents of the payload or empty string in case there is no coookie</returns>
		[NonAction]
		protected string ConsumeStateCookie(string key)
		{
			string value = Request.Cookies[Configuration.Program + "_" + key] ?? "";
			Response.Cookies.Delete(Configuration.Program + "_" + key);
			return string.IsNullOrEmpty(value) ? "" : QResources.DecryptPayloadBase64(value);
		}
	}
}
