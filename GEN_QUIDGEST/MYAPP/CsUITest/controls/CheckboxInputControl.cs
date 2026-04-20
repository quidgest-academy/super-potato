namespace quidgest.uitests.controls;

public class CheckboxInputControl(IWebDriver driver, By containerLocator, string css) : ControlObject(driver, containerLocator, By.CssSelector(css))
{
    private IWebElement Checkbox => m_control.FindElement(By.CssSelector("[data-testid=checkbox-button]"));

    /// <summary>
    /// True if the control is blocked, false otherwise
    /// </summary>
    public bool IsBlocked => !m_control.FindElement(By.CssSelector("input")).Enabled
        || m_control.FindElement(By.CssSelector("input")).GetAttribute("readonly") != null;

    public bool GetValue()
    {
        return m_control.FindElement(By.CssSelector("input")).Selected;
    }

    public void Toggle()
    {
        Checkbox.Click();
    }

    public void SetValue(bool val)
    {
        if (GetValue() != val)
            Toggle();
    }
}
