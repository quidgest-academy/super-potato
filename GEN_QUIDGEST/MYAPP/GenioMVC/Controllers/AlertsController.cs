using System.Collections.Generic;
using System.Linq;
using GenioMVC.Models.Navigation;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers
{
	public class AlertsController : ControllerBase
	{
		public AlertsController(UserContextService userContext) : base(userContext) { }
		
		public ActionResult Index()
		{
			var qs = new System.Collections.Specialized.NameValueCollection();
			qs.AddRange(Request.Query);
			bool isAjaxRequest = Request.IsAjaxRequest();
			ViewModels.Alerts_ViewModel vm = new ViewModels.Alerts_ViewModel(UserContext.Current, qs, isAjaxRequest);
			List<Alert> alerts = vm.GenAlerts();

			return JsonOK(alerts);
		}
	}
}
