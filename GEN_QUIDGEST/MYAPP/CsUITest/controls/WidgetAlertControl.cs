using System.Collections.Generic;

namespace quidgest.uitests.controls;

public class WidgetAlertControl : WidgetBaseControl
{
    public WidgetAlertControl(IWebDriver driver, By containerLocator, string css) :
    base(driver, containerLocator, css)
    { }

    protected IWebElement Counter => m_control.FindElement(By.CssSelector(".q-counter-widget"));
    protected IWebElement CounterValue => m_control.FindElement(By.CssSelector(".q-counter-widget__value"));

    public string GetTitle()
    {
        if (!IsVisible)
            throw new InvalidOperationException($"Widget with Id {id} is not present in the dashboard.");

        var title = m_control.FindElement(By.CssSelector(".q-counter-widget__title"));
        return title.Text;
    }

    public int GetCount()
    {
        if (!IsVisible)
            throw new InvalidOperationException($"Widget with Id {id} is not present in the dashboard.");
        
        wait.Until(c =>
            Counter.GetAttribute("data-loading") == null ||
            Counter.GetAttribute("data-loading") == "false");

        return int.Parse(CounterValue.Text);
    }
}
