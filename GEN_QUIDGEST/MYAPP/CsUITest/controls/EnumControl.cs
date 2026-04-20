using System.Linq;

namespace quidgest.uitests.controls;

public class EnumControl:DropdownControl
{
    protected IWebElement _button => _display.FindElement(By.CssSelector(".q-select__chevron"));

    protected IWebElement _value => _display.FindElement(By.CssSelector(".q-select__value"));

    public EnumControl(IWebDriver driver, By containerLocator, By controlLocator)
        : base(driver, containerLocator, controlLocator)
    {
    }

    public EnumControl(IWebDriver driver, By containerLocator, string controlId)
        : base(driver, containerLocator, controlId)
    {
    }

    public override string GetValue()
    {
        WaitForLoad();

        try
        {
            return _value.GetDomProperty("textContent");
        }
        catch (NoSuchElementException)
        {
            // The value element is only rendered when the select has a non-empty value.
            // If it's not present, it means the select currently has no value.
            return null;
        }
    }

    public override void SetValue(string val)
    {
        WaitForLoad();

        if (val.Equals(GetValue()))
            return;

        _button.Click();
        int ix = GetRowByText(val);
        
        if (ix != -1)
            _rows.ElementAt(ix).Click();
    }
}
