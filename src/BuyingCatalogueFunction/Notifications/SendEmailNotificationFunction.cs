using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.Notifications.Interfaces;
using BuyingCatalogueFunction.Notifications.Templates;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Notify.Client;

namespace BuyingCatalogueFunction.Notifications
{
    public class SendEmailNotificationFunction
    {
        private readonly TemplateOptions templateOptions;
        private readonly QueueOptions queueOptions;
        private readonly IGovNotifyEmailService emailService;
        private readonly ILogger<SendEmailNotificationFunction> logger;
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly QueueServiceClient queueServiceClient;

        public SendEmailNotificationFunction(
            IOptions<TemplateOptions> templateOptions,
            IOptions<QueueOptions> queueOptions,
            IGovNotifyEmailService emailService,
            ILogger<SendEmailNotificationFunction> logger,
            BuyingCatalogueDbContext dbContext,
            QueueServiceClient queueServiceClient)
        {
            this.templateOptions = templateOptions.Value;
            this.queueOptions = queueOptions.Value;
            this.emailService = emailService;
            this.logger = logger;
            this.dbContext = dbContext;
            this.queueServiceClient = queueServiceClient;
        }

        [Function(nameof(SendEmailNotification))]
        public async Task SendEmailNotification([QueueTrigger("%QUEUE:SEND_EMAIL_NOTIFICATION%", Connection = "AzureWebJobsStorage")] string message)
        {
            try
            {
                if (!Guid.TryParse(message, out var guid))
                {
                    throw new NoneTransientException($"{nameof(SendEmailNotification)} - Invalid message {message}");
                }

                var notification = await dbContext.EmailNotifications.FindAsync(guid);
                if (notification != null && string.IsNullOrWhiteSpace(notification.ReceiptId))
                {
                    var existingNotificationIds = await emailService.CheckForExisting(message);
                    if (!existingNotificationIds.Any())
                    {
                        var details = GetNotificationDetails(notification);
                        if (details != null)
                        {
                            await Send(notification, details);
                            await Complete(message);
                        }
                    }
                    else
                    {
                        // we've already sent it but our notification doesn't have the receipt id
                        // so lets update that now.
                        if (existingNotificationIds.Length == 1)
                        {
                            Update(notification, existingNotificationIds.First());
                            await Complete(message);
                        }
                    }
                }
            }
            catch (NoneTransientException ex)
            {
                logger.LogError(
                    ex,
                    "{Name} error - {Message}. None transient exception not thrown to suppress retries",
                    nameof(SendEmailNotification),
                    message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Name} error - {Message}.",
                    nameof(SendEmailNotification),
                    message);
                throw;
            }
        }

        private void Update(EmailNotification notification, string id)
        {
            notification.ReceiptId = id;
            dbContext.SaveChanges();
        }

        private async Task Send(EmailNotification notification, GovNotifyEmailTemplate details)
        {
            var id = await emailService.SendEmailAsync(
                notification.To,
                details.GetTemplateId(templateOptions),
                details.GetTemplatePersonalisation(),
                notification.Id.ToString());
            notification.ReceiptId = id;
            dbContext.SaveChanges();
        }

        private async Task Complete(string message)
        {
            var client = queueServiceClient.GetQueueClient(queueOptions.CompleteEmailNotifications);
            await client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
        }

        private GovNotifyEmailTemplate GetNotificationDetails(EmailNotification notification)
        {
            switch (notification.EmailNotificationType)
            {
                case EmailNotificationTypeEnum.ContractDueToExpire:
                    return new ContractDueToExpireEmailTemplate(notification.JsonAs<ContractDueToExpireEmailModel>());
                default:
                    throw new NoneTransientException($"Unhandled notification type {notification.EmailNotificationType}");
            }
        }
    }
}
