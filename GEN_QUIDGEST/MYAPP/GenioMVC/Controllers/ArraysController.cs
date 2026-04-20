using CSGenio.business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers
{
	/// <summary>
	/// Arrays controller
	/// </summary>
	public class ArraysController : ControllerBase
	{
        public ArraysController(UserContextService userContext) : base(userContext) { }

		/// <summary>
		/// Gets the array "s_module".
		/// </summary>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult S_module()
		{
			return Json(ArrayS_module.Serialize(UserContext.Current.User.Language));
		}

		/// <summary>
		/// Gets the array "s_roles".
		/// </summary>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult S_roles()
		{
			return Json(ArrayS_roles.Serialize(UserContext.Current.User.Language));
		}

		/// <summary>
		/// Gets the array "s_tpproc".
		/// </summary>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult S_tpproc()
		{
			return Json(ArrayS_tpproc.Serialize(UserContext.Current.User.Language));
		}

	}
}
