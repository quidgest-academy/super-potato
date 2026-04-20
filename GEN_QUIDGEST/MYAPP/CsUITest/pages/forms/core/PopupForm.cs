namespace quidgest.uitests.pages.forms.core;

public class PopupForm : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopupForm"/> class.
    /// </summary>
    /// <param name="driver">The WebDriver instance used to interact with the browser.</param>
    /// <param name="mode">The mode of the form.</param>
    /// <param name="id">The ID of the form.</param>
    /// <param name="usePkInId">Whether to use the primary key in IDs of the form controls.</param>
    public PopupForm(IWebDriver driver, FORM_MODE mode, string id, bool usePkInId = false)
        : base(driver, mode, id, containerLocator: By.CssSelector($"#q-modal-form-{id.ToUpper()}"), usePkInId: usePkInId) { }
}
