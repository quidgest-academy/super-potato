using quidgest.uitests.pages.forms.core;

#nullable enable

namespace quidgest.uitests.pages.forms;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class ContactForm : PopupForm
{
	/// <summary>
	/// Date
	/// </summary>
	public DateInputControl ContaDate => new DateInputControl(driver, ContainerLocator, "#CONTACT_CONTADATE____" + IdSuffix);

	/// <summary>
	/// Title
	/// </summary>
	public LookupControl PropeTitle => new LookupControl(driver, ContainerLocator, "container-CONTACT_PROPETITLE___" + IdSuffix);
	public SeeMorePage PropeTitleSeeMorePage => new SeeMorePage(driver, "CONTACT", "CONTACT_PROPETITLE___" + IdSuffix);

	/// <summary>
	/// Client name
	/// </summary>
	public BaseInputControl ContaClient => new BaseInputControl(driver, ContainerLocator, "container-CONTACT_CONTACLIENT__" + IdSuffix, "#CONTACT_CONTACLIENT__" + IdSuffix);

	/// <summary>
	/// Email do cliente
	/// </summary>
	public BaseInputControl ContaEmail => new BaseInputControl(driver, ContainerLocator, "container-CONTACT_CONTAEMAIL___" + IdSuffix, "#CONTACT_CONTAEMAIL___" + IdSuffix);

	/// <summary>
	/// Phone number
	/// </summary>
	public BaseInputControl ContaPhone => new BaseInputControl(driver, ContainerLocator, "container-CONTACT_CONTAPHONE___" + IdSuffix, "#CONTACT_CONTAPHONE___" + IdSuffix);

	/// <summary>
	/// Description
	/// </summary>
	public BaseInputControl ContaDescript => new BaseInputControl(driver, ContainerLocator, "container-CONTACT_CONTADESCRIPT" + IdSuffix, "#CONTACT_CONTADESCRIPT" + IdSuffix);

	public ContactForm(IWebDriver driver, FORM_MODE mode, By? containerLocator = null, bool usePkInId = false)
		: base(driver, mode, "CONTACT", usePkInId: usePkInId) { }
}
