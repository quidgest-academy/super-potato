namespace quidgest.uitests.pages;

public class ConfirmationPopup: PageObject
{
    IWebElement dialogContainer => driver.FindElement(By.CssSelector(".q-overlay"));
    IWebElement dialog => driver.FindElement(By.CssSelector(".q-dialog"));
    IWebElement buttonOk => dialog.FindElements(By.CssSelector("button"))?[0];
    IWebElement buttonCancel => dialog.FindElements(By.CssSelector("button"))?[1];
    IWebElement buttonDeny => dialog.FindElements(By.CssSelector("button"))?[2];
    IWebElement dialogText => dialog.FindElement(By.CssSelector(".q-dialog__body-text"));
    bool dialogAnimationEnded => !dialogContainer.GetAttribute("class").Contains("fade-enter-active");

    public ConfirmationPopup(IWebDriver driver): base(driver)
    {
		wait.Until(c => dialog);
        wait.Until(c => dialog.Displayed);
        wait.Until(c => dialogAnimationEnded);
	}

    public void Confirm()
    {
        buttonOk.AnimatedClick();
    }

    public void Cancel()
    {
        buttonCancel.AnimatedClick();
    }

    public void Deny()
    {
        buttonDeny.AnimatedClick();
    }

    public string GetDialogText()
    {
        return dialogText.Text;
    }
}
