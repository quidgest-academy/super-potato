using quidgest.uitests.pages.forms.core;

#nullable enable

namespace quidgest.uitests.pages.forms;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class CountryForm : Form
{
	/// <summary>
	/// Country
	/// </summary>
	public BaseInputControl CountCountry => new BaseInputControl(driver, ContainerLocator, "container-COUNTRY_COUNTCOUNTRY_" + IdSuffix, "#COUNTRY_COUNTCOUNTRY_" + IdSuffix);

	public CountryForm(IWebDriver driver, FORM_MODE mode, By? containerLocator = null, bool usePkInId = false)
		: base(driver, mode, "COUNTRY", containerLocator: containerLocator, usePkInId: usePkInId) { }
}
