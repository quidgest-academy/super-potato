using CSGenio.core.framework;

namespace Administration.Models;

/// <summary>
/// Web application implementation of HealthChecker that provides platform-specific
/// health validations for the WebAdmin application
/// </summary>
public class WebAdminHealthChecker : HealthChecker
{
	/// <summary>
	/// Initializes a new instance of WebAdminHealthChecker with default validation interval
	/// </summary>
	/// <param name="environment">The environment name (e.g., "Development", "Production")</param>
	public WebAdminHealthChecker(string environment) : base(environment) { }

	/// <summary>
	/// Initializes a new instance of WebAdminHealthChecker with custom validation interval
	/// </summary>
	/// <param name="environment">The environment name (e.g., "Development", "Production")</param>
	/// <param name="intervalMillis">The caching interval in milliseconds to prevent excessive validation calls</param>
	public WebAdminHealthChecker(string environment, int intervalMillis) : base(environment, intervalMillis) { }

	/// <summary>
	/// Validates database configuration and connectivity for all data systems
	/// </summary>
	/// <returns>A dictionary containing the health status of the data systems</returns>
	protected override IDictionary<string, HealthStatus> ValidateCoreResources()
	{
		Dictionary<string, HealthStatus> details = [];

		if (ValidateConfiguration(details))
			ValidateDBConnection(details);

		return details;
	}
}
