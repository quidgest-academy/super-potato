namespace quidgest.uitests.controls;

public class ButtonControl : ControlObject
{
    public ButtonControl(IWebDriver driver, By containerLocator, string css) 
        : base(driver, containerLocator, By.CssSelector(css))
    {
    }

    public void Click()
    {
        m_control.Click();
    }
}