using CSGenio.core.di;
using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GenioMVC;

public class ModuleActionFilter : IActionFilter
{
	private static readonly HashSet<(string action, string controller)> allowedActions =
	[
		("Profile", "Home"),
		("LogOff", "Account"),
		("GetIfUserLogged", "Account"),
		("UserAvatar", "Account"),
		("NavigationalBar", "Home"),
		("GetImage", "Account"),
		("Change2FA", "Home"),
		("CreateTOTP", "Home"),
		("GetConfig", "Config"),
		("ProfileRedirect", "Home"),
		("HomeRedirect", "Home"),
		("Change2FARedirect", "Home")
	];

	private readonly UserContext m_userContext;
	private IDisposable m_loggerScope;
	private IDisposable m_metricScope;

	public ModuleActionFilter(UserContextService userContext)
	{
		m_userContext = userContext.Current;
		m_loggerScope = m_userContext is not null ? setLoggerContext(m_userContext.User) : null;
	}

	/// <summary>
	/// Before controller action executes
	/// </summary>
	/// <param name="context"></param>
	public void OnActionExecuting(ActionExecutingContext context)
	{
		AuthorizeForUsers(context);
		VerifyInitialPHE(context);

		// Ensure UserContext is initialized after the model binding deserialized objects
		//  using the empty construction.
		foreach (var m in context.ActionArguments)
		{
			if (m.Value is ViewModelBase vmb)
				vmb.Init(m_userContext);
		}

		//Measure all the controller load times
		if (m_userContext is not null && context.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor action)
		{
			m_metricScope = GenioDI.MetricsOtlp.RecordTime("page_load_time", new System.Diagnostics.TagList([
					new("Controller", action.ControllerName),
					new("Action", action.ActionName),
					new("Module", m_userContext.User?.CurrentModule),
					new("Year", m_userContext.User?.Year)
				]), "ms", "Time to load the page.");
		}
	}

	/// <summary>
	/// After controller action executes
	/// </summary>
	/// <param name="context"></param>
	public void OnActionExecuted(ActionExecutedContext context)
	{
		var qAjaxId = context.HttpContext.Request.Headers["QAjaxIdentifier"];
		if (!string.IsNullOrEmpty(qAjaxId))
			context.HttpContext.Response.Headers["QAjaxIdentifier"] = qAjaxId;

		// MH (07/09/2017) - Ensure that the transaction was not left open after processing the request. And if transaction is still open it will be closed automatically.
		if (m_userContext is not null && m_userContext.PersistentSupport != null && !m_userContext.PersistentSupport.TransactionIsClosed)
		{
			CSGenio.framework.Log.Error(string.Format("The transaction still open after the action was executed. The transaction will be closed automatically by the application. (URL: {0})",
				context.HttpContext.Request.Path));

			try
			{
				m_userContext.PersistentSupport.closeTransaction();
			}
			catch (Exception ex)
			{
				CSGenio.framework.Log.Error(ex.ToString());
			}
		}

		// Dispose of the OpenTelemetry logger scope context
		m_loggerScope?.Dispose();
		m_metricScope?.Dispose();
	}

	private IDisposable setLoggerContext(CSGenio.framework.User user)
	{
		return CSGenio.framework.Log.SetContext(new
		{
			user = user.Name, // Add user to context
			user_ip = user.Location, // Add client ip to context
			year = user.Year // Add year to context
		});
	}

	private void AuthorizeForUsers(ActionExecutingContext context)
	{
		var u = m_userContext?.User;
		if (u is not null && !u.IsGuest())
		{
			// Check if user has their account disabled
			if (u.Status == 2)
			{
				//Force the user to logout
				context.HttpContext.SignOutAsync().Wait();
				context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "HomeRedirect" }, { "controller", "Home" } });
			}
			// Check if user needs to change password
			else if (u.NeedsToChangePassword() && !ActionsAllowed(context))
				context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "ProfileRedirect" }, { "controller", "Home" } });
			// Check if user has to setup 2FA
			else if (u.NeedsToSetup2FA() && !ActionsAllowed(context))
				context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Change2FARedirect" }, { "controller", "Home" } });
		}
	}

	/// <summary>
	/// Verify if the current module requires initial PHE and if the user already assigned it
	/// </summary>
	/// <param name="context">Action call context</param>
	/// <returns></returns>
	//created by FFS at 2024.01.03
	//last updated by [XX] at [XXXX.XX.XX]
	//last reviewed by [XX] at [XXXX.XX.XX]
	private void VerifyInitialPHE(ActionExecutingContext context)
	{
		var currentAction = context.RouteData.Values["action"].ToString();
		if (m_userContext is not null && !m_userContext.User.EphOk && !ActionsAllowed(context) && !ActionsInitialPHEAllowed(context))
			context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "GetEphFormActionByModule" }, { "controller", "Home" }, { "EphModule", m_userContext.User.CurrentModule } });
	}

	private bool ActionsAllowed(ActionExecutingContext filterContext)
	{
		var currentAction = filterContext.RouteData.Values["action"].ToString();
		var currentController = filterContext.RouteData.Values["controller"].ToString();

		return allowedActions.Contains((currentAction, currentController));
	}

	/// <summary>
	/// Define the actions allowed when the user didn't define the initial phe yet
	/// </summary>
	/// <param name="filterContext">Action call context</param>
	/// <returns></returns>
	//created by FFS at 2024.01.03
	//last updated by [XX] at [XXXX.XX.XX]
	//last reviewed by [XX] at [XXXX.XX.XX]
	private bool ActionsInitialPHEAllowed(ActionExecutingContext filterContext)
	{
		//List of actions that the user will run to select the initial phe
		var allowedActions = Helpers.Menus.Menus.GetAllowedRoutes(m_userContext.User, true);

		var currentAction = filterContext.RouteData.Values["action"].ToString();
		var currentController = filterContext.RouteData.Values["controller"].ToString();

		allowedActions.Add(("DefineEphForm", currentController));
		allowedActions.Add(("GetEphFormActionByModule", "Home"));

		return allowedActions.Contains((currentAction, currentController));
	}
}
