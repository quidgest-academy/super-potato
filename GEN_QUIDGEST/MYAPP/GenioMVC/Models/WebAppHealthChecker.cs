using CSGenio.core.framework;

// USE /[MANUAL FOR MODEL HEALTH_CHECK_IMPORT]/

namespace GenioMVC.Models;

/// <summary>
/// Web application implementation of HealthChecker that provides platform-specific
/// health validations for MVC-based web applications
/// </summary>
public class WebAppHealthChecker : HealthChecker
{
	/// <summary>
	/// Initializes a new instance of WebAppHealthChecker with default validation interval
	/// </summary>
	/// <param name="environment">The environment name (e.g., "Development", "Production")</param>
	public WebAppHealthChecker(string environment) : base(environment) { }

	/// <summary>
	/// Initializes a new instance of WebAppHealthChecker with custom validation interval
	/// </summary>
	/// <param name="environment">The environment name (e.g., "Development", "Production")</param>
	/// <param name="intervalMillis">The caching interval in milliseconds to prevent excessive validation calls</param>
	public WebAppHealthChecker(string environment, int intervalMillis) : base(environment, intervalMillis) { }

	/// <summary>
	/// Validates core resources including database, report server and external APIs
	/// </summary>
	/// <returns>A dictionary containing the health status of all validated core resources</returns>
	protected override IDictionary<string, HealthStatus> ValidateCoreResources()
	{
		Dictionary<string, HealthStatus> details = [];

		if (ValidateConfiguration(details))
		{
			ValidateDBConnection(details);
			ValidateReportServer(details);
			ValidateChatbotAPI(details);
		}

		return details;
	}

	/// <inheritdoc/>
	protected override IDictionary<string, HealthStatus> ValidateCustomResources()
	{
		Dictionary<string, HealthStatus> details = [];

		// Add web application-specific validations here, such as:
		// - External service dependencies
		// - Authentication provider connectivity
		// - Session storage availability
		// - Application-specific configuration checks
		// - Custom middleware or component health

		// Then, add the validation results to the details dictionary using descriptive keys:
		// details.Add("session_storage", HealthStatus.Ok);
		// details.Add("external_api", HealthStatus.Error);

// USE /[MANUAL FOR MODEL HEALTH_CHECK_VALIDATION]/

// USE /[MANUAL FOR MODEL HEALTH_CHECK_VALIDATION_MYAPP]/

		return details;
	}
}
