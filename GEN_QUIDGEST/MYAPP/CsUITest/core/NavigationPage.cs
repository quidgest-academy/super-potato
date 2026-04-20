namespace quidgest.uitests.core;

public class NavigationPage: PageObject {

	//private final static Logger LOGGER = LoggerFactory.getLogger(NavigationPage.class.getName());

	protected string year;
	protected string baseUrl;
	protected string language;


	public NavigationPage(IWebDriver driver, string year, string baseUrl, string language): base(driver) {
		this.year = year;
		this.baseUrl = baseUrl;
		this.language = language;
	}

	public void navigateToModule(string module) {
		if (string.IsNullOrEmpty(module)) throw new ArgumentException($"{nameof(module)} must contain value.");
		//try {
			Uri url = new Uri(baseUrl + year + "/" + module);
			driver.Navigate().GoToUrl(url);
			//LOGGER.debug("Navigating to: {}", url);

			// HACK: wait a bit for the result page to load
		// 	Thread.sleep(1000);
		// } catch (MalformedURLException e) {
		// 	LOGGER.error("Bad module navigation URL.", e);
		// } catch (InterruptedException e) {
		// 	e.printStackTrace();
		// }
	}

	public void navigateToMenu(string menu) {
		if (string.IsNullOrEmpty(menu)) throw new ArgumentException($"{nameof(menu)} must contain value.");

		//try {
			Uri url = new Uri(baseUrl + language + "/" + year + "/" + menu);
			driver.Navigate().GoToUrl(url);
			//LOGGER.debug("Navigating to: {}", url);

			// HACK: wait a bit for the result page to load
/*
			Thread.sleep(1000);
		} catch (MalformedURLException e) {
			LOGGER.error("Bad menu navigation URL.", e);
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
		*/
	}
}
