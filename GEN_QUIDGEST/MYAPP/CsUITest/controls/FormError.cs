namespace quidgest.uitests.controls
{
    /// <summary>
    /// Error message banner that appear when there is an error in the form
    /// </summary>
    public class FormError : ControlObject
    {

        public FormError(IWebDriver driver, By containerLocator) : base(driver, containerLocator, By.ClassName("q-validation-error"))
        {
            wait.Message = "Could not find any error message";
            wait.Until(c => m_control.Displayed);
        }

        public string Message => m_control.Text;

    }
}
