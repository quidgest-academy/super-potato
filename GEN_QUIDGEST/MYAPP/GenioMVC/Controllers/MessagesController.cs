using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers
{
	public class MessagesController : ControllerBase
	{
		public MessagesController(UserContextService userContext) : base(userContext) { }

		public ActionResult Index()
		{
			String Id = Messages.getID(Navigation.NavigationId);

			List<Message> messageList = TempData.GetObject<List<Message>>(Id) ?? new List<Message>();

			Messages_ViewModel viewModel = new Messages_ViewModel(messageList);

			return JsonOK(viewModel);
		}
	}
}
