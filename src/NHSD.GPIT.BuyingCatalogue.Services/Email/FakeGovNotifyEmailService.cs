using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Services.Email
{
    public class FakeGovNotifyEmailService : IGovNotifyEmailService
    {
        private readonly ILogger<FakeGovNotifyEmailService> logger;

        public FakeGovNotifyEmailService(ILogger<FakeGovNotifyEmailService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SendEmailAsync(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation)
        {
            var data = new Dictionary<string, string>();
            data["EmailAddress"] = emailAddress;
            data["TemplateId"] = templateId;
            foreach (var p in personalisation ?? new Dictionary<string, dynamic>())
                data.Add(p.Key, p.Value.ToString());

            logger.LogInformation("Email request {@Data}", data);

            return Task.CompletedTask;
        }
    }
}
