using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC.Models.Navigation;
using System.Linq.Expressions;

namespace GenioMVC.Helpers.Menus
{
	/// <summary>
	/// Validates show-when conditions on menu entries.
	/// </summary>
	public interface IMenuConditionValidator
	{
		/// <summary>
		/// Returns true if the menu entry's condition is satisfied for the given module.
		/// </summary>
		bool ValidateCondition(MenuEntry menuEntry, string module);
	}

	/// <summary>
	/// Validates menu conditions using the generated condition logic.
	/// </summary>
	public class MenuConditionValidator : IMenuConditionValidator
	{
		private readonly UserContext _userContext;

		public MenuConditionValidator(UserContext userContext)
		{
			_userContext = userContext;
		}

		public bool ValidateCondition(MenuEntry menuEntry, string module)
		{
			UserContext m_userContext = _userContext;
			var user = _userContext.User;
			var ps = _userContext.PersistentSupport;
			string currentModule = (string.IsNullOrEmpty(module) ? user.CurrentModule : module);


			// The menu ID must be "Module + ID"
			string menuID = currentModule + menuEntry.ID;

			switch (menuID)
			{
				default:
					break;
			}

			return false;
		}
	}
}
