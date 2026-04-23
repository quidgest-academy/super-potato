using quidgest.uitests.pages.forms.core;

#nullable enable

namespace quidgest.uitests.pages.forms;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class PropertyForm : Form
{
	/// <summary>
	/// Order
	/// </summary>
	public BaseInputControl PropeId => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEID______" + IdSuffix, "#PROPERTYPROPEID______" + IdSuffix);

	/// <summary>
	/// Sold
	/// </summary>
	public CheckboxInputControl PropeSold => new CheckboxInputControl(driver, ContainerLocator, "#container-PROPERTYPROPESOLD____" + IdSuffix);

	/// <summary>
	/// Sold date
	/// </summary>
	public DateInputControl PropeDtsold => new DateInputControl(driver, ContainerLocator, "#PROPERTYPROPEDTSOLD__" + IdSuffix);

	/// <summary>
	/// Last Visit
	/// </summary>
	public DateInputControl PropeLastvisit => new DateInputControl(driver, ContainerLocator, "#PROPERTY__PROPE__LASTVISIT" + IdSuffix);

	/// <summary>
	/// New Group
	/// </summary>
	public IWebElement PseudNewgrp05 => throw new NotImplementedException();

	/// <summary>
	/// Main info
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp01 => new CollapsibleZoneControl(driver, ContainerLocator, "#PROPERTYPSEUDNEWGRP01" + IdSuffix + "-container");

	/// <summary>
	/// Album
	/// </summary>
	public ListControl PseudField001 => new ListControl(driver, ContainerLocator, "#PROPERTYPSEUDFIELD001" + IdSuffix);

	/// <summary>
	/// Contacts
	/// </summary>
	public ListControl PseudField002 => new ListControl(driver, ContainerLocator, "#PROPERTYPSEUDFIELD002" + IdSuffix);

	/// <summary>
	/// AveragePrice
	/// </summary>
	public BaseInputControl PropeAverage => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEAVERAGE_" + IdSuffix, "#PROPERTYPROPEAVERAGE_" + IdSuffix);

	/// <summary>
	/// Localization
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp02 => new CollapsibleZoneControl(driver, ContainerLocator, "#PROPERTYPSEUDNEWGRP02" + IdSuffix + "-container");

	/// <summary>
	/// City
	/// </summary>
	public LookupControl CityCity => new LookupControl(driver, ContainerLocator, "container-PROPERTYCITY_CITY____" + IdSuffix);
	public SeeMorePage CityCitySeeMorePage => new SeeMorePage(driver, "PROPERTY", "PROPERTYCITY_CITY____" + IdSuffix);

	/// <summary>
	/// Country
	/// </summary>
	public IWebElement CountCountry => throw new NotImplementedException();

	/// <summary>
	/// Price
	/// </summary>
	public BaseInputControl PropePrice => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEPRICE___" + IdSuffix, "#PROPERTYPROPEPRICE___" + IdSuffix);

	/// <summary>
	/// Details
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp03 => new CollapsibleZoneControl(driver, ContainerLocator, "#PROPERTYPSEUDNEWGRP03" + IdSuffix + "-container");

	/// <summary>
	/// Building typology
	/// </summary>
	public RadiobuttonControl PropeTypology => new RadiobuttonControl(driver, ContainerLocator, "container-PROPERTYPROPETYPOLOGY" + IdSuffix);

	/// <summary>
	/// Building type
	/// </summary>
	public EnumControl PropeBuildtyp => new EnumControl(driver, ContainerLocator, "container-PROPERTYPROPEBUILDTYP" + IdSuffix);

	/// <summary>
	/// Ground size
	/// </summary>
	public BaseInputControl PropeGrdsize => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEGRDSIZE_" + IdSuffix, "#PROPERTYPROPEGRDSIZE_" + IdSuffix);

	/// <summary>
	/// Floor
	/// </summary>
	public BaseInputControl PropeFloornr => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEFLOORNR_" + IdSuffix, "#PROPERTYPROPEFLOORNR_" + IdSuffix);

	/// <summary>
	/// Size (m2)
	/// </summary>
	public BaseInputControl PropeSize => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPESIZE____" + IdSuffix, "#PROPERTYPROPESIZE____" + IdSuffix);

	/// <summary>
	/// Bathrooms number
	/// </summary>
	public BaseInputControl PropeBathnr => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEBATHNR__" + IdSuffix, "#PROPERTYPROPEBATHNR__" + IdSuffix);

	/// <summary>
	/// Construction date
	/// </summary>
	public DateInputControl PropeDtconst => new DateInputControl(driver, ContainerLocator, "#PROPERTYPROPEDTCONST_" + IdSuffix);

	/// <summary>
	/// Building age
	/// </summary>
	public BaseInputControl PropeBuildage => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEBUILDAGE" + IdSuffix, "#PROPERTYPROPEBUILDAGE" + IdSuffix);

	/// <summary>
	/// Agent information
	/// </summary>
	public CollapsibleZoneControl PseudNewgrp04 => new CollapsibleZoneControl(driver, ContainerLocator, "#PROPERTYPSEUDNEWGRP04" + IdSuffix + "-container");

	/// <summary>
	/// Agent's name
	/// </summary>
	public LookupControl AgentName => new LookupControl(driver, ContainerLocator, "container-PROPERTYAGENTNAME____" + IdSuffix);
	public SeeMorePage AgentNameSeeMorePage => new SeeMorePage(driver, "PROPERTY", "PROPERTYAGENTNAME____" + IdSuffix);

	/// <summary>
	/// Photography
	/// </summary>
	public IWebElement AgentPhotogra => throw new NotImplementedException();

	/// <summary>
	/// E-mail
	/// </summary>
	public IWebElement AgentEmail => throw new NotImplementedException();

	/// <summary>
	/// Profit
	/// </summary>
	public BaseInputControl PropeProfit => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEPROFIT__" + IdSuffix, "#PROPERTYPROPEPROFIT__" + IdSuffix);

	/// <summary>
	/// Tax
	/// </summary>
	public BaseInputControl PropeTax => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPETAX_____" + IdSuffix, "#PROPERTYPROPETAX_____" + IdSuffix);

	/// <summary>
	/// Title
	/// </summary>
	public BaseInputControl PropeTitle => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPETITLE___" + IdSuffix, "#PROPERTYPROPETITLE___" + IdSuffix);

	/// <summary>
	/// Main photo
	/// </summary>
	public BaseInputControl PropePhoto => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEPHOTO___" + IdSuffix, "#PROPERTYPROPEPHOTO___" + IdSuffix);

	/// <summary>
	/// Description
	/// </summary>
	public BaseInputControl PropeDescript => new BaseInputControl(driver, ContainerLocator, "container-PROPERTYPROPEDESCRIPT" + IdSuffix, "#PROPERTYPROPEDESCRIPT" + IdSuffix);

	public PropertyForm(IWebDriver driver, FORM_MODE mode, By? containerLocator = null, bool usePkInId = false)
		: base(driver, mode, "PROPERTY", containerLocator: containerLocator, usePkInId: usePkInId) { }
}
