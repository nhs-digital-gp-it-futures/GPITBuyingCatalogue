using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using Notify.Client;
using Notify.Interfaces;

namespace BuyingCatalogueFunction.Notifications
{
    public class NotificationFunction
    {
        private readonly IAsyncNotificationClient notificationClient;
        private readonly ILogger<NotificationFunction> logger;
        private readonly BuyingCatalogueDbContext dbContext;

        public NotificationFunction(
            IAsyncNotificationClient notificationClient,
            ILogger<NotificationFunction> logger,
            BuyingCatalogueDbContext dbContext)
        {
            this.notificationClient = notificationClient;
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [Function(nameof(NotificationFunction))]
        // public async Task Run([QueueTrigger("notifications", Connection = "AzureWebJobsStorage")] QueueMessage message)
        public async Task Run([QueueTrigger("notifications", Connection = "AzureWebJobsStorage")] string message)
        {
            try
            {
                var notification = await dbContext.Notifications.FindAsync(Guid.Parse(message));
                if (notification != null && string.IsNullOrWhiteSpace(notification.ReceiptId))
                {
                    var checkAlreadySentResponse = await notificationClient.GetNotificationsAsync(reference: message);
                    if (!checkAlreadySentResponse.notifications.Any())
                    {
                        var details = GetNotificationDetails(notification);
                        if (details != null)
                        {
                            var response = await notificationClient.SendEmailAsync(
                                notification.To,
                                details.GetTemplateId(),
                                // TODO: NotificationClient.PrepareUpload can throw ArgumentException("File is larger than 2MB")
                                // there would be no point retrying this message - consider handling this.
                                details.GetTemplatePersonalisation(NotificationClient.PrepareUpload),
                                notification.Id.ToString());
                            notification.ReceiptId = response.id;
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        // we've already send it but our notification doesn't have the receipt id
                        // so lets update that now.
                        if (checkAlreadySentResponse.notifications.Count == 1)
                        {
                            var woo = checkAlreadySentResponse.notifications.First();
                            notification.ReceiptId = woo.id;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch
            {
                // TODO: consider a custom exception to throw that we can catch and not throw to prevent retries
                throw;
            }
        }

        private GovNotifyEmailContent GetNotificationDetails(Notification notification)
        {
            switch (notification.NotificationType)
            {
                case NotificationTypeEnum.BuyerOrderCompleted:
                    return notification.JsonAs<BuyerOrderCompletedEmailContent>();
                default:
                    // TODO: consider a custom exception to throw that we can catch and not throw to prevent retries
                    // throw new InvalidOperationException("Uhandled - no point retrying ");
                    logger.LogWarning($"Unhandled notification type {notification.NotificationType}");
                    return null;
            }
        }
    }
}
