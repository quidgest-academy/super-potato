using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CSGenio.framework;
using GenioMVC.Helpers.Menus;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels.Home
{
	public class HomePageDefinition
	{
		public string Identifier { get; set; }

		public bool Public { get; set; }

		public string Module { get; set; }

		public int Order { get; set; }

		public string Menu { get; set; }

		public string Menu_Id { get; set; }

		public string MenuRoleId { get; set; }

		public string Form { get; set; }

		public string Controller { get; set; }

		[JsonIgnore]
		public CSGenio.framework.Role Role { get; set; }
	}

	public class HomePage_ViewModel(UserContext userContext, bool isGuestUser) : ViewModelBase(userContext)
	{
		public bool IsGuestUser { get; private set; } = isGuestUser;

		private static readonly List<HomePageDefinition> homePages =
		[
		];

		[JsonIgnore]
		public static List<HomePageDefinition> HomePages { get { return homePages; } }

		public string HomePageController { get; private set; }

		public string HomePageAction { get; private set; }

		public HomePageDefinition HomePageDef { get; private set; }

		public bool HasHomePage { get { return HomePageDef != null && !string.IsNullOrEmpty(HomePageController) && !string.IsNullOrEmpty(HomePageAction); } }

		public Dictionary<string, HomePageDefinition> GetAvaibleHomePages(List<string> modules)
		{
			var user = m_userContext.User;
			var result = new Dictionary<string, HomePageDefinition>();

			// Home pages of specific module
			var avaiblePages = homePages.Where(hp => modules.Contains(hp.Module)).OrderBy(hp => hp.Order).ToList();

			// Home page before Login
			if (IsGuestUser)
				avaiblePages.AddRange(homePages.Where(hp => hp.Public && hp.Module == "Public").OrderBy(hp => hp.Order));

			foreach (var hPage in avaiblePages.Distinct())
			{
				if (hPage.Public || user.VerifyAccess(hPage.Role, hPage.Module))
				{
					if (!string.IsNullOrEmpty(hPage.Menu))
					{
						// Check if user has access to this menu
						MenuEntry menu = new MenuEntry() { RoleId = hPage.MenuRoleId };
						if (!string.IsNullOrEmpty(menu.RoleId) && !menu.Allows(user, hPage.Module))
							continue;
					}

					if (!result.ContainsKey(hPage.Module))
						result.Add(hPage.Module, hPage);
				}
			}

			return result;
		}

		public void Load()
		{
			var user = m_userContext.User;
			var module = user.CurrentModule ?? string.Empty;

			// Home pages of specific module
			var avaiblePages = homePages.Where(hp => hp.Module == module).OrderBy(hp => hp.Order).ToList();

			// Home page before Login
			if (IsGuestUser)
				avaiblePages.AddRange(homePages.Where(hp => hp.Public && hp.Module == "Public").OrderBy(hp => hp.Order));

			foreach (var hPage in avaiblePages.Distinct())
			{
				if (hPage.Public || user.VerifyAccess(hPage.Role, module))
				{
					if (!string.IsNullOrEmpty(hPage.Menu))
					{
						MenuEntry menu = null;
						var menuId = hPage.Menu.Substring(0, 1);
						bool nextHomePage = false;

						try
						{
							menu = Menus.FindMenu(hPage.Module, menuId);
						}
						catch
						{
							continue;
						}

						if (menu != null)
						{
							if (!string.IsNullOrEmpty(menu.RoleId) && !menu.Allows(user, hPage.Module))
								continue;

							while (hPage.Menu != menuId && menu.ID != hPage.Menu_Id)
							{
								menuId = hPage.Menu.Substring(0, menuId.Length + 1);
								menu = (menu.Children ?? new List<MenuEntry>()).Find(m => m.ID == hPage.Menu_Id || m.ID == menuId);

								// Only menu branches that have the AccessLevel
								if (menu == null || (!string.IsNullOrEmpty(menu.RoleId) && !menu.Allows(user, hPage.Module)))
								{
									nextHomePage = true;
									break;
								}
							}
						}

						if (nextHomePage || menu == null)
							continue;

						HomePageDef = hPage;
						HomePageController = menu.Controller;
						HomePageAction = menu.Action_MVC;

						break;
					}
					else if (!string.IsNullOrEmpty(hPage.Form))
					{
						HomePageDef = hPage;
						HomePageController = hPage.Controller;
						HomePageAction = hPage.Form + "_Show";
						break;
					}
				}
			}
		}
	}
}
