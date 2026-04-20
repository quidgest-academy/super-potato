using Lucene.Net.Store;
using Microsoft.AspNetCore.Mvc;

using GenioMVC.Helpers.Menus;
using GenioMVC.Models.Navigation;

namespace GenioMVC.Controllers
{
	public class MenuSearchController : ControllerExtension
	{
		public MenuSearchController(UserContextService userContext) : base(userContext) { }

		// GET: /Menu/
		public ActionResult Search(string searchString = "")
		{
			var results = MenuSearch.Search(UserContext.Current, searchString, UserContext.Current.User,
				Thread.CurrentThread.CurrentCulture);
			foreach (var result in results)
				result.MenuObj = Menus.FindMenuForUserRec(UserContext.Current, result.Module, result.Id, true);
			return JsonOK(results);
		}
	}
}
