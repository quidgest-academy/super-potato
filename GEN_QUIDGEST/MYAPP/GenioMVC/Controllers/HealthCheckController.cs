using GenioMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenioMVC.Controllers;

public class HealthCheckController : Controller
{
	/// <summary>
	/// Performs a comprehensive health check of the application and its dependencies.
	/// Returns HTTP 200 (OK) if all systems are healthy, or HTTP 503 (Service Unavailable) if any issues are detected.
	/// </summary>
	/// <returns>
	/// JSON response containing detailed health status information for all checked components.
	/// HTTP 200 if healthy, HTTP 503 if unhealthy.
	/// </returns>
	[HttpGet]
	[AllowAnonymous]
	public IActionResult Index()
	{
		// Get the current environment (Development, Staging, Production, etc.)
		// Defaults to "Production" if ASPNETCORE_ENVIRONMENT is not set
		string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
		WebAppHealthChecker healthChecker = new(environment);

		return healthChecker.IsHealthy()
			? Ok(healthChecker.LastResult.ToJson())
			: StatusCode(503, healthChecker.LastResult.ToJson());
	}
}
