using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CSGenio.core.logger;
using CSGenio.framework;
using GenioMVC.Helpers.Menus;

namespace GenioMVC.Controllers
{
	public class ConfigController : ControllerExtension
	{
		private readonly bool enableOtlpTracing;

		public ConfigController(UserContextService userContextService, IConfiguration config) : base(userContextService)
		{
			var telemetryConfig = config.GetSection("TelemetryConfig").Get<TelemetryConfiguration>();
			enableOtlpTracing = telemetryConfig?.EnableTracing ?? false;
		}

		private object getVersionInfo()
		{
			return new
			{
				buildVersion = VersionInfo.Build,
				dbIdxVersion = VersionInfo.DatabaseIndex,
				dbVersion = VersionInfo.DatabaseSchema.ToString(),
				genioVersion = VersionInfo.GenioVersion.Replace('.', ','),
				trackChangesVersion = VersionInfo.Release,
				assemblyVersion = VersionInfo.GenAssemblyVersion,
				generationDate = new
				{
					year = VersionInfo.GenerationDate.Year,
					month = VersionInfo.GenerationDate.Month,
					day = VersionInfo.GenerationDate.Day
				}
			};
		}

		private object getConfig()
		{
			User user = m_userContext.User;

			string defaultSystem = Configuration.DefaultYear;
			List<string> years = Configuration.Years;

			/*
				When the user is already authenticated, we need to validate whether the year
					in which the request was made is among the years to which they have permission.
				This is necessary to be able to correctly validate permissions.
				At the end, the year on which the validation was performed will be returned to change the client-side if necessary (currentSystem).
				The same applies to the default year; if it is not one of the years the user has access to,
					the first year in the allowed list will be assigned as the default.
			*/
			if (!user.IsGuest() && user.Years?.Count > 0)
			{
				if (!user.Years.Contains(defaultSystem))
					defaultSystem = user.Years.First();
				if (!user.Years.Contains(user.Year))
					user.Year = defaultSystem;
				years = user.Years;
			}

			// Modules
			List<MenuEntry> availableModulesMenus = Menus.AvailableModules(m_userContext);
			var availableModules = availableModulesMenus.Select(m => new {
				id = m.ID,
				title = m.Title,
				vector = m.Vector,
				font = m.Font,
				image = m.ImageVUE
			}).ToDictionary(m => m.id, m => m);

			string defaultModule = availableModules.FirstOrDefault().Key ?? "Public";
			string currentModule = user.CurrentModule;
			if (currentModule == null || !availableModules.ContainsKey(currentModule))
				currentModule = defaultModule;

			// Number format
			object numberFormat = new
			{
				Configuration.NumberFormat.DecimalSeparator,
				Configuration.NumberFormat.GroupSeparator,
				Configuration.NumberFormat.NegativeFormat
			};

			// DateTime format's
			object dateFormat = new
			{
				time = Configuration.DateFormat.Time,
				date = Configuration.DateFormat.Date,
				dateTime = Configuration.DateFormat.DateTime,
				dateTimeSeconds = Configuration.DateFormat.DateTimeSeconds
			};

			// Full Calendar license
			string schedulerLicense = Configuration.ExistsProperty("SchedulerLicense") ? Configuration.GetProperty("SchedulerLicense") : null;

			// Home page
			bool isGuestUser = user.IsGuest();
			ViewModels.Home.HomePage_ViewModel homePages = new(m_userContext, isGuestUser);

			// Password Recover
			bool hasPasswordRecovery = GenioServer.security.SecurityFactory.HasPasswordManagement() && !string.IsNullOrEmpty(Configuration.PasswordRecoveryEmail);

			// Authentification
			bool hasUsernameAuth = GenioServer.security.SecurityFactory.HasUsernameAuth();

			return new
			{
				availableModules,
				defaultModule,
				currentModule,
				years,
				defaultSystem,
				currentSystem = user.Year,
				defaultListRows = Configuration.NrRegDBedit,
				userName = user.Name ?? "guest",
				numberFormat,
				dateFormat,
				schedulerLicense,
				homePages = homePages.GetAvaibleHomePages(availableModules.Keys.ToList()),
				hasPasswordRecovery,
				hasUsernameAuth,
				eventTracking = Configuration.EventTracking,
				enableTracing = enableOtlpTracing,
				versionInfo = getVersionInfo()
			};
		}

		[HttpGet]
		[AllowAnonymous]
		public JsonResult GetConfig()
		{
			object conf = getConfig();
			return JsonOK(conf);
		}

		[HttpGet]
		[AllowAnonymous]
		public JsonResult GetAFToken()
		{
			/*
			AntiForgery.GetTokens(null, out string cookieToken, out string formToken);
			Response.SetCookie(new System.Web.HttpCookie(AntiForgeryConfig.CookieName, cookieToken));
			return JsonOK(new { formToken });
			*/

			return JsonOK();
		}

		/// <summary>
		/// Returns system version information from the server
		/// </summary>
		[HttpGet]
		[AllowAnonymous]
		public JsonResult GetVersionInfo()
		{
			return JsonOK(getVersionInfo());
		}
	}
}
