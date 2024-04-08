using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace BuyingCatalogueFunction.Notifications
{
    public class CompleteEmailNotificationFunction
    {
        private readonly QueueOptions queueOptions;
        private readonly ILogger<SendEmailNotificationFunction> logger;
        private readonly BuyingCatalogueDbContext dbContext;

        public CompleteEmailNotificationFunction(
            IOptions<QueueOptions> queueOptions,
            ILogger<SendEmailNotificationFunction> logger,
            BuyingCatalogueDbContext dbContext,
            QueueServiceClient queueServiceClient)
        {
            this.queueOptions = queueOptions.Value;
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [Function(nameof(CompleteEmailNotification))]
        public async Task CompleteEmailNotification([QueueTrigger("%QUEUE:COMPLETE_EMAIL_NOTIFICATION%", Connection = "AzureWebJobsStorage")] string message)
        {
            try
            {
                if (!Guid.TryParse(message, out var guid))
                {
                    throw new NoneTransientException($"{nameof(CompleteEmailNotification)} - Invalid message {message}");
                }

                var notification = await dbContext.EmailNotifications.FindAsync(guid);
                if (notification != null)
                {
                    dbContext.EmailNotifications.Remove(notification);
                    dbContext.SaveChanges();
                }
            }
            catch (NoneTransientException ex)
            {
                logger.LogError(
                    ex,
                    "{Name} error - {Message}. None transient exception not thrown to suppress retries",
                    nameof(CompleteEmailNotification),
                    message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "{Name} error - {message}.",
                    nameof(CompleteEmailNotification),
                    message);
                throw;
            }
        }
    }
}
