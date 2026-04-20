#nullable enable

namespace quidgest.uitests.pages.forms.core;

/// <summary>
/// Base class for form pages in the application.
/// Provides common functionality and properties for interacting with forms.
/// </summary>
public abstract class Form : PageObject
{
    private readonly By? _containerLocator;
    private readonly By? _bodyLocator;

    /// <summary>
    /// The ID of the form.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Whether to use the primary key of the form when finding controls.
    /// </summary>
    public bool UsePkInId { get; }

    /// <summary>
    /// The primary key of the form.
    /// </summary>
    public string PrimaryKey => Container.FindElement(BodyLocator).GetAttribute("data-identifier");

    /// <summary>
    /// The ID of the form.
    /// </summary>
    public string IdSuffix => UsePkInId ? "_" + PrimaryKey : string.Empty;

    /// <summary>
    /// Gets the mode of the form.
    /// </summary>
    public FORM_MODE Mode { get; }

    /// <summary>
    /// Gets the mode of the form from the page url
    /// </summary>
    public string UrlMode { get; }

    /// <summary>
    /// Gets the locator for the form container element.
    /// </summary>
    protected By ContainerLocator => _containerLocator ?? By.CssSelector("#form-container");

    /// <summary>
    /// Gets the form body locator.
    /// </summary>
    protected By BodyLocator => _bodyLocator ?? ByData.Key(Id);

    /// <summary>
    /// Gets the form container element.
    /// </summary>
    protected IWebElement Container => driver.FindElement(ContainerLocator);

    /// <summary>
    /// Gets the error message banner.
    /// </summary>
    public FormError Error => new(driver, ContainerLocator);

    // UI elements for the form actions
    private IWebElement SaveBtn => Container.FindElement(By.CssSelector("#bottom-save-btn"));
    private IWebElement ApplyBtn => Container.FindElement(By.CssSelector("#bottom-apply-btn"));
    private IWebElement CancelBtn => Container.FindElement(By.CssSelector("#bottom-cancel-btn"));
    private IWebElement BackBtn => Container.FindElement(By.CssSelector("#bottom-back-btn"));
    private IWebElement ConfirmBtn => Container.FindElement(By.CssSelector("#bottom-confirm-btn"));

    /// <summary>
    /// Initializes a new instance of the Form class.
    /// </summary>
    /// <param name="driver">The WebDriver instance used to interact with the browser.</param>
    /// <param name="mode">The mode of the form (e.g. Create, Edit, View).</param>
    /// <param name="containerLocator">A custom locator for the form container.</param>
    /// <param name="bodyLocator">A custom locator for the form body.</param>
    public Form(IWebDriver driver, FORM_MODE mode, string id, By? containerLocator = null, By? bodyLocator = null, bool usePkInId = false) : base(driver)
    {
        Id = id;
        Mode = mode;
        UsePkInId = usePkInId;
        
        _containerLocator = containerLocator;
        _bodyLocator = bodyLocator;

        wait.Until(c => Container);
        WaitForLoading();

        
        UrlMode = GetFormModeFromURL(driver.Url);
    }

    /// <summary>
    /// Waits for the form to finish loading.
    /// </summary>
    public void WaitForLoading()
    {
        wait.Until(c => Container.FindElement(BodyLocator).GetAttribute("data-loading") != "true");
    }

    /// <summary>
    /// Saves the form.
    /// </summary>
    public void Save()
    {
        WaitForLoading();
        SaveBtn.Click();
    }

    /// <summary>
    /// Applies the form changes.
    /// </summary>
    public void Apply()
    {
        WaitForLoading();
        ApplyBtn.Click();
    }

    /// <summary>
    /// Cancels the form.
    /// </summary>
    /// <param name="force">If true, forces the cancellation and loses all changes.</param>
    public void Cancel(bool force = false)
    {
        WaitForLoading();
        CancelBtn.Click();

        // Force the cancel and lose all changes
        if (force)
        {
            ConfirmationPopup confirmPopup = new(driver);
            confirmPopup.Confirm();
        }
    }

    /// <summary>
    /// Goes back to the previous page.
    /// </summary>
    public void Back()
    {
        WaitForLoading();
        BackBtn.Click();
    }

    /// <summary>
    /// Clicks on the confirm button.
    /// </summary>
    public void Confirm()
    {
        WaitForLoading();
        ConfirmBtn.Click();
    }

    /// <summary>
    /// Compares the form mode that is set when initializing the form with the form mode set in the URL
    /// </summary>
    /// <returns>Wether or not the form modes are equal</returns>
    public bool ValidateFormMode()
    {
        if (Enum.TryParse(UrlMode, true, out FORM_MODE parsedMode))
        {
            return parsedMode == Mode;
        }
        return false;
    }

    /// <summary>
    /// Returns the form mode from the page url
    /// </summary>
    /// <param name="url">The URL of the page</param>
    /// <returns>The mode of the current form</returns>
    private string GetFormModeFromURL(string url)
    {
        string[] parts = url.Split('/');
        int formIndex = Array.IndexOf(parts, "form");

        if (formIndex != -1 && parts.Length > formIndex + 2)
        {
            return parts[formIndex + 2];
        }

        return ""; 
    }
}
