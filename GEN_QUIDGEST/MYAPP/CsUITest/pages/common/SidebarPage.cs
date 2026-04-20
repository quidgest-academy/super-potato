using System.ComponentModel;

namespace quidgest.uitests.pages;

public class SidebarPage: PageObject
{
    /// <summary>
    /// App layout container locator
    /// </summary>
	private By containerLocator;

    /// <summary>
    /// App layout container element
    /// </summary>
	private IWebElement Container;

    /// <summary>
    /// Sidebar container locator
    /// </summary>
    private By sidebarContainerLocator => By.Id("right-sidenav");

    /// <summary>
    /// Sidebar container element
    /// </summary>
    private IWebElement sidebarContainer => GetElement(Container, sidebarContainerLocator);

    /// <summary>
    /// Sidebar buttons
    /// </summary>
    public ButtonControl OpenButton => new ButtonControl(driver, containerLocator, "#open-sidebar-btn");
    public ButtonControl CloseButton => new ButtonControl(driver, sidebarContainerLocator, "#close-sidebar-btn");
    public ButtonControl AdvancedReportButton => new ButtonControl(driver, sidebarContainerLocator, "#advanced-report-mode-toggle");
    public ButtonControl FormActionsButton => new ButtonControl(driver, sidebarContainerLocator, "#form-actions-toggle");
    public ButtonControl AlertsButton => new ButtonControl(driver, sidebarContainerLocator, "#alerts-btn");
    public ButtonControl SuggestionModeButton => new ButtonControl(driver, sidebarContainerLocator, "#suggestion-mode-toggle");
    public ButtonControl ChatbotButton => new ButtonControl(driver, sidebarContainerLocator, "#chatbot-toggle");

    /// <summary>
    /// Sidebar content container
    /// </summary>
    private IWebElement contentContainer => Container.FindElement(By.CssSelector(".c-right-sidebar__content"));

    /// <summary>
    /// Whether the sidebar is open
    /// </summary>
    public bool IsOpen => sidebarContainer.Displayed == true;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="driver">Web driver</param>
    /// <param name="containerLocator">App layout container locator</param>
	/// <returns></returns>
    public SidebarPage(IWebDriver driver, By containerLocator) : base(driver) {
        this.containerLocator = containerLocator;
        this.Container = driver.FindElement(containerLocator);
    }

    /// <summary>
    /// Open the sidebar
    /// </summary>
    public void Open()
    {
        if (!IsOpen)
            OpenButton.Click();
        wait.Until(c => IsOpen);
    }

    /// <summary>
    /// Close the sidebar
    /// </summary>
    public void Close()
    {
        if (IsOpen)
            CloseButton.Click();
        wait.Until(c => !IsOpen);
    }
}
