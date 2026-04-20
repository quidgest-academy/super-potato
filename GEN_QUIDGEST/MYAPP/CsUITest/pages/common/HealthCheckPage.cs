using OpenQA.Selenium.Support.UI;
using System.Text.Json;

namespace quidgest.uitests.pages;

// Page Object Model for Health Check endpoint
public class HealthCheckPage(IWebDriver driver, string baseUrl)
{
	protected readonly IWebDriver driver = driver;
	protected readonly string baseUrl = baseUrl;

	/// <summary>
	/// The full Uri for the health check endpoint
	/// </summary>
	public Uri HealthCheckUrl
	{
		get
		{
			string baseUrl = this.baseUrl;
			if (!baseUrl.EndsWith("/"))
				baseUrl += "/";

			Uri baseUri = new(baseUrl);
			return new Uri(baseUri, "api/health");
		}
	}

	/// <summary>
	/// Wait for page to load and contain JSON
	/// </summary>
	/// <param name="timeoutSeconds">Timeout in seconds to wait for JSON content</param>
	private void WaitForJsonContent(int timeoutSeconds = 10)
	{
		WebDriverWait wait = new(driver, TimeSpan.FromSeconds(timeoutSeconds));
		wait.Until(driver => driver.PageSource.Contains("{"));
	}

	/// <summary>
	/// Get property value from JSON
	/// </summary>
	/// <param name="jsonContent">JSON string to parse</param>
	/// <param name="propertyName">Property name to retrieve</param>
	/// <returns>Property value as string, or null if not found</returns>
	private string GetJsonPropertyValue(string jsonContent, string propertyName)
	{
		try
		{
			using var doc = JsonDocument.Parse(jsonContent);
			if (doc.RootElement.TryGetProperty(propertyName, out var property))
				return property.ToString();
			return null;
		}
		catch (JsonException)
		{
			return null;
		}
	}

	/// <summary>
	/// Check if the health check status is "ok"
	/// </summary>
	/// <param name="jsonContent">JSON string to check</param>
	/// <returns>True if status is "ok", false otherwise</returns>
	private bool IsHealthStatusOk(string jsonContent)
	{
		string statusValue = GetJsonPropertyValue(jsonContent, "status");
		return string.Equals(statusValue, "ok", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Navigate to health check endpoint using Selenium
	/// </summary>
	public void NavigateToHealthCheck()
	{
		driver.Navigate().GoToUrl(HealthCheckUrl);
	}

	/// <summary>
	/// Get JSON content displayed in browser
	/// </summary>
	/// <returns>JSON content as displayed in the browser</returns>
	public string GetJsonContent()
	{
		WaitForJsonContent();

		try
		{
			// Try to find JSON in a <pre> tag first
			var preElement = driver.FindElement(By.TagName("pre"));
			return preElement.Text;
		}
		catch (NoSuchElementException)
		{
			// Fallback to body content if no <pre> tag
			var bodyElement = driver.FindElement(By.TagName("body"));
			return bodyElement.Text;
		}
	}

	/// <summary>
	/// Check if the health check status is "ok" using browser content
	/// </summary>
	/// <returns>True if status is "ok", false otherwise</returns>
	public bool IsHealthStatusOk()
	{
		try
		{
			string jsonContent = GetJsonContent();
			return IsHealthStatusOk(jsonContent);
		}
		catch (Exception)
		{
			return false;
		}
	}
}
