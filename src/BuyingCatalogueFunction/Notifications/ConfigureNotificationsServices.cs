using System.Diagnostics.CodeAnalysis;
using BuyingCatalogueFunction.Notifications.ContractExpiry.Interfaces;
using BuyingCatalogueFunction.Notifications.ContractExpiry.Services;
using BuyingCatalogueFunction.Notifications.Interfaces;
using BuyingCatalogueFunction.Notifications.PasswordExpiry.Interfaces;
using BuyingCatalogueFunction.Notifications.PasswordExpiry.Services;
using BuyingCatalogueFunction.Notifications.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Notify.Client;
using Notify.Interfaces;

namespace BuyingCatalogueFunction.Notifications;

[ExcludeFromCodeCoverage(Justification = "Registers dependencies in IoC container.")]
public sealed class ConfigureNotificationsServices : IConfigureServices
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TemplateOptions>(configuration.GetSection("template"));
        services.Configure<QueueOptions>(options =>
        {
            options.SendEmailNotifications = configuration.GetValue<string>("QUEUE:SEND_EMAIL_NOTIFICATION");
            options.CompleteEmailNotifications = configuration.GetValue<string>("QUEUE:COMPLETE_EMAIL_NOTIFICATION");
        });

        ConfigureGovNotify(services, configuration);

        services.AddTransient<IContractExpiryService, ContractExpiryService>();
        services.AddTransient<IPasswordExpiryService, PasswordExpiryService>();
        services.AddTransient<IEmailPreferenceService, EmailPreferenceService>();
    }

    private static void ConfigureGovNotify(IServiceCollection services, IConfiguration configuration)
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
    }
}
