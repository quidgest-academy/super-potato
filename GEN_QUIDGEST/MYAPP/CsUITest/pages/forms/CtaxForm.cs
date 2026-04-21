using quidgest.uitests.pages.forms.core;

#nullable enable

namespace quidgest.uitests.pages.forms;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class CtaxForm : Form
{
	/// <summary>
	/// City
	/// </summary>
	public LookupControl CityCity => new LookupControl(driver, ContainerLocator, "container-CTAX____CITY_CITY____" + IdSuffix);
	public SeeMorePage CityCitySeeMorePage => new SeeMorePage(driver, "CTAX", "CTAX____CITY_CITY____" + IdSuffix);

	/// <summary>
	/// Tax
	/// </summary>
	public BaseInputControl CtaxTax => new BaseInputControl(driver, ContainerLocator, "container-CTAX____CTAX_TAX_____" + IdSuffix, "#CTAX____CTAX_TAX_____" + IdSuffix);

	public CtaxForm(IWebDriver driver, FORM_MODE mode, By? containerLocator = null, bool usePkInId = false)
		: base(driver, mode, "CTAX", containerLocator: containerLocator, usePkInId: usePkInId) { }
}
