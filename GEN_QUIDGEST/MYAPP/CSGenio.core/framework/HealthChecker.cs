using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;

using CSGenio.core.ai;
using CSGenio.core.di;
using CSGenio.framework;
using CSGenio.persistence;

namespace CSGenio.core.framework;

/// <summary>
/// Represents the possible health status values for system resources
/// </summary>
public enum HealthStatus
{
	/// <summary>
	/// Indicates the resource is functioning normally
	/// </summary>
	[Description("ok")]
	Ok,

	/// <summary>
	/// Indicates the resource has encountered an error or is unavailable
	/// </summary>
	[Description("error")]
	Error
}

/// <summary>
/// Extension methods for HealthStatus enum
/// </summary>
static class HealthStatusExtensions
{
	/// <summary>
	/// Gets the description attribute value for a HealthStatus enum value
	/// </summary>
	/// <param name="status">The HealthStatus enum value</param>
	/// <returns>The description string from the Description attribute</returns>
	public static string GetDescription(this HealthStatus status)
	{
		var field = status.GetType().GetField(status.ToString());
		if (field == null)
			return status.ToString();

		var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
		return attribute?.Description ?? status.ToString();
	}
}

/// <summary>
/// Represents the result of a health check operation, containing status information
/// and details about individual system resources
/// </summary>
public class HealthCheckResult
{
	/// <summary>
	/// Initializes a new instance of HealthCheckResult
	/// </summary>
	/// <param name="details">Dictionary containing the health status of individual resources</param>
	/// <param name="environment">The environment name where the health check was performed</param>
	public HealthCheckResult(IDictionary<string, HealthStatus> details = null, string environment = null)
	{
		Timestamp = DateTime.UtcNow;
		Service = Configuration.Application?.Name ?? "";
		Environment = environment ?? "";
		Details = details ?? new Dictionary<string, HealthStatus>();

		// Overall status is Error if any resource has an error, otherwise Ok
		Status = Details.Count > 0 && Details.Values.Contains(HealthStatus.Error)
			? HealthStatus.Error
			: HealthStatus.Ok;
	}

	/// <summary>
	/// Indicates whether the overall health status is OK
	/// </summary>
	public bool IsOk => Status == HealthStatus.Ok;

	/// <summary>
	/// The overall health status of the system
	/// </summary>
	public HealthStatus Status { get; }

	/// <summary>
	/// The name of the service being monitored
	/// </summary>
	public string Service { get; }

	/// <summary>
	/// The environment name where the health check was performed
	/// </summary>
	public string Environment { get; }

	/// <summary>
	/// The timestamp when the health check was performed (UTC)
	/// </summary>
	public DateTime Timestamp { get; }

	/// <summary>
	/// A dictionary containing the health status of individual system resources
	/// </summary>
	public IDictionary<string, HealthStatus> Details { get; }

	/// <summary>
	/// Converts the health check result to a JSON-serializable object
	/// </summary>
	/// <returns>An anonymous object that can be serialized to JSON</returns>
	public object ToJson()
	{
		return new
		{
			status = Status.GetDescription(),
			timestamp = Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff UTC", CultureInfo.InvariantCulture),
			service = Service,
			environment = Environment.ToLower(),
			details = Details.ToDictionary(d => d.Key, d => d.Value.GetDescription())
		};
	}
}

/// <summary>
/// Class that checks the availability of critical resources such as databases.
/// Provides caching functionality to avoid excessive validation calls.
/// </summary>
public abstract class HealthChecker
{
	/// <summary>
	/// The environment name for this health checker instance
	/// </summary>
	protected readonly string environment;

	/// <summary>
	/// Initializes a new instance of HealthChecker
	/// </summary>
	/// <param name="environment">The environment name (e.g., "production", "staging")</param>
	/// <param name="intervalMillis">The caching interval in milliseconds to prevent excessive validation calls</param>
	protected HealthChecker(string environment, int intervalMillis = 5000)
	{
		this.environment = environment;
		ValidationIntervalMillis = intervalMillis;
	}

	/// <summary>
	/// The result of the last health check operation
	/// </summary>
	public HealthCheckResult LastResult { get; private set; }

	/// <summary>
	/// The time in milliseconds before a validation check will be performed again.
	/// The result of the last check will be used until this time elapses.
	/// A value of 0 or less means checks are performed immediately without delay.
	/// </summary>
	public int ValidationIntervalMillis { get; }

	/// <summary>
	/// Tests if an HTTP endpoint is accessible by sending an HTTP request
	/// </summary>
	/// <param name="url">The URL of the endpoint to test</param>
	/// <param name="timeoutSeconds">Timeout in seconds for the HTTP request</param>
	/// <returns>True if the endpoint responds successfully, false otherwise</returns>
	private static bool TestHttpEndpoint(string url, int timeoutSeconds = 10)
	{
		try
		{
			using var httpClient = new HttpClient();
			httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

			HttpResponseMessage response = httpClient.GetAsync(url).Result;
			// Consider 401 as "healthy" - server is responding, just needs authentication
			return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Unauthorized;
		}
		catch (Exception e)
		{
			GenioDI.Log.Error($"Health Check (HTTP Endpoint) - {e.Message}");
			return false;
		}
	}

	/// <summary>
	/// Validates the health of the SQL Server Reporting Services (SSRS) report server endpoint.
	/// Only performs validation if a report server URL is configured.
	/// </summary>
	/// <param name="details">Dictionary to store the validation results, keyed by component name</param>
	/// <returns>True if the report server is healthy or not configured, false if configured but unreachable</returns>
	protected bool ValidateReportServer(IDictionary<string, HealthStatus> details)
	{
		bool reportServerOk = true;
		string reportServerUrl = Configuration.SSRSServer?.url;

		if (!string.IsNullOrWhiteSpace(reportServerUrl))
		{
			reportServerOk = TestHttpEndpoint(reportServerUrl);
			details["report_server"] = reportServerOk ? HealthStatus.Ok : HealthStatus.Error;

			if (!reportServerOk)
				GenioDI.Log.Error($"Health Check (Report Server) - Couldn't connect to {reportServerUrl}");
		}

		return reportServerOk;
	}

	/// <summary>
	/// Validates the health of the chatbot API endpoint by testing HTTP connectivity.
	/// Only performs validation if an API endpoint is configured.
	/// </summary>
	/// <param name="details">Dictionary to store the validation results, keyed by component name</param>
	/// <returns>True if the chatbot API is healthy or not configured, false if configured but unreachable</returns>
	protected bool ValidateChatbotAPI(IDictionary<string, HealthStatus> details)
	{
		bool chatbotApiOk = true;
		string chatbotUrl = ChatbotService.EnsureApiUrl(Configuration.AiConfig.APIEndpoint);

		if (!string.IsNullOrWhiteSpace(chatbotUrl))
		{
			// Ensure the base URL has a trailing slash, otherwise, the Uri class won't be able to correctly build the final URL
			if (!chatbotUrl.EndsWith("/"))
				chatbotUrl += "/";

			Uri baseUri = new(chatbotUrl);
			// Access the health check endpoint of the chatbot service
			chatbotApiOk = TestHttpEndpoint(new Uri(baseUri, "health").AbsoluteUri);
			details["chatbot_api"] = chatbotApiOk ? HealthStatus.Ok : HealthStatus.Error;

			if (!chatbotApiOk)
				GenioDI.Log.Error($"Health Check (Chatbot) - Couldn't connect to {chatbotUrl}");
		}

		return chatbotApiOk;
	}

	/// <summary>
	/// Validates that the configuration file exists and contains required data system configurations
	/// </summary>
	/// <param name="details">Dictionary to store the validation results, keyed by component name</param>
	/// <returns>True if configuration file exists and contains at least one data system, false otherwise</returns>
	protected bool ValidateConfiguration(IDictionary<string, HealthStatus> details)
	{
		bool configOk = false;

		// Validate that the config file exists
		if (Configuration.ConfigVersion != null)
		{
			details["config_file"] = HealthStatus.Ok;

			// Validate that there are data systems configured
			configOk = Configuration.DataSystems?.Count > 0;
			details["database_config"] = configOk ? HealthStatus.Ok : HealthStatus.Error;

			if (!configOk)
				GenioDI.Log.Error("Health Check (Configuration) - No data system configured");
		}
		else
		{
			details["config_file"] = HealthStatus.Error;
			GenioDI.Log.Error("Health Check (Configuration) - No config file detected");
		}

		return configOk;
	}

	/// <summary>
	/// Validates the connection to a specific database system
	/// </summary>
	/// <param name="details">Dictionary to store the validation results, keyed by component name</param>
	/// <param name="dataSystem">The specific data system configuration to test</param>
	/// <returns>True if the data system connection is successful, false otherwise</returns>
	protected bool ValidateDBConnection(IDictionary<string, HealthStatus> details, DataSystemXml dataSystem)
	{
		bool dbOk = PersistentSupport.TestServerConnection(dataSystem);
		details[$"database_{dataSystem.Name}"] = dbOk ? HealthStatus.Ok : HealthStatus.Error;

		if (!dbOk)
			GenioDI.Log.Error($"Health Check (Data System) - Couldn't connect to data system {dataSystem.Name}");

		return dbOk;
	}

	/// <summary>
	/// Validates connections to all configured database systems by iterating through
	/// the configured data systems and testing each one individually.
	/// </summary>
	/// <param name="details">Dictionary to store the validation results, keyed by component name</param>
	/// <returns>True if all data system connections are successful, false if any connection fails</returns>
	protected bool ValidateDBConnection(IDictionary<string, HealthStatus> details)
	{
		bool dbOk = true;
		foreach (DataSystemXml dataSystem in Configuration.DataSystems ?? [])
			dbOk &= ValidateDBConnection(details, dataSystem);
		return dbOk;
	}

	/// <summary>
	/// Abstract method that must be implemented by derived classes to validate core platform resources.
	/// Implementation should call the appropriate validation methods based on platform-specific requirements.
	/// </summary>
	/// <returns>Dictionary containing the health status of all validated core resources</returns>
	protected abstract IDictionary<string, HealthStatus> ValidateCoreResources();

	/// <summary>
	/// Performs additional custom validation checks that are platform or environment specific.
	/// Override this method in derived classes to implement custom validation logic.
	/// </summary>
	/// <returns>A dictionary containing the health status of custom resources</returns>
	protected virtual IDictionary<string, HealthStatus> ValidateCustomResources()
	{
		// Default implementation returns empty dictionary (no custom validations)
		return new Dictionary<string, HealthStatus>();
	}

	/// <summary>
	/// Validates the health of all configured data systems
	/// </summary>
	/// <returns>A dictionary containing the health status of all system resources</returns>
	public IDictionary<string, HealthStatus> Validate()
	{
		IDictionary<string, HealthStatus> details = new Dictionary<string, HealthStatus>();

		// Check if we have a cached result
		string cacheKey = "SystemHealthCheck";
		if (QCache.Instance.User.Get(cacheKey) is HealthCheckResult lastResult)
		{
			LastResult = lastResult;
			return lastResult.Details;
		}

		try
		{
			// Add core resource validations
			details = ValidateCoreResources();

			// Add custom resource validations
			details = details
				.Union(ValidateCustomResources())
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}
		catch (Exception e)
		{
			// Ensure the final status is error, in case there's a low-level exception in one of the
			// validations and nothing is put in the details (should never happen, it's a last case scenario).
			details["unknown_resource"] = HealthStatus.Error;
			GenioDI.Log.Error($"Health Check - {e.Message}");
		}

		// Create and cache the combined result
		LastResult = new(details, environment);
		// Cache results for the specified interval to avoid abuse
		if (ValidationIntervalMillis > 0)
			QCache.Instance.User.Put(cacheKey, LastResult, TimeSpan.FromMilliseconds(ValidationIntervalMillis));

		return details;
	}

	/// <summary>
	/// Checks if all configured data systems are healthy
	/// </summary>
	/// <returns>True if all systems are healthy, false otherwise</returns>
	public bool IsHealthy()
	{
		Validate();
		return LastResult.IsOk;
	}
}
