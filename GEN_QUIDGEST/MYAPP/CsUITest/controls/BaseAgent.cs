using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Linq;

namespace quidgest.uitests.controls
{
    public class BaseAgent: ChatbotPage
    {
        public BaseAgent(IWebDriver driver) : base(driver) 
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Gets a suggestion text based on the test id
        /// </summary>
        public string GetSuggestionText(string suggestionTestId)
        {
            var suggestion = GetSuggestion(suggestionTestId);
            var textElement = wait.Until(d =>suggestion.FindElement(By.CssSelector(".q-field-preview__content")));

            return textElement.Text;
        }

        /// <summary>
        /// Gets a suggestion based on the test id
        /// </summary>
        /// <param name="suggestionTestId"></param>
        /// <returns></returns>
        public IWebElement GetSuggestion(string suggestionTestId)
        {
            return messagesContainer.FindElement(By.CssSelector(suggestionTestId));
        }

        /// <summary>
        /// Gets the suggestions container from the lastest message
        /// </summary>
        /// <returns></returns>
        public IWebElement GetSuggestionsContainer()
        {
            var messages = messagesContainer?.FindElements(By.CssSelector(".q-chatbot__messages-wrapper"));

            if(messages == null || !messages.Any())
                throw new InvalidOperationException("No messages found in the chatbot container.");

            return messages.Last();
        }


        /// <summary>
        /// Gets all the suggestions from the lastest message
        /// </summary>
        public IList<IWebElement> GetAllSuggestions()
        {
            var suggestionsContainer = GetSuggestionsContainer();

            return suggestionsContainer.FindElements(By.CssSelector(".q-field-preview")).ToList();
        }

        /// <summary>
        /// Applies the lastest suggestions
        /// </summary>
        public void ApplyLatestSuggestions()
        {
            var suggestionsContainer = GetSuggestionsContainer();
            try
            {
                var applyAllButton = wait.Until(d =>
                    suggestionsContainer.FindElement(By.CssSelector(".q-chatbot__apply-all-button"))
                );
                applyAllButton.Click();
            }
            catch (NoSuchElementException)
            {
                throw new InvalidOperationException("Apply All button was not found in the latest suggestions container.");
            }
        }
    }
}
