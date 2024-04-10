using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Notifications.Interfaces;
using Notify.Interfaces;

namespace BuyingCatalogueFunction.Notifications.Services
{

    public class GovNotifyEmailService : IGovNotifyEmailService
    {
        private readonly IAsyncNotificationClient notificationClient;

        public GovNotifyEmailService(
            IAsyncNotificationClient notificationClient)
        {
            this.notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
        }

        public async Task<string[]> CheckForExisting(string notificationId)
        {
            var existing = await notificationClient.GetNotificationsAsync(reference: notificationId);
            return existing
                .notifications
                .Select(n => n.id)
                .ToArray();
        }

        public async Task<string> SendEmailAsync(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation, string notficationId)
        {
            var response = await notificationClient.SendEmailAsync(
                emailAddress,
                templateId,
                personalisation,
                notficationId);

            return response.id;
        }
    }
}
