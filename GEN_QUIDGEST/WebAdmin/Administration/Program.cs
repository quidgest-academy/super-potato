using Administration;
using Administration.Models;
using CSGenio.framework;
using CSGenio.persistence;
using CSGenio.config;
using GenioServer.security;
using log4net;
using log4net.Config;
using SoapCore;
using Administration.AuxClass;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using CSGenio.core.logger;
using CSGenio.core.di;

//---------------------------------
// Setup the GenioServer services
//---------------------------------
CSGenio.GenioDIDefault.Use();

//---------------------------------
// Setup the WebServer services
//---------------------------------
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; //leave property names unchanged
    })
    .AddXmlSerializerFormatters();


//---------------------------------
// Telemetry Services
//---------------------------------
var telemetryConfig = builder.Configuration.GetSection("TelemetryConfig").Get<TelemetryConfiguration>();
builder.Services.ConfigureTelemetry(telemetryConfig, builder.Logging);


//gzip compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

//Add chaching service
builder.Services.AddDistributedMemoryCache();

//Add SOAP service interface
builder.Services.AddSingleton<IAdminService, WebAPI>();
builder.Services.AddSingleton<IUserManagementService, UserManagement>();

//Add configuration manager
builder.Services.AddSingleton<CSGenio.config.IConfigurationManager>(new FileConfigurationManager(AppDomain.CurrentDomain.BaseDirectory));


// Support for installing as a machine service (windows or linux)
// In the cloud just install this as a normal WebApp with Always On option.
if (OperatingSystem.IsWindows())
    builder.Host.UseWindowsService();
if (OperatingSystem.IsLinux())
    builder.Host.UseSystemd();

//Background services (messaging, scheduling, ...)
builder.WebHost.UseShutdownTimeout(TimeSpan.FromSeconds(60));
builder.Services.AddHostedService<MessagingServiceHost>();
//register the scheduler host as a normal service as well so we can access it from the controllers
builder.Services.AddSingleton<SchedulerServiceHost>();
builder.Services.AddHostedService<SchedulerServiceHost>(p => p.GetRequiredService<SchedulerServiceHost>());


// USE /[MANUAL FOR APP_INIT]/

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.DocInclusionPredicate((_, apiDesc) =>
    {
        return apiDesc.ActionDescriptor.RouteValues["controller"] == "RestAdmin";
    });
});

//Add Cors
var corsSettings = builder.Configuration.GetSection("Cors").Get<CorsConfig>() ?? new CorsConfig { AllowedHeaders = ["Content-Type"], AllowedMethods = ["GET", "POST" ], AllowedOrigins = [] };

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebAdminCorsPolicy", policy =>
    {
        policy.WithOrigins(corsSettings.AllowedOrigins)
              .WithMethods(corsSettings.AllowedMethods)
              .WithHeaders(corsSettings.AllowedHeaders);
    });
});

var app = builder.Build();

app.UseRouting();
app.UseCors("WebAdminCorsPolicy");

//Map SOAP endpoint
((IEndpointRouteBuilder) app).UseSoapEndpoint<IAdminService>("/WebAPI.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer, true); //cast needed to solve ambiguity
((IEndpointRouteBuilder) app).UseSoapEndpoint<IUserManagementService>("/UserManagement.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer, true);

// Configure the HTTP request pipeline.
app.UseResponseCompression();

// Use Open Telemetry Logging
if (telemetryConfig != null && telemetryConfig.LoggerType == TelemetryConfiguration.LoggerConfigType.OTLP)
    GenioDI.Log = new OpenTelemetryImpl(app.Services.GetRequiredService<ILoggerFactory>());

// Redirection needs to come before any routing in the pipeline
// Default will be to use http.
// Set https_port when using a different https port than 443
string? https_redirect = app.Configuration["https_redirect"];
if (https_redirect == "redirect")
    app.UseHttpsRedirection();
if (https_redirect == "hsts")
    app.UseHsts();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

//This is only needed when using the [ApiController] attributes
app.MapControllers();

// Get default system
string defaultSystem = "0";

// Default route
app.MapControllerRoute("default",
    "api/{culture}/{system}/{controller}/{action}/{id?}",
    new {
        culture = Administration.AuxClass.Culture.CultureManager.DefaultCulture.Name,
        system = defaultSystem
    });

app.MapControllerRoute("defaultWithoutSystem",
    "api/{controller}/{action}/{id?}",
    new {
        culture = Administration.AuxClass.Culture.CultureManager.DefaultCulture.Name,
        system = defaultSystem
    });

// Health check endpoint
app.MapControllerRoute("health",
    "api/health",
    new {
        controller = "HealthCheck",
        action = "Index"
    });

app.Run();
