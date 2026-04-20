using System;
using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class DropdownMenuControl : ControlObject
{
    /// <summary>
    /// CSS selector for the main button element.
    /// </summary>
    private string toggleButtonSelector;

    /// <summary>
    /// CSS selector for the dropdown menu element.
    /// </summary>
    private string dropdownSelector;

    /// <summary>
    /// CSS selector for the option element.
    /// </summary>
    private string itemSelector;

    /// <summary>
    /// Main button to open and close dropdown menu.
    /// </summary>
    private ButtonControl toggleButton => new ButtonControl(driver, m_containerLocator, this.toggleButtonSelector);

    /// <summary>
    /// Dropdown menu.
    /// </summary>
    private IWebElement dropdown => GetElement(By.CssSelector(dropdownSelector));

    /// <summary>
    /// Underlay element behind dropdown when it's open.
    /// </summary>
    private IWebElement underlay => GetElement(By.CssSelector(".q-overlay__underlay"));

    /// <summary>
    /// Whether the dropdown menu is open or not.
    /// </summary>
    private Boolean isOpen => dropdown != null;

    /// <summary>
    /// Option elements in the dropdown menu.
    /// </summary>
    private List<IWebElement> options => dropdown?.FindElements(By.CssSelector(itemSelector)).ToList();

    public DropdownMenuControl(IWebDriver driver, By containerLocator, string toggleButtonSelector, string dropdownSelector, string itemSelector) 
        : base(driver, containerLocator, By.CssSelector(toggleButtonSelector))
    {
        this.toggleButtonSelector = toggleButtonSelector;
        this.dropdownSelector = dropdownSelector;
        this.itemSelector = itemSelector;
    }

    /// <summary>
    /// Open the dropdown menu.
    /// </summary>
    public void OpenMenu()
    {
        if (isOpen) return;

        toggleButton.Click();
        wait.Until(c => isOpen);
    }

    /// <summary>
    /// Close the dropdown menu.
    /// </summary>
    public void CloseMenu()
    {
        if (!isOpen) return;

        underlay.Click();
        wait.Until(c => !isOpen);
    }

    /// <summary>
    /// Get the index of an option by name.
    /// </summary>
    /// <param name="name">The name of the option. The text content of it's element.</param>
    /// <returns>Index of the option with this name or -1 if no option has this name.</returns>
    public int GetOptionNameIndex(string name)
    {
        bool dropdownOpen = isOpen;

        if (!dropdownOpen) OpenMenu();

        List<string> dropdownOptions = options.Select(o => o.Text).ToList();

        if (!dropdownOpen) CloseMenu();

        for (int index = 0; index < dropdownOptions.Count; index++)
        {
            // If option has this name
            if (dropdownOptions.ElementAt(index).Equals(name, StringComparison.OrdinalIgnoreCase))
                return index;
        }

        // No option with this name
        return -1;
    }

    /// <summary>
    /// Select and option in the dropdown by index.
    /// </summary>
    /// <param name="index">The option index, starting from 0.</param>
    public void SelectOption(int index)
    {
        OpenMenu();

        if (index < 0 || index >= options.Count)
        {
            CloseMenu();
            return;
        }

        options[index].Click();
    }

    /// <summary>
    /// Select and option in the dropdown by name.
    /// </summary>
    /// <param name="name">The option name. It's text content.</param>
    public void SelectOption(string name)
    {
        OpenMenu();

        int index = GetOptionNameIndex(name);

        if (index < 0 || index >= options.Count)
        {
            CloseMenu();
            return;
        }

        options[index].Click();
    }
}