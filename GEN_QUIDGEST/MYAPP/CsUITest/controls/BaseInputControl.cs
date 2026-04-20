namespace quidgest.uitests.controls;

public class BaseInputControl(IWebDriver driver, By containerLocator, string controlId, string inputId) : InputControl(driver, containerLocator, By.CssSelector(inputId))
{
    /// <summary>
    /// True if the control is blocked, false otherwise
    /// </summary>
    public bool IsBlocked => m_container.FindElement(By.CssSelector($"#{controlId} > div.q-field")).GetAttribute("class").Contains("q-field--readonly");

    /// <summary>
    /// Get the input's value
    /// </summary>
    public string GetValue()
    {
        return m_control.GetAttribute("value");
    }

    /// <summary>
    /// Set the input's value
    /// </summary>
    public override void SetValue(string val)
    {
        ClearValue();
        m_control.SendKeys(val);
    }

    /// <summary>
    /// Clear the input's value. The built-in Clear() method does not always work but this does
    /// </summary>
    public void ClearValue()
    {
        m_control.SendKeys(Keys.Control + "a");
        m_control.SendKeys(Keys.Delete);
    }

    /// <summary>
    /// Confirm the input's value
    /// </summary>
    public void Confirm()
	{
		m_control.SendKeys(Keys.Enter);
	}
}
