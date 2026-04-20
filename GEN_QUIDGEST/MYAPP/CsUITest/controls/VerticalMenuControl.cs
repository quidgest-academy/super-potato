using AngleSharp.Text;

namespace quidgest.uitests.controls;

public class VerticalMenuControl : PageObject, IMenuControl
{

    private IWebElement navbar => driver.FindElement(By.CssSelector(".main-sidebar"));

    private IWebElement modules => navbar.FindElement(By.Id("modules-tree-view"));
    private IWebElement currentModule => modules.FindElement(By.CssSelector("li.nav-item a"));
    private IWebElement moduleList => modules.FindElement(By.Id("collapsible-modules"));

    private IWebElement bookmarks => navbar.FindElement(By.ClassName("bookmarks__container"));

    private IWebElement menus => navbar.FindElement(By.Id("menu-tree-view"));

    private MenuTree _menuTree;

    public VerticalMenuControl(IWebDriver driver, MenuTree menuTree) : base(driver)
    {
        _menuTree = menuTree;

        wait.Until(c => navbar);
        wait.Until(c => menus);
    }

    private void WaitForLoading()
    {
        wait.Until(c => menus);
        wait.Until(c => modules);
    }

    public void ActivateMenu(string moduleId, string itemId)
    {
        WaitForLoading();

        //Vertical menus do not render the subelements until the parent element is clicked
        // they don't even exist so we cannot FindElement to get the final element.
        //To solve this we need to generate the declaration of the menu tree to a class
        // then request the forward path to the destination element to it.
        //Another solution would be to make the test programmer to explicitly activate the
        // menus in the correct sequence, but that would be needlessly verbose for tests.

        var menuNode = _menuTree.FindMenu(moduleId, itemId);
        //recursively click on the parent menus until we can click on the one we want
        ClickParentRecursive(moduleId, menuNode);
    }

    private void ClickParentRecursive(string moduleId, MenuTreeNode node)
    {
        var parent = node.Parent;
        if (parent != null)
            ClickParentRecursive(moduleId, parent);

        var liTarget = menus.FindElement(By.Id(moduleId + node.Id));
        wait.Until(c => liTarget.Displayed);
        liTarget.AnimatedClick();
    }


    public void ActivateModule(string moduleId)
    {
        WaitForLoading();

        var cm = currentModule.GetAttribute("data-key");
        if (cm == moduleId) return;
        modules.Click();
        wait.Until(c => moduleList.Displayed);
        var item = moduleList.FindElement(ByData.Key(moduleId));
        wait.Until(c => item.Displayed);
        item.Click();
    }

    public void ActivateBookmark(string moduleId, string itemId)
    {
        if (!HasBookmark(moduleId, itemId))
            throw new InvalidOperationException($"Menu {moduleId}_{itemId} is not bookmarked.");

        bookmarks.Click();
        var bookmark = bookmarks.FindElement(ByData.Key($"bookmark_{moduleId}_{itemId}"));
        wait.Until(c => bookmark.Displayed);
        bookmark.FindElement(By.TagName("a")).Click();
    }

    public void AddBookmark(string moduleId, string itemId)
    {
        bookmarks.Click();
        var addButton = bookmarks.FindElement(By.CssSelector(".bookmarks__btn--add"));
        wait.Until(c => addButton.Displayed);
        addButton.Click();

        // The current implementation of favorites requires navigation to the chosen menu to create the bookmark.
        ActivateModule(moduleId);
        ActivateMenu(moduleId, itemId);

        wait.Until(c => HasBookmark(moduleId, itemId));
    }

    public void RemoveBookmark(string moduleId, string itemId)
    {
        bookmarks.Click();
        var bookmark = bookmarks.FindElement(ByData.Key($"bookmark_{moduleId}_{itemId}"));
        wait.Until(c => bookmark.Displayed);

        var removeButton = bookmark.FindElement(By.CssSelector(".bookmarks__btn--remove"));
        removeButton.Click();
        wait.Until(c => !HasBookmark(moduleId, itemId));

        bookmarks.Click();
    }

    public bool HasBookmark(string moduleId, string itemId)
    {
        return bookmarks.FindElements(ByData.Key($"bookmark_{moduleId}_{itemId}")).Count > 0;
    }

    public int GetMenuCount(string moduleId, string itemId)
    {
        WaitForLoading();

        // Get menu item element
        var menuNode = menus.FindElement(By.Id(moduleId + itemId));

        // Get record counter element
        IWebElement counterElem = menuNode
            ?.FindElement(By.CssSelector("a"))
            ?.FindElement(By.CssSelector("p"))
            ?.FindElement(By.CssSelector("span"))
            ?.FindElement(By.CssSelector("span"));

        // Get record counter element text
        string counterElemText = counterElem?.GetDomProperty("innerText");

        // Convert to integer
        return counterElemText == null ? 0 : counterElemText.ToInteger(0);
    }

    public int GetBookmarkCount()
    {
        return bookmarks.FindElements(By.CssSelector(".bookmarks__btn--link")).Count;
    }
}
