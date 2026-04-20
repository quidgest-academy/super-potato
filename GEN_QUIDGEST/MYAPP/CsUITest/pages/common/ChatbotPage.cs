using OpenQA.Selenium.Support.UI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace quidgest.uitests.pages;

public class ChatbotPage: PageObject
{
    /// <summary>
    /// Chat bot container locator
    /// </summary>
	private By containerLocator => By.Id("chatbot-tab");

    /// <summary>
    /// Chat bot container element
    /// </summary>
	private IWebElement container => driver.FindElement(containerLocator);

    /// <summary>
    /// Chat bot toolbar container locator
    /// </summary>
	private By toolbarContainerLocator => By.CssSelector(".q-chatbot__tools");

    /// <summary>
    /// Chat bot toolbar container element
    /// </summary>
	private IWebElement toolbarContainer => container.FindElement(toolbarContainerLocator);

    private IWebElement ChatSelect => toolbarContainer.FindElement(By.CssSelector(".q-chatbot__tools__select .q-field__control"));

    /// <summary>
    /// Clear chat button
    /// </summary>
    private ButtonControl ClearButton => new ButtonControl(driver, toolbarContainerLocator, ".q-chatbot__tools-clear");

    /// <summary>
    /// Message area container element
    /// </summary>
    protected IWebElement messagesContainer => container.FindElement(By.CssSelector(".q-chatbot__messages-container"));

    /// <summary>
    /// Text area to type messages
    /// </summary>
	private IWebElement messageInput => wait.Until<IWebElement>(driver =>
    {
        IWebElement tempElement = container.FindElement(By.CssSelector("textarea"));
        return (tempElement.Displayed && tempElement.Enabled) ? tempElement : null;
    });

    /// <summary>
    /// Send button
    /// </summary>
    private ButtonControl SendButton => new ButtonControl(driver, containerLocator, ".q-chatbot__send");

    /// <summary>
    /// Upload button
    /// </summary>
    private ButtonControl UploadButton => new ButtonControl(driver, containerLocator, ".q-chatbot__upload");

    /// <summary>
    /// Whether the chat bot is in the process of generating a response
    /// </summary>
	private bool generatingResponse => GetElement(messagesContainer, By.CssSelector(".pulsing-dots")) != null;

    /// <summary>
    /// Constructor
    /// </summary>
    public ChatbotPage(IWebDriver driver) : base(driver) {}

    /// <summary>
    /// Send a message to the chat bot and get the response
    /// </summary>
    /// <param name="message">The message being sent to the chat bot</param>
    /// <returns>The response from the chat bot</returns>
    public string SendMessage(string message)
    {
        // Send message
        messageInput.SendKeys(message);
        SendButton.Click();

        // First wait for the notice about generating a response to appear
        WebDriverWait waitForStart = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        waitForStart.Until(c => generatingResponse);

        // Then wait for the notice about generating a response to disappear
        WebDriverWait waitForFinish = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        waitForFinish.Until(c => !generatingResponse);

        // Get all the message elements
        IList<IWebElement> messageElements = messagesContainer.FindElements(By.CssSelector(".q-chatbot__message"));

        // Return the most recent message which is the response from the chat bot
        return messageElements.Last().Text;
    }

    /// <summary>
    /// Clear the chat history
    /// </summary>
    public void ClearChat()
    {
        ClearButton.Click();
    }

    /// <summary>
    /// Changes the chat
    /// </summary>
    /// <param name="chatLabel"></param>
    public void ChangeChat(string chatLabel)
    {
        ChatSelect.Click();
        var overlay = driver.FindElement(By.CssSelector(".q-overlay"));
        var chat = overlay.FindElement(By.CssSelector($"[data-key='{chatLabel}']"));

        chat.Click();
    }
}
