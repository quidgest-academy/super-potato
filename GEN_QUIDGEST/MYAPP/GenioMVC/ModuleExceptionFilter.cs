using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using CSGenio.framework;
using GenioMVC.Models.Exception;
using GenioMVC.Models.Navigation;

namespace GenioMVC;

public class ModuleExceptionFilter : IExceptionFilter
{
	private readonly UserContext m_userContext;
	private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

	public ModuleExceptionFilter(UserContextService userContext, ITempDataDictionaryFactory tempDataDictionaryFactory)
	{
		m_userContext = userContext.Current;
		_tempDataDictionaryFactory = tempDataDictionaryFactory;
	}

	public void OnException(ExceptionContext context)
	{
		Exception ex = context.Exception;
		if (ex is InvalidAuthenticationException)
		{
			context.HttpContext.SignOutAsync().Wait();
			m_userContext.Destroy();
			context.Result = RedirectToErrorPage("Invalid authentication");
		}
		else if (ex is UserUnavailableException)
		{
			context.HttpContext.SignOutAsync().Wait();
			m_userContext.Destroy();
			context.Result = RedirectToErrorPage(ex.Message);
		}

		if (ex != null)
			Log.Error(string.Format("Controller OnException: {0}; {1};", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "") + ex.ToString());

		// Fetch the error list from the current thread if EventTracking is enabled
		var errors = Configuration.EventTracking ? Log.GetThreadErrors() : null;

		// Clear the error cache for the current thread to reset the state
		Log.ClearThreadErrorsCache();

		// If there are errors, store them in TempData for persistence across redirects
		if (errors != null)
			_tempDataDictionaryFactory.GetTempData(context.HttpContext).SetObject("ErrorList", errors);

		var urlReferrer = context.HttpContext.Request.Headers["Referer"];
		CSGenio.framework.Log.Error(string.Format("Application_Error: {0}; {1}; UrlReferer:{2}; URL:{3}; ", ex.Message, ex.InnerException != null ? ex.InnerException.Message : "", urlReferrer, context.HttpContext.Request.Path) + ex.ToString());
	}

	protected JsonResult RedirectToErrorPage(string message)
	{
		return new JsonResult(new {
			statusCode = System.Net.HttpStatusCode.Redirect,
			type = "erro",
			message,
			NavigationData = m_userContext.CurrentNavigation.GetHistoryToUpdateClientSide()
		});
	}
}
