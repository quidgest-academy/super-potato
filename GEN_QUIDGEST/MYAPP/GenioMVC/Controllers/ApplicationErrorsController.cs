using CSGenio.framework;
using GenioMVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers
{
	public class ApplicationErrorsController : ControllerBase
	{
		public ApplicationErrorsController(UserContextService userContext) : base(userContext) { }

		[HttpPost]
		public JsonResult LogJavaScriptError(string message)
		{
			Log.Error("_Javascript_" + message);
			return JsonOK(new { success = true });
		}

		public new ActionResult NotFound()
		{
			if (Request.IsAjaxRequest())
				return NotFoundError();
			else
				return RedirectToVuePage("NotFound", null, false);
        }

		public ActionResult ServerError()
		{
            if (Request.IsAjaxRequest())
                return InternalServerError();
            else
                return RedirectToVuePage("ServerError", null, false);
        }
	}
}
