using System;
using System.Linq;
using System.Collections.Generic;

using CSGenio.business;
using GenioMVC.Models.Navigation;
using Quidgest.Persistence.GenericQuery;

namespace GenioMVC.ViewModels.Dashboard
{
	public abstract class WidgetProvider : WidgetProperties
	{
		protected List<Widget> m_instances;

		/// <summary>
		/// The list of instances of this widget type that this user has access to
		/// </summary>
		public List<Widget> UserWidgets(UserContext userContext)
		{
			return m_instances.Where(i => i.UserHasAccess(userContext)).ToList();
		}

		public abstract void LoadInstances(UserContext userContext);

		public Widget GetInstance(string widgetId)
		{
			return m_instances.FirstOrDefault(w => w.Id == widgetId);
		}
	}

	public class CustomWidgetProvider<T> : WidgetProvider where T : DbArea
	{
		public delegate ListingMVC<A> GetRowsDelegate<A>(UserContext userContext, CriteriaSet args = null) where A : T;

		public delegate CriteriaSet GetWidgetConditions();

		public delegate Logical IsInstanceVisible(UserContext userContext, T row);

		public GetRowsDelegate<T> RowsSelector;

		public IsInstanceVisible InstanceVisible { get; set; } = (userContext, row) => true ;

		public GetWidgetConditions Limits;

		public string Form { get; set; }

		public string Component { get; set; }

		public string[] FieldsOnText { get; set; }

		private bool UsesAggregateMethod
		{
			get
			{
				return InstantionMethod == WidgetInstantionMethod.Aggregate
					|| InstantionMethod == WidgetInstantionMethod.Both;
			}
		}

		private bool UsesSplitMethod
		{
			get
			{
				return InstantionMethod == WidgetInstantionMethod.Split
					|| InstantionMethod == WidgetInstantionMethod.Both;
			}
		}

		public override void LoadInstances(UserContext userContext)
		{
			m_instances = new List<Widget>();

			CriteriaSet args = null;
			if (Limits != null)
			{
				args = Limits();
				// If the function for obtaining the limitations returns nothing,
				// it only happens when there is a missing limit and it cannot be applied.
				// The same happens with menu dbedits. In this case, it is the same as not having the rows.
				if (args == null)
					return;
			}

			List<T> rows = null;
			if (RowsSelector != null)
				rows = RowsSelector(userContext, args).Rows;

			List<T> visibleRows = null;
			if(rows != null)
				visibleRows = rows.Where(row => InstanceVisible(userContext, row)).ToList();

			// Has base area, one instance per row
			if (visibleRows != null && UsesSplitMethod)
			{
				foreach (var row in visibleRows)
				{
					// Support dynamic titles
					string title = Title;
					if (FieldsOnText != null)
					{
						List<string> values = new List<string>();
						foreach (var field in FieldsOnText)
							values.Add((string) row.returnValueField(field));

						title = string.Format(title, values.ToArray());
					}

					m_instances.Add(
						new CustomWidget()
						{
							Id = Id,
							Rowkey = row.QPrimaryKey,
							Order = Order,
							Width = Width,
							Height = Height,
							Required = Required,
							Visible = Visible,
							Role = Role,
							Module = Module,
							Title = title,
							RefreshMode = RefreshMode,
							RefreshRate = RefreshRate,
							UsesCache = UsesCache,
							CacheTTL = CacheTTL,
							Group = Group,
							Form = Form,
							Component = Component
						}
					);
				}
			}

			// Has base area, paginated
			if (visibleRows != null && UsesAggregateMethod)
			{
				List<string> keys = visibleRows.Select(row => row.QPrimaryKey).ToList();

				m_instances.Add(
					new CustomPaginatedWidget()
					{
						Id = Id,
						Keys = keys,
						Order = Order,
						Width = Width,
						Height = Height,
						Required = Required,
						Visible = Visible,
						Style = Style,
						BorderStyle = BorderStyle,
						Role = Role,
						Module = Module,
						Title = Title,
						RefreshMode = RefreshMode,
						RefreshRate = RefreshRate,
						UsesCache = UsesCache,
						CacheTTL = CacheTTL,
						Group = Group,
						Form = Form,
						Component = Component
					}
				);
			}

			// "Empty form" 
			// Empty forms don't have rows soo we can check ShowWidget directly
			if (visibleRows == null && this.ShowWidget)
			{
				m_instances.Add(
					new CustomWidget()
					{
						Id = Id,
						Order = Order,
						Width = Width,
						Height = Height,
						Required = Required,
						Visible = Visible,
						Style = Style,
						BorderStyle = BorderStyle,	
						Role = Role,
						Module = Module,
						Title = Title,
						RefreshMode = RefreshMode,
						RefreshRate = RefreshRate,
						UsesCache = UsesCache,
						CacheTTL = CacheTTL,
						Group = Group,
						Form = Form,
						Component = Component
					}
				);
			}
		}
	}

	public class BookmarkWidgetProvider : WidgetProvider
	{
		public string ButtonText { get; set; }

		public override void LoadInstances(UserContext userContext)
		{
			m_instances = new List<Widget>();

			var ckey = string.Format(
				"bookmarks.{0}.{1}",
				userContext.User.Name,
				userContext.User.Codpsw
			);

			var model = CSGenio.framework.QCache.Instance.User.Get(ckey)
				as ViewModels.Bookmarks.Bookmarks_ViewModel;

			if (model == null)
			{
				model = new ViewModels.Bookmarks.Bookmarks_ViewModel();
				model.LoadMenus(userContext);

				CSGenio.framework.QCache.Instance.User.Put(ckey, model, TimeSpan.FromMinutes(15));
			}

			foreach (var bookmark in model.Bookmarks)
			{
				if (bookmark.MenuEntryObj != null)
				{
					m_instances.Add(
						new BookmarkWidget(bookmark)
						{
							Id = "Bookmark_" + bookmark.Module + "_" + bookmark.MenuID,
							Order = Order,
							Width = Width,
							Height = Height,
							Required = Required,
							Style = Style,	
							BorderStyle = BorderStyle,						
							Visible = Visible,
							Title = GenioMVC.Helpers.Helpers.GetTextFromResources(bookmark.MenuEntryObj.Title),
							Group = "BOOKMARKS",
							ButtonText = ButtonText
						}
					);
				}
			}
		}
	}
}
