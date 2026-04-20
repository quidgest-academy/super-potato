using System.Collections.Generic;
using GenioMVC.Helpers.Menus;

namespace GenioMVC.UnitTests
{
    /// <summary>
    /// Test implementation of IMenuLoader that returns a preconfigured menu list.
    /// Tracks whether GetAllMenus was called via the WasLoaded property.
    /// </summary>
    public class TestMenuLoader : IMenuLoader
    {
        private readonly List<MenuEntry> _menus;

        public bool WasLoaded { get; private set; }

        public TestMenuLoader(List<MenuEntry> menus = null)
        {
            _menus = menus ?? new List<MenuEntry>();
        }

        public List<MenuEntry> GetAllMenus()
        {
            WasLoaded = true;
            return _menus;
        }
    }
}
