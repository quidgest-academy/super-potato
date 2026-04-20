using System.Globalization;

namespace quidgest.uitests.controls;

public class DateInputControl : InputControl
{
    private IWebElement input => m_control.FindElement(By.CssSelector("input.dp__input"));

    private readonly string format;

    public DateInputControl(IWebDriver driver, By containerLocator, string css, string format = "dd/MM/yyyy") 
        : base(driver, containerLocator, By.CssSelector(css))
    {
        this.format = format;
    }

    public DateTime? GetValue()
    {
        var v = input.GetAttribute("value");
        if (string.IsNullOrEmpty(v))
            return null;
        return DateTime.ParseExact(v, format, CultureInfo.InvariantCulture);
    }

    public override void SetValue(string val)
    {
        input.Clear();
        input.SendKeys(val);
        input.SendKeys(Keys.Return);
    }

    public void SetValue(DateTime? val)
    {
        if (val.HasValue)
        {
            var v = val.Value.ToString(format, CultureInfo.InvariantCulture);
            SetValue(v);
        }
        else
            input.Clear();
    }
}