namespace quidgest.uitests.controls;

public class TabControl : ControlObject
{
    public TabControl(IWebDriver driver, By containerLocator, string css)
        : base(driver, containerLocator, By.CssSelector(css))
    {
    }

    public bool IsOpen => m_control.GetAttribute("class").Contains("active");

    public void Activate()
    {
        m_control.Click();
    }
}