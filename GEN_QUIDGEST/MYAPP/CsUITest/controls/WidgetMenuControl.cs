using System.Collections.Generic;

namespace quidgest.uitests.controls;

public class WidgetMenuControl : WidgetBaseControl
{
    public WidgetMenuControl(IWebDriver driver, By containerLocator, string css) :
    base(driver, containerLocator, css)
    { }

    public string GetTitle()
    {
        if (!IsVisible)
            throw new InvalidOperationException($"Widget with Id {id} is not present in the dashboard.");

        var title = m_control.FindElement(By.CssSelector(".q-menu-widget__title"));
        return title.Text;
    }
}
