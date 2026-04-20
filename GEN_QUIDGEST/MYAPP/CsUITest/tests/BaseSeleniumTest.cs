namespace quidgest.uitests.tests;

/// <summary>
/// Basic test setup that helps deal with selenium driver setup and cleanup
/// </summary>
public class BaseSeleniumTest
{
    protected IWebDriver Driver { get; private set; }

    [OneTimeSetUp]
    public void SetupOnce()
    {
        Driver = DriverFactory.getWebDriver();
    }
	
	[SetUp]
	public void Setup()
	{
		Driver.SwitchTo().NewWindow(WindowType.Tab);
	}

    [TearDown]
    public void Teardown()
    {
        //To speed up the tests instead of restarting the entire browser we just clean the cookies and start a new tab.
        //Using a new tab avoid situations where the previous test might have left a modal popup waiting for user input.
        //Tests that need full isolation can always be put in a separate test class alone.
        Driver.Manage().Cookies.DeleteAllCookies();
    }

    [OneTimeTearDown]
    public void TeardownFinal()
    {
        Driver.Quit();
    }
}