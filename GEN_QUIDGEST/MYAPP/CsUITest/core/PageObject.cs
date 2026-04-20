using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Linq;

namespace quidgest.uitests.core;

/// <summary>
/// Base class for every Page Object Model (POM).
/// </summary>
/// <remarks>
/// https://www.selenium.dev/documentation/en/guidelines_and_recommendations/page_object_models/
/// </remarks>
public class PageObject {
	//private final static Logger LOGGER = LoggerFactory.getLogger(PageObject.class.getName());

	protected IWebDriver driver;
	protected WebDriverWait wait;

	/// <summary>
	/// Initialize a Page Object Model (POM)
	/// </summary>
	/// <param name="driver">WebDriver</param>
	public PageObject(IWebDriver driver) {
		this.driver = driver;
		this.wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(Configuration.Instance.ExplicitWait.Value));
		this.wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));
	}

    /// <summary>
    /// Get a DOM element within an element if it exists. Otherwise return null.
    /// </summary>
	/// <param name="element">The DOM element to search within.</param>
    /// <param name="by">The locating mechanism to use.</param>
    public IWebElement GetElement(ISearchContext element, By by)
    {
		if (element == null)
			return null;

		// Get matching elements as list (although there should only be one)
		// This way, if the element doesn't exist, the list is empty
		// but there is no exception
		ReadOnlyCollection<IWebElement> elementList = element.FindElements(by);

		// Element not found
		if (!elementList.Any())
			return null;

		// Element found
		return elementList[0];
    }

    /// <summary>
    /// Get a DOM element if it exists. Otherwise return null.
    /// </summary>
    /// <param name="by">The locating mechanism to use.</param>
    public IWebElement GetElement(By by)
    {
        IWebElement document = driver.FindElement(By.TagName("HTML"));

		return GetElement(document, by);
    }


}
