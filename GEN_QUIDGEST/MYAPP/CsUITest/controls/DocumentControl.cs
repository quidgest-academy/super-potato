namespace quidgest.uitests.controls;

public class DocumentControl(IWebDriver driver, By containerLocator, string controlId) : ControlObject(driver, containerLocator, By.Id(controlId))
{
    protected IWebElement FileInput => m_control.FindElement(By.CssSelector("[data-testid='file-input']"));
    protected IWebElement OptionsBtn => m_control.FindElement(By.CssSelector("[data-type='options-button']"));
    protected IWebElement DeleteBtn => driver.FindElement(By.CssSelector("[data-key='delete']"));

    /// <summary>
    /// True if the control is blocked, false otherwise
    /// </summary>
    public bool IsBlocked => m_control.GetAttribute("class").Contains("q-document--readonly");

    /// <summary>
    /// Get the file name
    /// </summary>
    public string GetFileName()
    {
        return m_control.FindElement(By.CssSelector("[data-testid='document-input']")).GetAttribute("value");
    }

    /// <summary>
    /// Set a new file
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    public void UploadFile(string filePath)
    {
        FileInput.SendKeys(filePath);
    }

    /// <summary>
    /// Deletes the current file
    /// </summary>
    public void DeleteFile()
    {
        OptionsBtn.Click();
        wait.Until(c => DeleteBtn);
        DeleteBtn.Click();

        ConfirmationPopup confirmPopup = new(driver);
        confirmPopup.Confirm();
    }
}
