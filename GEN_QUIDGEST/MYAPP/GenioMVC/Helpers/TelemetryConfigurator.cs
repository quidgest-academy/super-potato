using CSGenio.core.di;
using CSGenio.core.logger;
using GenioMVC.Metrics;
using log4net.Config;
using log4net;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using CSGenio.framework;

namespace GenioMVC.Helpers
{
    public static class TelemetryConfigurator
    {
        public static void ConfigureTelemetry(this IServiceCollection services, TelemetryConfiguration telemetryConfig, ILoggingBuilder loggingBuilder)
        {
            
            var serviceInstanceId = telemetryConfig?.CustomInstanceId;
            if (string.IsNullOrEmpty(serviceInstanceId))
                serviceInstanceId = Environment.GetEnvironmentVariable("TELEMETRY_CUSTOM_INSTANCE_ID");
            if (string.IsNullOrEmpty(serviceInstanceId))
            {
                //Persist the instanceId so its perserved between service restarts
                var ifile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", "InstanceId.var");
                if (File.Exists(ifile))
                {
                    serviceInstanceId = File.ReadAllText(ifile);
                }
                else
                {
                    serviceInstanceId = Guid.NewGuid().ToString();
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp"));
                    File.WriteAllText(ifile, serviceInstanceId);
                }
            }

            //Setup the service naming conventions that will label the telemetry scopes
            var serviceName = ResourceBuilder.CreateDefault().AddService(
                !string.IsNullOrEmpty(telemetryConfig?.CustomApplicationId) ? telemetryConfig.CustomApplicationId : Configuration.Application.Id,
                Configuration.Program + "." + Configuration.Acronym,
                Configuration.GenAssemblyVersion,
                false,
                serviceInstanceId);

            // Configure Metrics
            ConfigureMetrics(services, telemetryConfig, serviceName);
            
            // Configure Logging
            ConfigureLogging(loggingBuilder, telemetryConfig, serviceName);

            // Configure Tracing
            ConfigureTracing(services, telemetryConfig, serviceName);
        }

        private static void ConfigureLogging(ILoggingBuilder loggingBuilder, TelemetryConfiguration telemetryConfig, ResourceBuilder serviceName)
        {
            if (telemetryConfig != null && telemetryConfig.LoggerType == TelemetryConfiguration.LoggerConfigType.OTLP)
            {
                loggingBuilder.AddOpenTelemetry(options =>
                {
                    options.IncludeScopes = true;
                    options.SetResourceBuilder(serviceName);
                    options.AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(telemetryConfig.CollectorAddress));
                });
            }
            else
            {
                var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new FileInfo("web.config"));
            }
        }

        private static void ConfigureMetrics(IServiceCollection services, TelemetryConfiguration telemetryConfig, ResourceBuilder serviceName)
        {
            if (telemetryConfig == null)
            {
                GenioDI.MetricsOtlp = new MetricsOtlpImpl();
                return;
            }
            
            Meter mainMeter = new Meter("MainMeter");

            services.AddOpenTelemetry().WithMetrics(options =>
            {
                options.SetResourceBuilder(serviceName);
                options.AddMeter(mainMeter.Name);
                options.AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(telemetryConfig.CollectorAddress));

                // Asp .Netcore builtin metrics
                if (telemetryConfig.EnableInternalMetrics)
                {
                    options.AddAspNetCoreInstrumentation();
                    options.AddRuntimeInstrumentation();
                    options.AddProcessInstrumentation();
                }
            });

            GenioDI.MetricsOtlp = new MetricsOtlpImpl(mainMeter);
        }

        private static void ConfigureTracing(IServiceCollection services, TelemetryConfiguration telemetryConfig, ResourceBuilder serviceName)
        {
            if (telemetryConfig == null || !telemetryConfig.EnableTracing) return;

            services.AddOpenTelemetry().WithTracing(builder =>
            {
                builder.SetResourceBuilder(serviceName);
                builder.AddAspNetCoreInstrumentation(options =>
                {
                    options.EnrichWithHttpResponse = (activity, httpResponse) =>
                    {
                        var request = httpResponse.HttpContext.Request;

                        foreach (var routeVal in request.RouteValues)
                            activity.SetTag($"http.route.{routeVal.Key}", routeVal.Value.ToString());

                        foreach (var queryVal in request.Query)
                            activity.SetTag($"http.query.{queryVal.Key}", queryVal.Value.ToString());

                        activity.DisplayName = $"{request.Method} {request.Scheme} {request.Path}";

                        activity.SetTag("user.username", httpResponse.HttpContext.User.Identity.Name);
                        activity.SetTag("user.ip_address", httpResponse.HttpContext.GetIpAddress());
                    };
                })
                .AddHttpClientInstrumentation()
                .AddSource("ClientSide.Telemetry")
                .AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(telemetryConfig.CollectorAddress));
            });
        }
    }
}
