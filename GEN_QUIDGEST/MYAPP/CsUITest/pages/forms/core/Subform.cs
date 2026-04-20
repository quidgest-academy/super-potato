#nullable enable

namespace quidgest.uitests.pages.forms.core;

public class Subform : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Subform"/> class.
    /// </summary>
    /// <param name="driver">The WebDriver instance used to interact with the browser.</param>
    /// <param name="mode">The mode of the form.</param>
    /// <param name="id">The ID of the form.</param>
    /// <param name="parentFormId">The ID of the parent form.</param>
    /// <param name="containerLocator">A custom locator for the form container.</param>
    /// <param name="usePkInId">Whether to use the primary key in IDs of the form controls.</param>
    public Subform(IWebDriver driver, FORM_MODE mode, string id, string parentFormId, By? containerLocator = null, bool usePkInId = false)
        : base(driver, mode, id, containerLocator: containerLocator, bodyLocator: ByData.Key(parentFormId), usePkInId: usePkInId) { }
}
