using CSGenio.core.ai;
using CSGenio.core.di;
using CSGenio.core.logger;
using CSGenio.framework;
using CSGenio.persistence;
using GenioMVC;
using GenioMVC.Helpers;
using GenioServer.security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;

//---------------------------------
// Setup the GenioServer services
//---------------------------------
CSGenio.GenioDIDefault.Use();

//---------------------------------
// Setup the WebServer services
//---------------------------------
var builder = WebApplication.CreateBuilder(args);

// If it finds it, read the web.config as if it was a strongly typed Options provider
((IConfigurationBuilder)builder.Configuration).Add(new WebConfigConfigurationSource());

// Customize the default automatic validation behaviour to our custom on demand validation
builder.Services.AddSingleton<IObjectModelValidator>(s =>
{
    var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
    var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
    return new OnDemandObjectValidator(metadataProvider, options.ModelValidatorProviders, options);
});

//---------------------------------
// Telemetry Services
//---------------------------------
var telemetryConfig = builder.Configuration.GetSection("TelemetryConfig").Get<TelemetryConfiguration>();
builder.Services.ConfigureTelemetry(telemetryConfig, builder.Logging);

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ModuleActionFilter>();
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        //options.ModelBinderProviders.Insert(0, new GenioBindingProvider());
    }).AddSessionStateTempDataProvider()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; //leave property names unchanged

        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new BoolJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new SelectListJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new NameValueCollectionJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new ConcurrentStackJsonConverter<GenioMVC.Models.Navigation.HistoryLevel>());

        options.JsonSerializerOptions.TypeInfoResolver = new ConditionalJsonInfoResolver();
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// Add Http Client and Service for ChatbotAPI
builder.Services.AddHttpClient();
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddSingleton<IToolRepo>(McpToolFactory.AllGenioTools());

// Menu loader — singleton since it caches the XML on first access
builder.Services.AddSingleton<GenioMVC.Helpers.Menus.IMenuLoader, GenioMVC.Helpers.Menus.XmlMenuLoaderService>();

// Any controller that needs User information it can add UserContextService to its constructor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
// TODO: Replace all controller constructors that use the concrete class and replace it with the interface
builder.Services.AddScoped<UserContextService>();

// Add a shim authentication to provide compatibility with the old code
// It would actually handle the authentication, UserContextService will deal with that
// but it will simulate what old Asp.Net Mvc webapps initialized in HttpContext.User
// https://enzonunziata.com/article/custom-aspnet-core-authentication-part-1

// Setup Session
// The Distributed Memory Cache isn't an actual distributed cache. Cached items are stored by the app instance on the server where the app is running.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    builder.Configuration.GetSection("SessionOptions").Bind(options);
});

// Authentication handlers
var schemeName = LegacyFormsAuthenticationOptions.DefaultScheme;
builder.Services.AddAuthentication(schemeName)
    .AddScheme<LegacyFormsAuthenticationOptions, LegacyFormsAuthentication>(schemeName, options => {
        builder.Configuration.GetSection("LegacyFormsAuthentication").Bind(options);
    });

// gzip compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

// Background services (messaging, scheduling, ...)
builder.WebHost.UseShutdownTimeout(TimeSpan.FromSeconds(60));
builder.Services.AddHostedService<MessagingServiceHost>();

// USE /[MANUAL FOR APP_INIT]/

var app = builder.Build();

// Log unhandled exceptions and detail the error details in http 500 responses
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
app.UseResponseCompression();

// Use Open Telemetry Logging
if (telemetryConfig != null && telemetryConfig.LoggerType == TelemetryConfiguration.LoggerConfigType.OTLP)
    GenioDI.Log = new OpenTelemetryImpl(app.Services.GetRequiredService<ILoggerFactory>());

// Redirection needs to come before any routing in the pipeline
// Default will be redirecting to https.
// Set https_redirect to 'none' when reverse proxy deals already deals with https redirection.
// Set https_port when using a different https port than 443
string? httpsRedirect = app.Configuration["https_redirect"];
if (httpsRedirect == null || httpsRedirect == "redirect")
    app.UseHttpsRedirection();
if (httpsRedirect == "hsts")
    app.UseHsts();

// Callback paths calculations need to take into account reverse Proxys
// TODO: Get a utility class or service
AbsoluteUrlUtils.ProxyUrl = app.Configuration["ProxyUrl"] ?? "";

if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            // Disable cache for index html
            if (ctx.File.Name.Equals("index.html"))
            {
                ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
                ctx.Context.Response.Headers.Expires = "0";
            }
        }
    });
}

// AspCore wrapper already does this, so it's not needed
//app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// This is only needed when using the [ApiController] attributes
//app.MapControllers();

// External authentication endpoints
app.MapControllerRoute(
    name: "authRedirectRoute",
    pattern: "auth/{action}/{providerId}",
    defaults: new {
        culture = "en_US",
        system = Configuration.DefaultYear,
        module = "Public",
        controller = "Account"
    });


// Add specific MCP route
app.MapControllerRoute(
    name: "mcp",
    pattern: "mcp",
    defaults: new { controller = "Mcp", action = "HandleMcp" });


//Chatbot API proxy endpoints
var chatbotRoutes = new[]
{
    new { Name = "chatbotapi_submit", Pattern = "chatbotapi/prompt/submit", Action = "ChatbotApiStreamProxy" },
    new { Name = "chatbotapi_agent_response", Pattern = "chatbotapi/get-job-result", Action = "ChatbotApiStreamProxy" },
    new { Name = "chatbotapi_direct_agent_chat", Pattern = "chatbotapi/prompt/direct-agent-chat", Action = "ChatbotApiStreamProxy" },
    new { Name = "chatbotapi_clear", Pattern = "chatbotapi/prompt/clear", Action = "ChatbotApiProxy" },
    new { Name = "chatbotapi_load", Pattern = "chatbotapi/prompt/load", Action = "ChatbotApiProxy" },
    new { Name = "chatbotapi_cancel_execution", Pattern = "chatbotapi/prompt/cancel-execution", Action = "ChatbotApiProxy" }
};

foreach (var route in chatbotRoutes)
{
    app.MapControllerRoute(
        name: route.Name,
        pattern: route.Pattern,
        defaults: new { controller = "ChatbotApi", action = route.Action });
}

// Configuration and Antiforgery token
app.MapControllerRoute("config",
    "api/Config/{action}/{system}",
    new {
        controller = "Config",
        action = "GetConfig",
        system = Configuration.DefaultYear
    });

// User profile
app.MapControllerRoute("RouteForUsersProfile",
    "{culture}/{system}/User{action}/{id}",
    new {
        culture = "en-US",
        system = Configuration.DefaultYear,
        controller = "Home",
        action = "Profile",
        module = "Public"
    });

// Default route
app.MapControllerRoute("default",
    "api/{culture}/{system}/{module}/{controller}/{action}/{id?}",
    new {
        culture = "en-US",
        system = Configuration.DefaultYear,
        module = "Public",
        controller = "Home",
        action = "Index"
    });

// Health check endpoint
app.MapControllerRoute("health",
    "api/health",
    new {
        controller = "HealthCheck",
        action = "Index"
    });

app.Run();
