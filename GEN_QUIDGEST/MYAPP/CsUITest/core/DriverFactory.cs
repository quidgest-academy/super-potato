using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
//using OpenQA.Selenium.Opera;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace quidgest.uitests.core;

public class DriverFactory {

	public static IWebDriver getWebDriver() {
		var c = Configuration.Instance;
		return getWebDriver(c.Browser, c.Headless.Value, c.ImplicitWait.Value, c.WindowWidth.Value, c.WindowHeight.Value);
	}

	public static IWebDriver getWebDriver(string browser, bool headless, int ImplicitWaitMilliseconds, int windowwidth, int windowheight) {
		IWebDriver driver;

		switch(browser)
		{
			case "firefox":
				new DriverManager().SetUpDriver(new FirefoxConfig(), VersionResolveStrategy.MatchingBrowser);
				FirefoxOptions firefoxOptions = new FirefoxOptions();
				if(headless)
					firefoxOptions.AddArguments("--headless");
				driver = new FirefoxDriver(firefoxOptions);
				break;
			case "edge":
				new DriverManager().SetUpDriver(new EdgeConfig(), VersionResolveStrategy.MatchingBrowser);
				driver = new EdgeDriver();
				break;
			//case "opera":
			//	new DriverManager().SetUpDriver(new OperaConfig());
			//	driver = new OperaDriver();
			//	break;
			default: //chrome
				new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
				ChromeOptions chromeOptions = new ChromeOptions();
				if(headless)
					chromeOptions.AddArgument("--headless");

				chromeOptions.AddArgument("--window-size=" + windowwidth + "," + windowheight);

				chromeOptions.AddArgument("--allow-insecure-localhost");
				
				// Disable Chrome's built-in password manager and credential service
				// to prevent prompts during automated tests.
				chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
				chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
				chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);

				// Setting the logging level allows to obtain much more information than by default level. 
				// This becomes very useful in debugging the applications and e2e tests.
				// TODO: It should be possible to configure it via JSON.
				// chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.All);

				driver = new ChromeDriver(chromeOptions);
				break;
		}

		driver.Manage().Timeouts().ImplicitWait = System.TimeSpan.FromMilliseconds(ImplicitWaitMilliseconds);
		return driver;
	}
}
