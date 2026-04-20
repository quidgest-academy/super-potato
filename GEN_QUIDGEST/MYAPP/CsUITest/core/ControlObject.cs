namespace quidgest.uitests.core;

/// <summary>
/// A Control is any UI that belongs to a container,
/// and all its own locators are scoped by the container.
/// </summary>
public class ControlObject : PageObject
{
    protected readonly By m_containerLocator;
    protected IWebElement m_container => driver.FindElement(m_containerLocator);

    protected readonly By m_controlLocator;
    protected IWebElement m_control => m_container.FindElement(m_controlLocator);

    public ControlObject(IWebDriver driver, By containerLocator, By controlLocator) : base(driver)
    {
        m_containerLocator = containerLocator;
        m_controlLocator = controlLocator;

        wait.Until(c => m_control);
    }
}
