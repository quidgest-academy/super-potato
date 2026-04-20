using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class BasePropertyListControl : ControlObject
{
    public BasePropertyListControl(IWebDriver driver, By containerLocator, By css)
        : base(driver, containerLocator, css)
    {
        WaitForLoad();
    }
    
    protected IList<IWebElement> Rows => m_control.FindElements(By.CssSelector(".q-property-list__row"));

    public int RowCount => Rows.Count;

    private void WaitForLoad()
   {
        wait.Until(c => m_control.GetAttribute("data-loading") == "false" || m_control.GetAttribute("data-loading") == null);
   }

    /// <summary>
    /// Loses focus on the control
    /// </summary>
    public void LoseFocus()
    {
        // Click on the body or another safe area to lose focus
        var body = driver.FindElement(By.TagName("body"));
        body.Click(); ;
    }
}
