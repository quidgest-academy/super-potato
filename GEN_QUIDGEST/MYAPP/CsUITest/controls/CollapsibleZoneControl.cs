namespace quidgest.uitests.controls;

public class CollapsibleZoneControl : ControlObject
{
    private IWebElement toggle => m_control.FindElement(By.CssSelector(".q-collapsible__header button"));

    public CollapsibleZoneControl(IWebDriver driver, By containerLocator, string css)
        : base(driver, containerLocator, By.CssSelector(css))
    {
    }

    public bool IsExpanded
    {
        get
        {
            IWebElement m_content = m_control.FindElement(By.CssSelector(".q-card__content > .q-collapsible__content-wrapper"));
            return m_content.GetAttribute("class").Contains("q-collapsible__content-show");
        }
    }

    public void Toggle()
    {
        toggle.Click();
    }
}
