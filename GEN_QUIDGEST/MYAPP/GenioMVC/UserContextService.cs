using CSGenio.framework;
using GenioMVC.Helpers;
using GenioMVC.Helpers.Culture;
using GenioMVC.Helpers.Menus;
using GenioMVC.Models.Navigation;

namespace GenioMVC;

public class UserContextService : IUserContextService
{
	private readonly IMenuLoader _menuLoader;

	public UserContextService(IHttpContextAccessor context, IConfiguration configuration, IMenuLoader menuLoader)
	{
		_menuLoader = menuLoader;
		var httpContext = context.HttpContext;
		if (httpContext is null)
			throw new ArgumentNullException(nameof(httpContext));

		// Don't go any further unless the system has a configuration.
		if (Configuration.ConfigVersion is null)
			return;

		Current = new UserContext(httpContext, configuration);

		// Note: When a SameIP security policy is active this kind of Location resetting is invalid
		// In fact the user should be forbidden to continue to use the application, needing to log out of his previous location to login to the new one.
		User user = Current.User;
		user.Location = httpContext.GetIpAddress();

		// Extract transient state from the routing data
		var routeData = httpContext.GetRouteData().Values;
		if (routeData.TryGetValue("system", out object? rval) && rval != null && rval is string year)
			// Validate the user has rights to access this year, otherwise just attribute the default year
			user.Year = ValidateInputYear(year);

		if (routeData.TryGetValue("culture", out object? cval) && cval != null && cval is string culture)
		{
			// Validate this is a supported language, otherwise just attribute the default language
			user.Language = ValidateInputLang(culture).Replace("-", "").ToUpperInvariant();
			CultureManager.SetCulture(culture);
		}

		if (routeData.TryGetValue("module", out object? mval) && mval != null && mval is string module)
			// Validate this is a supported module, otherwise just attribute the Public module
			user.CurrentModule = ValidateInputModule(module);

		// Decode navigation information
		string? navigationId = null;
		if (httpContext.Request.Query.TryGetValue("nav", out var navValues))
			navigationId = navValues.FirstOrDefault();
		navigationId ??= "main";

		if (httpContext.Request.HasJsonContentType())
		{
			//Buffering must be enabled for the body to be processed multiple times,
			// once here, and another during model binding of the controller parameters.
			//https://github.com/dotnet/aspnetcore/issues/47350
			//https://github.com/dotnet/aspnetcore/issues/14396
			// The official recomendation does not seem to work, only resetting the stream position to 0 worked
			httpContext.Request.EnableBuffering();

			// Btw, this is a horrible hacky solution to inject the navigation data.
			// This mechanism should be changed to use either cookies or http headers for this out-of-band transport.
			var readTask = httpContext.Request.ReadFromJsonAsync<JsonNavDataContainer>();
			readTask.ConfigureAwait(false);
			SetNavigationContext(readTask.GetAwaiter().GetResult(), navigationId);

			httpContext.Request.Body.Position = 0;
		}
		else if (httpContext.Request.HasFormContentType)
		{
			var form = httpContext.Request.Form;
			JsonNavDataContainer navigation = new()
			{
				JsonNavigationData = form["JsonNavigationData"]
			};
			SetNavigationContext(navigation, navigationId);
		}
		else // Create a context with the navigationId passed
			SetNavigationContext(null, navigationId);

		// Check for maintenance Status
		Maintenance.GetMaintenanceStatus(Current.PersistentSupport);

		CSGenio.framework.Log.Debug(httpContext.Request.Method + " " + httpContext.Request.Path.ToString());
	}

	private string ValidateInputYear(string year)
	{
		if (!Current.User.IsGuest() && Current.User.Years.Contains(year))
			return year;
		return Configuration.DefaultYear;
	}

	private string ValidateInputLang(string lang)
	{
		if (CultureManager.CultureIsSupported(lang))
			return lang;
		return CultureManager.DefaultCultureName;
	}

	private string ValidateInputModule(string module)
	{
		var conditionValidator = new MenuConditionValidator(Current);
		var menuService = new UserMenuService(_menuLoader, conditionValidator, Current.User);
		if (menuService.UserHasAccessToModule(module))
			return module;
		return "Public";
	}

	public class JsonNavDataContainer
	{
		public string JsonNavigationData { get; set; } = string.Empty;
	}

	private void SetNavigationContext(JsonNavDataContainer? data, string navigationId)
	{
		string json = data?.JsonNavigationData ?? string.Empty;

		GenioMVC.Models.Navigation.NavigationContext? navigationContext = null;
		if (json != string.Empty)
		{
			var navigationContextBase = NavigationSerializer.Deserialize<NavigationContextBase>(json);

			if (navigationContextBase != null)
			{
				navigationContext = new GenioMVC.Models.Navigation.NavigationContext(Current, navigationContextBase);
				navigationContext.SaveOriginal();
			}
		}

		navigationContext ??= new GenioMVC.Models.Navigation.NavigationContext(Current) { NavigationId = navigationId };
		Current.SetNavigation(navigationContext);
	}

	public UserContext Current { get; private set; }
}
