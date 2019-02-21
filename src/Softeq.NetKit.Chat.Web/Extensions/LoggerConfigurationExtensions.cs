// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Chat.Web.Extensions
{
    public static class LoggerConfigurationExtensions
    {
        public static IWebHostBuilder UseSerilog(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, collection) => collection.AddLogging(builder => SetupSerilog(context, collection)));
        }

        private static void SetupSerilog(WebHostBuilderContext hostingContext, IServiceCollection serviceCollection)
        {
            var applicationName = hostingContext.Configuration["Serilog:ApplicationName"];
            var environment = hostingContext.HostingEnvironment.EnvironmentName;
            var applicationSlotName = $"{applicationName}:{environment}";

            var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(hostingContext.Configuration)
                                                               .Enrich.WithProperty(PropertyNames.Application, applicationSlotName);

            var template = GetLogTemplate();

            if (hostingContext.HostingEnvironment.IsProduction() ||
                hostingContext.HostingEnvironment.IsStaging() ||
                hostingContext.HostingEnvironment.IsDevelopment())
            {
                var instrumentationKey = hostingContext.Configuration["ApplicationInsights:InstrumentationKey"];
                var telemetryClient = new TelemetryClient { InstrumentationKey = instrumentationKey };
                loggerConfiguration.WriteTo.ApplicationInsights(telemetryClient, new LoggerTelemetryConverter());

                serviceCollection.AddSingleton(telemetryClient);
            }
            else
            {
                loggerConfiguration.WriteTo.Debug(outputTemplate: template);
            }

            bool.TryParse(hostingContext.Configuration["Serilog:EnableLocalFileSink"], out var localFileSinkEnabled);
            if (localFileSinkEnabled)
            {
                loggerConfiguration.WriteTo.RollingFile("logs/log-{Date}.txt",
                                                        outputTemplate: template,
                                                        fileSizeLimitBytes: int.Parse(hostingContext.Configuration["Serilog:FileSizeLimitMBytes"]) * 1024 * 1024);
            }

            var logger = loggerConfiguration.CreateLogger();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                logger.Event("UnhandledExceptionCaughtByAppDomainUnhandledExceptionHandler")
                      .With.Message("Exception object = '{@ExceptionObject}'; Is terminating = '{IsTerminating}'", args.ExceptionObject, args.IsTerminating)
                      .AsFatal();
            };

            Log.Logger = logger;
        }

        private static string GetLogTemplate()
        {
            return new SerilogTemplateBuilder().Timestamp()
                                               .Level()
                                               .CorrelationId()
                                               .EventId()
                                               .Message()
                                               .NewLine()
                                               .Exception()
                                               .Build();
        }
    }
}
