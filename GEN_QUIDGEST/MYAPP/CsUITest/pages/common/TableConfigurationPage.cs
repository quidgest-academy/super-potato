namespace quidgest.uitests.pages;

public class TableConfigurationPage : PageObject
{
    /// <summary>
    /// Table configuration container element
    /// </summary>
    private IWebElement tableConfigurationContainer => GetElement(driver, By.CssSelector(".q-table-config"));

    /// <summary>
    /// Column configuration buttons
    /// </summary>
    private IWebElement resetColumnConfigBtn => GetElement(driver, By.Id("reset-column-config-btn"));
    private IWebElement applyColumnConfigBtn => GetElement(driver, By.Id("apply-config-btn"));
    private IWebElement cancelColumnConfigBtn => GetElement(driver, By.Id("cancel-config-btn"));

    /// <summary>
    /// Column configuration table
    /// </summary>
    public ListControl columnConfigList => new ListControl(driver, By.CssSelector(".q-table-config"), "[data-testid='table-container']");

    public TableConfigurationPage(IWebDriver driver) : base(driver)
    {
        wait.Until(c => tableConfigurationContainer != null);
    }

    /// <summary>
    /// Reset the column configuration
    /// </summary>
    public void ResetColumnConfig()
    {
        if (resetColumnConfigBtn == null)
            return;

        resetColumnConfigBtn.Click();
    }

    /// <summary>
    /// Apply the column configuration
    /// </summary>
    public void ApplyColumnConfig()
    {
        if (applyColumnConfigBtn == null)
            return;

        applyColumnConfigBtn.Click();
    }

    /// <summary>
    /// Cancel changes to the column configuration
    /// </summary>
    public void CancelColumnConfig()
    {
        if (cancelColumnConfigBtn == null)
            return;

        cancelColumnConfigBtn.Click();
    }
}
