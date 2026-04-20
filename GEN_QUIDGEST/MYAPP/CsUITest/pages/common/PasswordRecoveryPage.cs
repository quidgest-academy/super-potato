namespace quidgest.uitests.pages;

public class PasswordRecoveryPage : PageObject
{
    private IWebElement recoveryForm => driver.FindElement(By.Id("recover-password-container"));
    private IWebElement emailInput => recoveryForm.FindElement(By.Id("Email"));
    private IWebElement recoveryBtn => recoveryForm.FindElement(By.Id("reset-button"));

    public PasswordRecoveryPage(IWebDriver driver) : base(driver)
    {
        wait.Until(c => recoveryForm != null);
    }

    private void WaitForLoad()
    {
        wait.Until(c => recoveryBtn.GetAttribute("data-loading") == "false");
    }

    public void RecoverPassword(string email)
    {
        emailInput.SendKeys(email);
        recoveryBtn.Click();
    }

    public bool HasErrorMessage(string id)
    {
        WaitForLoad();
        IWebElement errorMessage = recoveryForm.FindElement(By.Id(id));

        return errorMessage.Text.Length > 0;
    }
}
