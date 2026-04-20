using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using GenioMVC.ViewModels;
using System.Text;
using System.Xml.Serialization;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonPropertyName = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace GenioMVC.Helpers.Menus
{
	public class UserAvatarMenu
	{
		/// <summary>
		/// Action triggered in the controller
		/// </summary>
		public string Action { set; get; }

		/// <summary>
		/// Controller where to invoke the action
		/// </summary>
		public string Controller { set; get; }

		/// <summary>
		/// ID of the record to pass to the action
		/// </summary>
		public string RecordID { set; get; }

		/// <summary>
		/// Title/description to display
		/// </summary>
		public string Title { set; get; }

		/// <summary>
		/// CSS class for the displayed icon
		/// </summary>
		public string Font { set; get; }

		/// <summary>
		/// Image to display
		/// </summary>
		public string Image { set; get; }

		/// <summary>
		/// Retrieve from MANWIN a list of UserAvatarMenu items.
		/// </summary>
		/// <param name="sp">Persisent support for DB access</param>
		/// <param name="user">Current user</param>
		/// <returns>List with positioned UserAvatarMenu items</returns>
		public static List<UserAvatarMenu> GetMenus(PersistentSupport sp, User user)
		{
			List<UserAvatarMenu> result = new List<UserAvatarMenu>();

			// Specify custom items to show on user avatar menu.
			// result.Add(new UserAvatarMenu {
			//    Action = "",
			//    Controller = "",
			//    RecordID = "",
			//    Title = "",
			//    Font = "",
			//    Image = ""
			// });
// USE /[MANUAL FOR USER_AVATAR_MENU]/

			return result;
		}
	}

	/// <summary>
	/// User avatar menu item that comes from a MenuEntry and an EPH
	/// Created by [TMV] (2020.09.23)
	/// Refactored by [JMN] (2021.01.22)
	/// </summary>
	public class EPHUserAvatarMenu : UserAvatarMenu
	{
		/// <summary>
		/// ID of the associated MenuEntry
		/// </summary>
		public string MenuID { set; get; }

		public EPHUserAvatarMenu(MenuEntry other)
		{
			Title = other.Title;
			Action = other.Children.FirstOrDefault().Route_VUE;
			Controller = other.Children.FirstOrDefault().Controller;
			MenuID = other.ID;
			Font = other.Font;
			Image = other.Image;
		}

		/// <summary>
		/// Retrieve User Avatar items from EPH form menus.
		/// EPH takes into account the current user and module.
		/// </summary>
		public static List<EPHUserAvatarMenu> GetMenus(UserContext userContext)
		{
			List<EPHUserAvatarMenu> result = new List<EPHUserAvatarMenu>();
			try
			{
				User user = userContext.User;

				List<string> forms = EPH.GetEphCurrentForm(user);
				string modulo = user.CurrentModule;

				foreach (string form in forms.Distinct())
				{
					try
					{
						string id = form;
						MenuEntry menu = null;

						// Search the root menu for the dbedit
						while (!String.IsNullOrEmpty(id))
						{
							menu = Menus.FindMenu(modulo, id);

							if (String.IsNullOrEmpty(menu.Controller))
								break;

							id = menu.ParentId;
						}

						result.Add(new EPHUserAvatarMenu(menu));
					}
					catch (System.Exception e)
					{
						CSGenio.framework.Log.Error("Error creating EPH avatar menu for the menu " + forms + e.Message);
					}
				}
			}
			catch (System.Exception e)
			{
				CSGenio.framework.Log.Error("Unexpected error retrieving EPH avatar menus" + e.Message);
			}

			return result;
		}
	}

	/// <summary>
	/// Classe auxiliar to receber a àrvore de menus serializada em Xml pela API de geração
	/// Representa uma entrada de menu
	/// </summary>
	[XmlRoot("MENU")]
	public class MenuEntry
	{
		/// <summary>
		/// Supreme Administrator
		/// </summary>
		private const int MAX_LEVEL = 99;

		/// <summary>
		/// Type de menu
		/// ITEM - Item normal de menu
		/// LIST - Listing dos elementos de uma table
		/// REPORT - Relatório
		/// </summary>
		[XmlAttribute("TYPE")]
		public string Type { get; set; }

		/// <summary>
		/// Descrição que o user vê associada a esta entrada de menu
		/// </summary>
		[XmlAttribute("DESC")]
		public string Title { get; set; }

		/// <summary>
		/// Sigla que o user vê associada a esta entrada de menu
		/// </summary>
		[XmlAttribute("SIGLA")]
		public string Sigla { get; set; }

		/// <summary>
		/// ID do módulo / entrada de menu
		/// </summary>
		[XmlAttribute("ID")]
		[JsonPropertyName("Id")]
		public string ID { get; set; }

		/// <summary>
		/// ID do módulo / entrada de menu anterior
		/// </summary>
		[XmlAttribute("PRTID")]
		[JsonIgnore]
		public string ParentId { get; set; }

		/// <summary>
		/// Nível de acesso necessário to poder aceder a este menu
		/// </summary>
		[XmlElement("ACCESS")]
		[JsonIgnore]
		public string RoleId { get; set; }

		/// <summary>
		/// Documento ou página externa a abrir
		/// </summary>
		[XmlAttribute("WEBPAGE")]
		[JsonPropertyName("WebPage")]
		public string WEBPAGE { get; set; }

		/// <summary>
		/// text do bullet
		/// </summary>
		[XmlAttribute("HELPTITLE")]
		[JsonPropertyName("HelpTitle")]
		public string HELPTITLE { get; set; }

		/// <summary>
		/// Imagem associada a esta entrada de menu
		/// </summary>
		[XmlAttribute("IMG")]
		public string Image { get; set; }

		/// <summary>
		/// Imagem associada a esta entrada de menu(VUE)
		/// </summary>
		[XmlAttribute("IMGVUE")]
		public string ImageVUE { get; set; }

		/// <summary>
		/// Fonte (icon) associada a esta entrada de menu
		/// </summary>
		[XmlAttribute("FNT")]
		public string Font { get; set; }

		/// <summary>
		/// Vector (icon) associada a esta entrada de menu
		/// </summary>
		[XmlAttribute("SVG")]
		public string Vector { get; set; }

		/// <summary>
		/// Menu to open after clicking the parent menu
		/// </summary>
		[XmlAttribute("DEFAULTACTION")]
		public string DefaultAction { get; set; }

		/// <summary>
		/// Acção desencadeada por esta entrada de menu
		/// </summary>
		[XmlAttribute("ACT")]
		public string Action { get; set; }

		/// <summary>
		/// Acção desencadeada por esta entrada de menu (VUE)
		/// </summary>
		[XmlAttribute("ACTVUE")]
		[JsonPropertyName("RouteName")]
		public string Route_VUE { get; set; }

		#region MVC specific parameters (to be removed)

		/// <summary>
		/// Acção desencadada no lado do MVC por esta entrada de menu
		/// </summary>
		[XmlAttribute("ACTMVC")]
		[JsonPropertyName("ActionMVC")]
		public string Action_MVC { get; set; }

		/// <summary>
		/// Controller invocado na acção desencadeada por esta entrada de menu
		/// </summary>
		[XmlAttribute("CONT")]
		public string Controller { get; set; }

		/// <summary>
		/// Filters for this action
		/// </summary>
		[XmlElement("FILTERS")]
		[JsonIgnore]
		public string Filters { get; set; }

		/// <summary>
		/// QueryString for this action
		/// </summary>
		[XmlAttribute("QUERYSTRING")]
		[JsonIgnore]
		public string QueryString { get; set; }

		#endregion

		/// <summary>
		/// Nível deste menu na àrvore, o nível associado aos módulos é o nível 0
		/// </summary>
		[XmlAttribute("LVL")]
		public int TreeLevel { get; set; }

		/// <summary>
		/// Indica se o relatório deve ser pré-visualizado
		/// </summary>
		[XmlAttribute("PREVIEW")]
		public bool Preview { get; set; }

		/// <summary>
		/// Indica se o menu tem um separador (visual)
		/// </summary>
		[XmlAttribute("SEPARATES")]
		public bool Separates { get; set; }

		/// <summary>
		/// Filhos deste menu
		/// </summary>
		[XmlElement("MENU")]
		public List<MenuEntry> Children { get; set; }

		/// <summary>
		/// Se ativo soma os elementos deste DbEdit/menu
		/// </summary>
		[XmlAttribute("SUMMENU")]
		[JsonIgnore]
		public bool SumMenu { get; set; }

		/// <summary>
		/// True if the record sum should be hidden for this menu, false otherwise
		/// </summary>
		[XmlAttribute("HIDEMENUSUM")]
		public bool HideMenuSum { get; set; }

		/// <summary>
		/// Se ativo significa que o menu tem condições
		/// </summary>
		[XmlAttribute("HASCONDITION")]
		[JsonIgnore]
		public bool HasCondition { get; set; }

		/// <summary>
		/// Se ativo, significa que menu tem continuação para form
		/// </summary>
		[XmlAttribute("ISFORM")]
		public bool IsForm { get; set; }

		/// <summary>
		/// Modo de entrada
		/// </summary>
		[XmlAttribute("FORMMODE")]
		public string Mode { get; set; }

		/// <summary>
		/// The menu order
		/// </summary>
		[XmlAttribute("ORDER")]
		public string Order { get; set; }

		/// <summary>
		/// Contador desta lista presente neste menu
		/// </summary>
		public int Count { get; set; }

		public MenuEntry(MenuEntry other)
		{
			Type = other.Type;
			Title = other.Title;
			Sigla = other.Sigla;
			ID = other.ID;
			RoleId = other.RoleId; // este copia o apontador, deve ser irrelevante!
			Image = other.Image;
			ImageVUE = other.ImageVUE;
			Font = other.Font;
			Action = other.Action;
			Vector = other.Vector;
			Route_VUE = other.Route_VUE;
			TreeLevel = other.TreeLevel;
			// não copia os filhos, para efeitos do algoritmo de reconstrução da árvore de menus
			// ignorar o Children é propositado
			Children = new List<MenuEntry>();
			WEBPAGE = other.WEBPAGE;
			HELPTITLE = other.HELPTITLE;
			Controller = other.Controller;
			Filters = other.Filters;
			QueryString = other.QueryString;
			Preview = other.Preview;
			Separates = other.Separates;
			SumMenu = other.SumMenu;
			HideMenuSum = other.HideMenuSum;
			HasCondition = other.HasCondition;
			IsForm = other.IsForm;
			Mode = other.Mode;
			Order = other.Order;
			DefaultAction = other.DefaultAction;
		}

		// construtor vazio para permitir (des)serializar em format XML
		public MenuEntry() { }

		/// <summary>
		/// Checks if a user has access to view a menu entry
		/// </summary>
		/// <param name="user">User we want to check</param>
		/// <param name="module">Module the entry is in</param>
		/// <returns></returns>
		public bool Allows(User user, string module)
		{
			//use the full qualified name to prevent problems with tables with name ROLE
			CSGenio.framework.Role role = CSGenio.framework.Role.GetRole(RoleId);
			//TVC 2023.10.04 When there is no role assigned and the user is a guest the menu is allowed
			// or when there is no role assigned and the user has any role in that menu module
			if (role.Equals(CSGenio.framework.Role.INVALID) && (user.IsGuest() || user.RolesPerModule.ContainsKey(module)))
				return true;
			return user.VerifyAccess(role, module);
		}
	}

	public class Menus
	{
		private static readonly IMenuLoader m_menuLoader = new XmlMenuLoaderService();

		public static List<MenuEntry> AllMenus => m_menuLoader.GetAllMenus();

		public static MenuEntry FindMenu(string module, string menuID)
		{
			string findKey = module + menuID;

			if (m_flatMenus == null )
			{
				m_flatMenus = new Dictionary<string, MenuEntry>();
				foreach (var moduleEntry in AllMenus)
				{
					var menus = FlattenMenu(moduleEntry);
					foreach (var entry in menus)
					{
						string key = moduleEntry.ID + entry.ID;
						m_flatMenus[key] = entry;
					}
				}
			}

			return m_flatMenus[findKey];
		}

		/// <summary>
		/// Finds a menu and applies the same transformation to the MenuEntry object as in the navigation bar
		/// </summary>
		/// <param name="userContext">User context we want to check</param>
		/// <param name="module">Module the entry is in</param>
		/// <param name="menuID">ID of the entry</param>
		/// <param name="count">Whether to count</param>
		/// <returns></returns>
		public static MenuEntry FindMenuForUserRec(UserContext userContext, string module, string menuID, bool count = false)
		{
			List<MenuEntry> menuListForUserRec;

			// Find menu and put in a List because MenusForUserRec() takes a List
			MenuEntry menu = FindMenu(module, menuID);
			List<MenuEntry> menuList = new List<MenuEntry>();
			menuList.Add(menu);

			// Apply same transformation to menu entries as in the navigation bar so they are consistent
			menuListForUserRec = MenusForUserRec(userContext, menuList, module, count);

			return menuListForUserRec.FirstOrDefault();
		}

		/// <summary>
		/// Checks if the user has access to the menu
		/// </summary>
		/// <param name="userContext">The context of the user</param>
		/// <param name="menu">The menu entry object</param>
		/// <param name="module">The module ID</param>
		/// <returns></returns>
		private static bool AllowMenu(UserContext userContext, MenuEntry menu, string module)
		{
			bool hasUserAcess = (menu.TreeLevel > -1 && menu.Allows(userContext.User, module)) || menu.TreeLevel == -1;
			bool hideMenu = menu.HasCondition && !new MenuConditionValidator(userContext).ValidateCondition(menu, module);

			return hasUserAcess && !hideMenu;
		}

		private static List<MenuEntry> FlattenMenu(MenuEntry entry)
		{
			List<MenuEntry> menuList = new List<MenuEntry>();
			menuList.Add(entry);
			foreach (var child in entry.Children)
			{
				menuList.AddRange(FlattenMenu(child));
			}
			return menuList;
		}

		private static Dictionary<string, MenuEntry> m_flatMenus = null;


		public static List<MenuEntry> MenusForUser(UserContext userContext)
		{
			List<MenuEntry> result = new List<MenuEntry>();

			foreach (MenuEntry mod in AllMenus)
			{
				var module = mod.ID;

				List<MenuEntry> modMenus = MenusForUserRec(userContext, mod.Children, module);

				// só se o módulo tiver entradas de menu é que se adiciona às entradas de módulos
				if (modMenus.Count > 0)
				{
					MenuEntry thisMod = new MenuEntry(mod);
					thisMod.Children = modMenus;
					result.Add(thisMod);
				}
			}

			return result;
		}

		public static List<MenuEntry> AvailableModules(UserContext userContext)
		{
			var conditionValidator = new MenuConditionValidator(userContext);
			var menuService = new UserMenuService(m_menuLoader, conditionValidator, userContext.User);
			return menuService.GetAvailableModules();
		}

		public static List<MenuEntry> GetModuleMenus(UserContext userContext, string module, bool count = false)
		{
			foreach (MenuEntry mod in AllMenus)
			{
				if (mod.ID != module)
					continue;
				List<MenuEntry> modMenus = MenusForUserRec(userContext, mod.Children, mod.ID, count);

				return modMenus;
			}

			return new List<MenuEntry>();
		}

		public static List<MenuEntry> MenusForModule(UserContext userContext, MenuEntry module, bool count = false)
		{
			return MenusForUserRec(userContext, module.Children, module.ID, count);
		}

		private static List<MenuEntry> MenusForUserRec(UserContext userContext, List<MenuEntry> menus, string module, bool count = false)
		{
			List<MenuEntry> result = new List<MenuEntry>();

			foreach (MenuEntry entry in menus)
			{
				MenuEntry menu = new MenuEntry(entry);
				// se o o user tem nível de acesso adicionam-se os filhos tratados
				bool showMenu = AllowMenu(userContext, entry, module);
				if (!showMenu) continue;

				menu.Children = MenusForUserRec(userContext, entry.Children, module, count);
				if (entry.Children.Count == 1)
				{
					MenuEntry child = entry.Children.First();

					if (child.Type == "LIST")
						menu.Children = entry.Children;
					else if (!string.IsNullOrEmpty(child.Action) && child.TreeLevel == -1)
					{
						menu.Count = menu.Children.Select(x => x.Count).ToArray().Sum();

						menu.Type = child.Type;
						menu.Preview = child.Preview;
						menu.Action = child.Action;
						menu.Action_MVC = child.Action_MVC;
						menu.Route_VUE = child.Route_VUE;
						menu.Controller = child.Controller;
						menu.WEBPAGE = child.WEBPAGE;
						menu.QueryString = child.QueryString;
						menu.IsForm = child.IsForm;
						menu.Mode = child.Mode;
						menu.Children = new List<MenuEntry>();

						// MH - Depois duns testes detetou-se que o presente código executado no caso dos menus "finais" (ex: ... -> MB -> R).
						// Neste caso, não faz sentido copiar imagem e help. No entanto, podemos copiar caso se o "menu" não o tiver preenchido.
						if (string.IsNullOrEmpty(menu.Image) && !string.IsNullOrEmpty(child.Image))
						{
							menu.Image = child.Image;
							menu.ImageVUE = child.ImageVUE;
							menu.Vector = child.Vector;
						}
						if (string.IsNullOrEmpty(menu.Font) && !string.IsNullOrEmpty(child.Font))
							menu.Font = child.Font;
						if (string.IsNullOrEmpty(menu.HELPTITLE) && !string.IsNullOrEmpty(child.HELPTITLE))
							menu.HELPTITLE = child.HELPTITLE;

						// Menu counter of elements to be shown (using reflection)
						if (count && child.SumMenu && menu.Count == 0)
						{
							var list = ListViewModel.CreateListViewModel(userContext, menu.Controller, menu.Action_MVC);
							menu.Count = list.GetCount(userContext.User);
						}
					}
				}
				else
					menu.Count = menu.Children.Select(x => x.Count).ToArray().Sum();

				if (menu.Controller != null && menu.Action != null && menu.Action_MVC != null || menu.Children.Count > 0)
					result.Add(menu);
				//se for uma nova página web também adiciona aos menus
				else if (menu.Action == "GenGenio.MenuPaginaWeb")
					result.Add(menu);
			}

			return result;
		}

		/// <summary>
		/// Gets the menu entry and checks show when conditions of the children
		/// </summary>
		/// <param name="userContext">The context of the user</param>
		/// <param name="module">The module ID</param>
		/// <param name="menu">The menu ID</param>
		/// <returns>The menu with the submenus filtered</returns>
		public static MenuEntry GetFilteredMenu(UserContext userContext, string module, string menuID)
		{
			MenuEntry originalMenu = FindMenu(module, menuID);
			MenuEntry menu = new(originalMenu)
			{
				Children = [.. originalMenu.Children.Where(child => AllowMenu(userContext, child, module))]
			};

			return menu;
		}


		public static List<string> MenuTextPath(string module, string menuID)
		{
			var path = new List<string>();

			var _mId = menuID;
			while (!string.IsNullOrEmpty(_mId))
			{
				MenuEntry menu;
				try
				{
					menu = FindMenu(module, _mId);
				}
				catch (KeyNotFoundException)
				{
					// algo de errado no XML ou primeiro menu não existe
					return new List<string>();
				}

				// ex: SE e SU
				if (string.IsNullOrEmpty(menu.Title))
				{
					_mId = menu.ParentId;
					continue;
				}

				path.Add(Helpers.GetTextFromResources(menu.Title));

				_mId = menu.ParentId;
			}

			path.Reverse();
			return path;
		}

		/// <summary>
		/// Get list of actions to fill the initial phe
		/// </summary>
		/// <param name="user">User logged in</param>
		/// <param name="MvcRoute">It indicates if we want the action name or the route name</param>
		/// <returns>HashSet<(string action, string controller)></returns>
		//created by FFS at 2024.01.03
		//last updated by [XX] at [XXXX.XX.XX]
		//last reviewed by [XX] at [XXXX.XX.XX]
		public static HashSet<(string action, string controller)> GetAllowedRoutes(User user, bool MvcRoute)
		{
			var allowedActions = new HashSet<(string action, string controller)>();

			if (user.CurrentModule == null || user.CurrentModule == "Public")
				return allowedActions;

			// Get the action for the form id
			string id = user.EphTofill.GetForm(user.CurrentModule);

			MenuEntry menu;

			// Search all branches possible to navigate
			while (!string.IsNullOrEmpty(id))
			{
				menu = Menus.FindMenu(user.CurrentModule, id);

				if (string.IsNullOrEmpty(menu.Controller))
					break;

				string controller = menu.Controller.Substring(0, 1).ToUpper() + menu.Controller.Substring(1).ToLower();

				id = menu.ParentId;

				if (MvcRoute)
					allowedActions.Add((menu.Action_MVC, controller));
				else
					allowedActions.Add((menu.Route_VUE, null));
			}

			return allowedActions;
		}
	}
}
