using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Helpers.Menus;

namespace GenioMVC.Models.Navigation
{
	public class MenuResult
	{
		public string Id { get; set; }

		public string Text { get; set; }

		public string FlatMenu { get; set; }

		public string ActionId { get; set; }

		public string Module { get; }

		public string ModuleText { get; }

		/// <summary>
		/// TODO: Fill in right here vs fill in the controller
		/// </summary>
		public MenuEntry MenuObj { get; set; }

		public MenuResult(string id,string module, string text, string moduleText)
		{
			this.Id = id;
			Module = module;
			Text = text;
			ModuleText = moduleText;
		}

		public static List<MenuResult> SearchableMenus(UserContext userContext, CultureInfo cultureInfo)
		{
			List<MenuResult> menuEntries = new List<MenuResult>();

			foreach (var modulo in Menus.AllMenus)
			{
				foreach (var menu in modulo.Children)
				{
					string menuText = Resources.Resources.ResourceManager.GetString(menu.Title, cultureInfo);
					string moduleText = Resources.Resources.ResourceManager.GetString(modulo.Title, cultureInfo);
					var result = new MenuResult(menu.ID, modulo.ID, menuText, moduleText);

					if (string.IsNullOrWhiteSpace(menu.RoleId))
						menuEntries.AddRange(SearchableSubmenu(userContext, menu, result));
					else
					{
						var role = CSGenio.framework.Role.GetRole(menu.RoleId);
						if (userContext.User.VerifyAccess(role, modulo.ID))
							menuEntries.AddRange(SearchableSubmenu(userContext, menu, result));
					}
				}
			}

			return menuEntries;
		}

		private static List<MenuResult> SearchableSubmenu(UserContext userContext, MenuEntry menu, MenuResult parent)
		{
			List<MenuResult> menuEntries = new List<MenuResult>();

			if (menu.Type == "ITEM" && menu.TreeLevel > 0)
			{
				if (!menu.HasCondition || (menu.HasCondition && new MenuConditionValidator(userContext).ValidateCondition(menu, parent.Module)))
				{
					string menuText = Resources.Resources.ResourceManager.GetString(menu.Title);
					MenuResult result = new MenuResult(menu.ID, parent.Module, menuText, parent.ModuleText);
					string flatMenu = parent.FlatMenu;

					if (string.IsNullOrEmpty(parent.FlatMenu))
						flatMenu = menuText;
					else
						flatMenu += " > " + menuText;

					result.FlatMenu = flatMenu;

					foreach (var subMenu in menu.Children)
					{
						if (string.IsNullOrWhiteSpace(subMenu.RoleId))
							menuEntries.AddRange(SearchableSubmenu(userContext, subMenu, result));
						else
						{
							var role = CSGenio.framework.Role.GetRole(subMenu.RoleId);
							if (userContext.User.VerifyAccess(role, result.Module))
								menuEntries.AddRange(SearchableSubmenu(userContext, subMenu, result));
						}
					}
				}
			}
			else
			{
				parent.ActionId = menu.ID;
				menuEntries.Add(parent);
			}

			return menuEntries;
		}
	}
}
