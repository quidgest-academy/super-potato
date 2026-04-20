using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace quidgest.uitests.controls;

public class BaseDashboardControl : ControlObject
{
    private bool loading => m_control.FindElements(By.CssSelector("[data-loading=true]")).Any();

    public bool IsEmpty => m_control.FindElements(By.CssSelector(".grid-stack > .no-widgets")).Any();

    public int VisibleWidgetCount => m_control.FindElements(By.CssSelector(".q-widget")).Count;

    public BaseDashboardControl(IWebDriver driver, By containerLocator, string css) :
        base(driver, containerLocator, By.CssSelector(css))
    {
        WaitForLoading();
    }

    /// <summary>
    /// Wait for page to load
    /// </summary>
    private void WaitForLoading()
    {
        wait.Until(c => !loading);
    }
}
