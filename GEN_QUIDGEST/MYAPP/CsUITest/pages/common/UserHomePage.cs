namespace quidgest.uitests.pages;

public class UserHomePage: PageObject {
	
	IWebElement userAvatar => driver.FindElement(By.CssSelector("#user-avatar"));

	public InputControl Name => new InputControl(driver, By.Id("container-ValNome"), By.Id("ValNome"));

    public InputControl OldPassword => new InputControl(driver, By.Id("container-OldPassword"), By.Id("OldPassword"));

    public InputControl NewPassword => new InputControl(driver, By.Id("container-NewPassword"), By.Id("NewPassword"));

    public InputControl ConfirmPassword => new InputControl(driver, By.Id("container-ConfirmPassword"), By.Id("ConfirmPassword"));

    public ButtonControl ChangePasswordButton => new ButtonControl(driver, By.Id("container-ChangePassword"), "#ChangePassword");

    public UserHomePage(IWebDriver driver) : base(driver) {
		wait.Until(c => userAvatar != null);
        wait.Until(c => Name != null);
        wait.Until(c => OldPassword != null);
        wait.Until(c => NewPassword != null);
        wait.Until(c => ConfirmPassword != null);
        wait.Until(c => ChangePasswordButton != null);
    }

    /// <summary>
    /// Change the user's password.
    /// </summary>
    /// <param name="oldPassword">The user's current password.</param>
    /// <param name="newPassword">The user's new password.</param>
    public void ChangePassword(string oldPassword, string newPassword)
    {
        OldPassword.SetValue(oldPassword);
        NewPassword.SetValue(newPassword);
        ConfirmPassword.SetValue(newPassword);
        ChangePasswordButton.Click();
    }
}
