using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using BuyingCatalogueFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace BuyingCatalogueFunction;

[ExcludeFromCodeCoverage(Justification = "Bootstraps IHost.")]
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

                var accountName = configuration.GetValue<string>("AzureWebJobsStorage:AccountName");
                var clientId = configuration.GetValue<string>("AzureWebJobsStorage:ClientId");

                services.AddAzureClients(builder =>
                {
                    if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(clientId))
                    {
                        TokenCredential credential =
                            new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(clientId));

                        builder.UseCredential(credential);

                        builder.AddQueueServiceClient(
                                new Uri($"https://{accountName}.queue.core.windows.net"))
                            .WithCredential(credential);
                    }
                    else
                    {
                        builder.AddQueueServiceClient(configuration.GetValue<string>("AzureWebJobsStorage"));
                    }
                });


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
