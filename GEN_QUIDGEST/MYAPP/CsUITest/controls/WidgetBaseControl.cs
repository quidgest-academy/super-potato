using System.Collections.Generic;

namespace quidgest.uitests.controls;

public class WidgetBaseControl : ControlObject
{
    public WidgetBaseControl(IWebDriver driver, By containerLocator, string css) :
    base(driver, containerLocator, By.CssSelector(css))
    { }

    internal string id => m_control.GetAttribute("id");

    public bool IsVisible => m_control != null;

    public string GetGroupTitle()
    {
        if (!IsVisible)
            throw new InvalidOperationException($"Widget with Id {id} is not present in the dashboard.");

        var groupTitle = m_control.FindElement(By.CssSelector(".q-widget__group"));

        if (groupTitle == null)
            return "";

        return groupTitle.Text;
    }

    public void ExecuteAction()
    {
        m_control.Click();
    }
}
