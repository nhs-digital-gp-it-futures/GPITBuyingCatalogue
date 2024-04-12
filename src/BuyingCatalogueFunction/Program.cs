using System.Threading.Tasks;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.EpicsAndCapabilities.Interfaces;
using BuyingCatalogueFunction.EpicsAndCapabilities.Services;
using BuyingCatalogueFunction.Notifications.Interfaces;
using BuyingCatalogueFunction.Notifications.Services;
using BuyingCatalogueFunction.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Notify.Client;
using Notify.Interfaces;
using Serilog;
using Serilog.Events;

namespace BuyingCatalogueFunction;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
#if DEBUG
            .WriteTo.Debug()
            .WriteTo.Seq("http://localhost:5341")
#endif
            .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces)
            .WriteTo.Console()
            .CreateLogger();

        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                services.Configure<TemplateOptions>(options =>
                {
                    options.ContractExpiryTemplateId = configuration.GetValue<string>("TEMPLATE:CONTRACT_EXPIRY_TEMPLATE_ID");
                });

                services.Configure<QueueOptions>(options =>
                {
                    options.SendEmailNotifications = configuration.GetValue<string>("QUEUE:SEND_EMAIL_NOTIFICATION");
                    options.CompleteEmailNotifications = configuration.GetValue<string>("QUEUE:COMPLETE_EMAIL_NOTIFICATION");
                });

                services.AddSingleton<IIdentityService, FunctionsIdentityService>();

                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();

                services.ConfigureGovNotify(configuration);
                services.ConfigureQueueStorage(configuration);

                services.AddTransient<IContractExpiryService, ContractExpiryService>();
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

    public static IServiceCollection ConfigureGovNotify(this IServiceCollection services, IConfiguration configuration)
    {
        var notifyApiKey = configuration.GetValue<string>("NOTIFY_API_KEY");
        if (!string.IsNullOrWhiteSpace(notifyApiKey))
        {
            services.AddScoped<IAsyncNotificationClient, NotificationClient>(sp => new NotificationClient(notifyApiKey));
            services.AddScoped<IGovNotifyEmailService, GovNotifyEmailService>();
        }
        else
        {
            services.AddScoped<IGovNotifyEmailService, FakeGovNotifyEmailService>();
        }

        return services;
    }

    public static void ConfigureQueueStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<QueueServiceClient>(_ => new(configuration.GetValue<string>("AzureWebJobsStorage")));
    }
}
