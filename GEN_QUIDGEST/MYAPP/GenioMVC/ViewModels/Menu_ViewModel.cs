using CSGenio.framework;
using GenioMVC.Helpers.Menus;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels
{
	public class Menu_ViewModel
	{
		private List<MenuEntry> m_allMenus;

		public List<MenuEntry> MenuList => m_allMenus;

		public List<MenuEntry> AvailableModules { get; set; }

		public string CurrentModule { get; set; }

		public string Icon { get; set; }

		public Menu_ViewModel(UserContext userContext)
		{
			User user = userContext.User;
			AvailableModules = Menus.AvailableModules(userContext);

			if (AvailableModules.Count >= 1)
			{
				if (user.CurrentModule != null && user.CurrentModule != "Public" && user.CurrentModule != "admin")
				{
					try
					{
						MenuEntry selectedModule = AvailableModules.First(m => m.ID.Equals(user.CurrentModule));
						FillMenuInfo(userContext, selectedModule);
					}
					catch (Exception)
					{
						m_allMenus = new List<MenuEntry>();
					}
				}
				else
					FillMenuInfo(userContext, AvailableModules.First());
			}
			else
				m_allMenus = new List<MenuEntry>();
		}

		private void FillMenuInfo(UserContext userContext, MenuEntry selectedModule)
		{
			m_allMenus = Menus.MenusForModule(userContext, selectedModule, true);
			User user = userContext.User;
			user.CurrentModule = selectedModule.ID;
			CurrentModule = user.CurrentModule;
			if (!string.IsNullOrEmpty(selectedModule.Image))
				Icon = selectedModule.Image;
		}
	}
}
