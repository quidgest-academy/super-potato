using quidgest.uitests.pages.forms.core;

#nullable enable

namespace quidgest.uitests.pages.forms;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class CityForm : Form
{
	/// <summary>
	/// City
	/// </summary>
	public BaseInputControl CityCity => new BaseInputControl(driver, ContainerLocator, "container-CITY____CITY_CITY____" + IdSuffix, "#CITY____CITY_CITY____" + IdSuffix);

	/// <summary>
	/// Country
	/// </summary>
	public LookupControl CountCountry => new LookupControl(driver, ContainerLocator, "container-CITY____COUNTCOUNTRY_" + IdSuffix);
	public SeeMorePage CountCountrySeeMorePage => new SeeMorePage(driver, "CITY", "CITY____COUNTCOUNTRY_" + IdSuffix);

	public CityForm(IWebDriver driver, FORM_MODE mode, By? containerLocator = null, bool usePkInId = false)
		: base(driver, mode, "CITY", containerLocator: containerLocator, usePkInId: usePkInId) { }
}
