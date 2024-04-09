using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Notifications.Interfaces;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.Notifications.Services
{
    public class FakeGovNotifyEmailService : IGovNotifyEmailService
    {
        private readonly ILogger<FakeGovNotifyEmailService> logger;

        public FakeGovNotifyEmailService(ILogger<FakeGovNotifyEmailService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string[]> CheckForExisting(string notificationId)
        {
            return await Task.FromResult(Array.Empty<string>());
        }

        public async Task<string> SendEmailAsync(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation, string notficationId)
        {
            logger.LogInformation("Send email requested for {EmailNotificationId} using {TemplateId}", notficationId, templateId);
            return await Task.FromResult("fake-id");
        }
    }
}
