using quidgest.uitests.pages.forms.core;

#nullable enable

namespace quidgest.uitests.pages.forms;

[System.CodeDom.Compiler.GeneratedCode("Genio", "")]
public class AlbumForm : PopupForm
{
	/// <summary>
	/// Photo
	/// </summary>
	public BaseInputControl PhotoPhoto => new BaseInputControl(driver, ContainerLocator, "container-ALBUM___PHOTOPHOTO___" + IdSuffix, "#ALBUM___PHOTOPHOTO___" + IdSuffix);

	/// <summary>
	/// Title
	/// </summary>
	public BaseInputControl PhotoTitle => new BaseInputControl(driver, ContainerLocator, "container-ALBUM___PHOTOTITLE___" + IdSuffix, "#ALBUM___PHOTOTITLE___" + IdSuffix);

	/// <summary>
	/// Property
	/// </summary>
	public LookupControl PropeTitle => new LookupControl(driver, ContainerLocator, "container-ALBUM___PROPETITLE___" + IdSuffix);
	public SeeMorePage PropeTitleSeeMorePage => new SeeMorePage(driver, "ALBUM", "ALBUM___PROPETITLE___" + IdSuffix);

	public AlbumForm(IWebDriver driver, FORM_MODE mode, By? containerLocator = null, bool usePkInId = false)
		: base(driver, mode, "ALBUM", usePkInId: usePkInId) { }
}
