using quidgest.uitests.pages.forms.core;

#nullable enable

namespace quidgest.uitests.pages.forms;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class AgentForm : Form
{
	/// <summary>
	/// Agent's name
	/// </summary>
	public BaseInputControl AgentName => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTNAME____" + IdSuffix, "#AGENT___AGENTNAME____" + IdSuffix);

	/// <summary>
	/// Birthdate
	/// </summary>
	public DateInputControl AgentBirthdat => new DateInputControl(driver, ContainerLocator, "#AGENT___AGENTBIRTHDAT" + IdSuffix);

	/// <summary>
	/// Age
	/// </summary>
	public BaseInputControl AgentAge => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTAGE_____" + IdSuffix, "#AGENT___AGENTAGE_____" + IdSuffix);

	/// <summary>
	/// E-mail
	/// </summary>
	public BaseInputControl AgentEmail => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTEMAIL___" + IdSuffix, "#AGENT___AGENTEMAIL___" + IdSuffix);

	/// <summary>
	/// Telephone
	/// </summary>
	public BaseInputControl AgentTelephon => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTTELEPHON" + IdSuffix, "#AGENT___AGENTTELEPHON" + IdSuffix);

	/// <summary>
	/// Country of birth
	/// </summary>
	public LookupControl CbornCountry => new LookupControl(driver, ContainerLocator, "container-AGENT___CBORNCOUNTRY_" + IdSuffix);
	public SeeMorePage CbornCountrySeeMorePage => new SeeMorePage(driver, "AGENT", "AGENT___CBORNCOUNTRY_" + IdSuffix);

	/// <summary>
	/// Country of residence
	/// </summary>
	public LookupControl CaddrCountry => new LookupControl(driver, ContainerLocator, "container-AGENT___CADDRCOUNTRY_" + IdSuffix);
	public SeeMorePage CaddrCountrySeeMorePage => new SeeMorePage(driver, "AGENT", "AGENT___CADDRCOUNTRY_" + IdSuffix);

	/// <summary>
	/// Number of properties
	/// </summary>
	public BaseInputControl AgentNrprops => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTNRPROPS_" + IdSuffix, "#AGENT___AGENTNRPROPS_" + IdSuffix);

	/// <summary>
	/// Profit
	/// </summary>
	public BaseInputControl AgentProfit => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTPROFIT__" + IdSuffix, "#AGENT___AGENTPROFIT__" + IdSuffix);

	/// <summary>
	/// AveragePrice
	/// </summary>
	public BaseInputControl AgentAverage_price => new BaseInputControl(driver, ContainerLocator, "container-AGENT__AGENT__AVERAGE_PRICE" + IdSuffix, "#AGENT__AGENT__AVERAGE_PRICE" + IdSuffix);

	/// <summary>
	/// Last property sold (price)
	/// </summary>
	public BaseInputControl AgentLastprop => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTLASTPROP" + IdSuffix, "#AGENT___AGENTLASTPROP" + IdSuffix);

	/// <summary>
	/// Photography
	/// </summary>
	public BaseInputControl AgentPhotogra => new BaseInputControl(driver, ContainerLocator, "container-AGENT___AGENTPHOTOGRA" + IdSuffix, "#AGENT___AGENTPHOTOGRA" + IdSuffix);

	/// <summary>
	/// Agent information
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp01 => new CollapsibleZoneControl(driver, ContainerLocator, "#AGENT___PSEUDNEWGRP01" + IdSuffix + "-container");

	public AgentForm(IWebDriver driver, FORM_MODE mode, By? containerLocator = null, bool usePkInId = false)
		: base(driver, mode, "AGENT", containerLocator: containerLocator, usePkInId: usePkInId) { }
}
