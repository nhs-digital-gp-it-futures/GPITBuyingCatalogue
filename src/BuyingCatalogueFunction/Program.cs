using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace BuyingCatalogueFunction;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddSingleton<IIdentityService, FunctionsIdentityService>();
                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();

                services.AddScoped<QueueServiceClient>(_ => new(configuration.GetValue<string>("AzureWebJobsStorage")));
                services.AddDbContext<BuyingCatalogueDbContext>((_, options) =>
                {
                    options.UseSqlServer(configuration.GetValue<string>("BUYINGCATALOGUECONNECTIONSTRING"));
                    options.EnableSensitiveDataLogging();
                });

                services.ConfigureDependentServices(configuration);
            })
            .Build();

        await host.RunAsync();
    }

    private static void ConfigureDependentServices(this IServiceCollection services, IConfiguration configuration)
    {
        var configureServicesType = typeof(IConfigureServices);
        var configureServicesTypes = configureServicesType.Assembly.GetTypes()
            .Where(x => x.IsClass && configureServicesType.IsAssignableFrom(x))
            .Select(x => (IConfigureServices)Activator.CreateInstance(x))
            .Where(x => x != null);

        foreach (var implementation in configureServicesTypes)
        {
            implementation.ConfigureServices(services, configuration);
        }
    }
}
