using System.Threading.Tasks;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces;
using BuyingCatalogueFunction.EpicsAndCapabilities.Services;
using BuyingCatalogueFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using Notify.Client;
using Notify.Interfaces;

namespace BuyingCatalogueFunction;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.AddConsole();
            })
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddSingleton<IIdentityService, FunctionsIdentityService>();

                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();

                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();
                services.ConfigureGovNotify();
                services.ConfigureQueueStorage();

                services.AddTransient<ICapabilityService, CapabilityService>();
                services.AddTransient<IEpicService, EpicService>();
                services.AddTransient<IStandardService, StandardService>();
                services.AddTransient<IStandardCapabilityService, StandardCapabilityService>();

                services.AddDbContext<BuyingCatalogueDbContext>((_, options) =>
                {
                    options.UseSqlServer(configuration.GetValue<string>("BUYINGCATALOGUECONNECTIONSTRING"));
                    options.EnableSensitiveDataLogging();
                });
            })
            .Build();

        await host.RunAsync();
    }

    public static IServiceCollection ConfigureGovNotify(this IServiceCollection services)
    {
        var notifyApiKey = "bctest-1f322ce2-8cdd-495a-a128-4a984ef22a8d-045c6505-a9ae-43ad-b483-256e8811c413";
        if (!string.IsNullOrWhiteSpace(notifyApiKey))
        {
            services.AddScoped<IAsyncNotificationClient, NotificationClient>(sp => new NotificationClient(notifyApiKey));
            // services.AddScoped<IGovNotifyEmailService, GovNotifyEmailService>();
        }
        else
        {
            //services.AddScoped<IGovNotifyEmailService, FakeGovNotifyEmailService>();
        }

        return services;
    }

    public static void ConfigureQueueStorage(this IServiceCollection services) //, IConfiguration configuration)
    {
        // var settings = configuration.GetSection("AzureBlobSettings").Get<AzureBlobSettings>();

        //services.AddSingleton(settings);
        // GetEnvironmentVariable("AzureWebJobsStorage")
        //services.AddScoped<QueueServiceClient>(_ => new(settings.ConnectionString));
        services.AddScoped<QueueServiceClient>(_ => new(System.Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
    }


}
