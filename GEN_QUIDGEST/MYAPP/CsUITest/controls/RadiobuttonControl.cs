using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class RadiobuttonControl : ControlObject
{
    // TODO: maybe change to testid = "radio"
    private IEnumerable<IWebElement> _items => m_container.FindElements(By.CssSelector("input[type=radio]"));

    public RadiobuttonControl(IWebDriver driver, By containerLocator, string controlId)
        : base(driver, containerLocator, By.Id(controlId))
    {
    }

    public string GetValue()
    {
        var elem = _items.FirstOrDefault(e => e.GetAttribute("checked") != null);
        return elem?.GetAttribute("value");
    }

    public void SetValue(string value)
    {
        var elem = _items.FirstOrDefault(e => e.GetAttribute("value") == value);

        // The click must be done on the parent.
        var parent = elem?.FindElement(By.XPath("./.."));
        parent?.Click();
    }
}
