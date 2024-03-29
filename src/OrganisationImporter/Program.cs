﻿using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Services;
using Serilog;

namespace OrganisationImporter
{
    public static class Program
    {
        private static IConfigurationRoot _configuration;
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureWebJobs()
                .ConfigureAppConfiguration(configBuilder => _configuration = configBuilder.Build())
                .ConfigureServices(ConfigureServices)
                .UseSerilog((_, services, loggerConfiguration) =>
                {
                    var telemetryConfig = TelemetryConfiguration.CreateDefault();
                    telemetryConfig.ConnectionString = _configuration["APPLICATIONINSIGHTS:CONNECTION_STRING"];
                    loggerConfiguration
                        .ReadFrom.Configuration(_configuration)
                        .WriteTo
                        .Console()
                        .WriteTo
                        .ApplicationInsights(
                            telemetryConfig,
                            TelemetryConverter.Traces);
                });

            var host = builder.Build();

            using (host)
            {
                var jobHost = host.Services.GetService(typeof(IJobHost)) as JobHost;
                var organisationImportService = host.Services.GetService<OrganisationImportService>();
                var inputs = new Dictionary<string, object>
                {
                    { "service", organisationImportService },
                    { "importUrl", _configuration.GetValue<Uri>("TRUD_URL") }
                };

                await host.StartAsync();
                await jobHost!.CallAsync(nameof(DoImport), inputs);
                await host.StopAsync();
            }
        }

        [NoAutomaticTrigger]
        public static async Task DoImport(OrganisationImportService service, Uri importUrl)
        {
            await service.ImportFromUrl(importUrl);
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddApplicationInsightsTelemetryWorkerService();
            services.AddDbContext<BuyingCatalogueDbContext>(opts =>
            {
                var connectionString = _configuration.GetValue<string>("BUYINGCATALOGUECONNECTIONSTRING");
                opts.UseSqlServer(connectionString);
                opts.EnableSensitiveDataLogging();
            });

            services.AddSingleton<OrganisationImportService>();
            services.AddSingleton<IHttpService, HttpService>();
            services.AddSingleton<ITrudService, TrudService>();
            services.AddSingleton<IZipService, ZipService>();
        }
    }
}
