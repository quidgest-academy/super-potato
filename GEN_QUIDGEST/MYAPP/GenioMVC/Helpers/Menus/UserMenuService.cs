using System.Collections.Generic;
using System.Linq;
using CSGenio;
using CSGenio.framework;

namespace GenioMVC.Helpers.Menus
{
    /// <summary>
    /// Determines whether a user has access to menu modules.
    /// </summary>
    public class UserMenuService
    {
        private readonly IMenuLoader _menuLoader;
        private readonly IMenuConditionValidator _conditionValidator;
        private readonly User _user;

        public UserMenuService(IMenuLoader menuLoader, IMenuConditionValidator conditionValidator, User user)
        {
            _menuLoader = menuLoader;
            _conditionValidator = conditionValidator;
            _user = user;
        }

        /// <summary>
        /// Checks whether the user has access to at least one menu entry in the specified module.
        /// Returns false early for unknown or public modules without loading menus.
        /// </summary>
        /// <param name="moduleName">The module identifier to check access for.</param>
        public bool UserHasAccessToModule(string moduleName)
        {
            if (moduleName == "Public" || !Configuration.Modules.Contains(moduleName))
                return false;

            var allMenus = _menuLoader.GetAllMenus();
            var moduleEntry = allMenus.FirstOrDefault(m => m.ID == moduleName);

            if (moduleEntry?.Children == null)
                return false;

            return HasAccessibleEntry(moduleEntry.Children, moduleName);
        }

        private static bool IsActionable(MenuEntry entry)
        {
            return (entry.Controller != null && entry.Action != null && entry.Action_MVC != null)
                || entry.Action == "GenGenio.MenuPaginaWeb";
        }

        /// <summary>
        /// Depth-first two-phase check for accessible actionable entries.
        /// Phase 1: searches for unconditional access without evaluating any conditions.
        /// Phase 2: validates conditions only if no unconditional path was found.
        /// A failing condition blocks its entire subtree.
        /// </summary>
        private bool HasAccessibleEntry(List<MenuEntry> entries, string module)
        {
            if (entries == null)
                return false;

            var conditionalEntries = new List<MenuEntry>();

            // Phase 1: look for unconditional accessible entries, defer conditional ones
            foreach (var entry in entries)
            {
                // TreeLevel == -1 entries (menu continuations) skip role checks — access is controlled by their parent
                if (entry.TreeLevel > -1 && !entry.Allows(_user, module))
                    continue;

                if (entry.HasCondition)
                {
                    conditionalEntries.Add(entry);
                    continue;
                }

                if (IsActionable(entry))
                    return true;

                // Unconditional container — recurse into children
                if (HasAccessibleEntry(entry.Children, module))
                    return true;
            }

            // Phase 2: validate conditions only if no unconditional path was found (case where all possibl menus have conditions)
            foreach (var entry in conditionalEntries)
            {
                if (!_conditionValidator.ValidateCondition(entry, module))
                    continue;

                if (IsActionable(entry))
                    return true;

                // Condition passed on a container — check its subtree
                if (HasAccessibleEntry(entry.Children, module))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the list of module entries the user has access to.
        /// </summary>
        public List<MenuEntry> GetAvailableModules()
        {
            var result = new List<MenuEntry>();
            foreach (var mod in _menuLoader.GetAllMenus())
            {
                if (UserHasAccessToModule(mod.ID))
                    result.Add(mod);
            }
            return result;
        }

    }
}
