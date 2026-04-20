using System;
using System.Collections.Generic;
using System.Linq;

using CSGenio.business;
using CSGenio.framework;
using GenioMVC.Helpers.Menus;
using GenioMVC.Models.Navigation;

namespace GenioMVC.ViewModels.Bookmarks
{
	[Serializable]
	public class Bookmark_Menu_ViewModel
	{
		public string Codusrcfg { get; set; }

		public string Module { get; set; }

		public string MenuID { get; set; }

		public string Description { get; set; }

		public MenuEntry MenuEntryObj {get; set; }
	}

	[Serializable]
	public class Bookmarks_ViewModel
	{
		private List<Bookmark_Menu_ViewModel> _bookmarks;
		public List<Bookmark_Menu_ViewModel> Bookmarks { get => _bookmarks; set => _bookmarks = value; }

		public Bookmarks_ViewModel()
		{
			this._bookmarks = new List<Bookmark_Menu_ViewModel>();
		}

		public void LoadMenus(UserContext userContext)
		{
			var user = userContext.User;
			var sp = userContext.PersistentSupport;

			try
			{
				var favs = CSGenioAusrcfg.searchListBookmarks(sp, user);

				foreach (var fav in favs.OrderBy(fav => fav.Module).ThenBy(fav => fav.ValId))
				{
					if (user.IsAuthorized(fav.ValModulo))
					{
						var _mId = fav.ValId;
						MenuEntry menu = null;
						try
						{
							menu = Helpers.Menus.Menus.FindMenuForUserRec(userContext, fav.ValModulo, fav.ValId, true);
						}
						catch (KeyNotFoundException)
						{
							// Menu ceased to exist
							// Invalid bookmark
						}

						var pathText = Resources.Resources.MARCADOR_INVALIDO22981;
						var path = Menus.MenuTextPath(fav.ValModulo, fav.ValId);
						if (path.Any())
							pathText = fav.ValModulo + " > " + string.Join(" > ", path);

						_bookmarks.Add(new Bookmark_Menu_ViewModel()
						{
							Codusrcfg = fav.ValCodusrcfg,
							MenuID = fav.ValId,
							Module = fav.ValModulo,
							Description = pathText,
							MenuEntryObj = menu
						});
					}
				}
			}
			catch (Exception e)
			{
				sp.closeConnection();
				Log.Error(String.Format("On Load bookmarks. Message: {0}", e.Message));
			}
		}
	}
}
