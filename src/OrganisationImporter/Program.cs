using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrganisationImporter.Services;
using Serilog;

namespace OrganisationImporter
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var builder = new HostBuilder();

            builder.ConfigureWebJobs(_ => { });
            builder.ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder.AddEnvironmentVariables();
                _configuration = configBuilder.Build();
            });

            builder.ConfigureLogging(loggingBuilder => loggingBuilder.AddSerilog());
            builder.ConfigureServices(ConfigureServices);

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
            services.AddSingleton<OrganisationImportService>();
        }
    }
}
