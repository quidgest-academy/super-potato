using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls;

public class CheckboxGroupControl : ControlObject
{
    private IEnumerable<IWebElement> _items => m_container.FindElements(By.CssSelector(".q-checkbox"));

    public CheckboxGroupControl(IWebDriver driver, By containerLocator, string controlId)
        : base(driver, containerLocator, By.Id(controlId))
    {
    }

    public List<string> GetCheckedValues()
    {
        return _items
            .Where(label => label.GetAttribute("class")?.Contains("q-checkbox--checked") == true)
            .Select(label => label.Text.Trim())
            .ToList();
    }

    public void CheckValue(string value)
    {
        var checkbox = _items
            .FirstOrDefault(c => c.Text.Trim().Equals(value));

        if (checkbox == null)
            return;

        var isChecked = checkbox.GetAttribute("class")?.Contains("q-checkbox--checked") ?? false;
        if (!isChecked)
            checkbox.Click();
    }

    public void UncheckValue(string value)
    {
        var checkbox = _items
            .FirstOrDefault(l => l.Text.Trim().Equals(value));

        if (checkbox == null)
            return;

        var isChecked = checkbox.GetAttribute("class")?.Contains("q-checkbox--checked") ?? false;
        if (isChecked)
            checkbox.Click();
    }
}