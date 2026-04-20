namespace quidgest.uitests.core
{
    public class InputControl : ControlObject
    {
        public InputControl(IWebDriver driver, By containerLocator, By controlLocator) : base(driver, containerLocator, controlLocator)
        {
        }

        /// <summary>
        /// Click the input
        /// </summary>
        public virtual void Click()
        {
            m_control.Click();
        }

        /// <summary>
        /// Set the control's value
        /// </summary>
        /// <param name="value">Value</param>
        public virtual void SetValue(string value)
        {
            m_control.SendKeys(value);
        }
    }
}
