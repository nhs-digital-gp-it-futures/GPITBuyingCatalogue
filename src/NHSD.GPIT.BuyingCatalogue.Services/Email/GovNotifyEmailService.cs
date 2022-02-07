using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using Notify.Exceptions;
using Notify.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.Services.Email
{
    public class GovNotifyEmailService : IGovNotifyEmailService
    {
        private readonly IAsyncNotificationClient notificationClient;
        private readonly ILogger<GovNotifyEmailService> logger;

        public GovNotifyEmailService(
            IAsyncNotificationClient notificationClient,
            ILogger<GovNotifyEmailService> logger)
        {
            this.notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation)
        {
            try
            {
                await notificationClient.SendEmailAsync(emailAddress, templateId, personalisation);
            }
            catch (NotifyClientException exception)
            {
                logger.LogError(exception, "An error occurred when calling the NotificationClient: {Message}", exception.Message);
                throw;
            }
        }
    }
}
