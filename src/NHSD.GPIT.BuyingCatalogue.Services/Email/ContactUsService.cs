using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Services.Email
{
    public sealed class ContactUsService : IContactUsService
    {
        public const string FullNameToken = "full_name";
        public const string EmailAddressToken = "email_address";
        public const string MessageToken = "message";

        private readonly ContactUsSettings settings;
        private readonly IGovNotifyEmailService govNotifyEmailService;

        public ContactUsService(
            ContactUsSettings settings,
            IGovNotifyEmailService govNotifyEmailService)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.govNotifyEmailService = govNotifyEmailService ?? throw new ArgumentNullException(nameof(govNotifyEmailService));
        }

        public async Task SubmitQuery(
            string fullName,
            string emailAddress,
            string message)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentNullException(nameof(fullName));

            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            var tokens = new Dictionary<string, dynamic>
            {
                [FullNameToken] = fullName,
                [EmailAddressToken] = emailAddress,
                [MessageToken] = message,
            };

            EmailAddressTemplate recipient = settings.GeneralQueriesRecipient;

            await Task.WhenAll(
                govNotifyEmailService.SendEmailAsync(recipient.Address, settings.AdminTemplateId, tokens),
                govNotifyEmailService.SendEmailAsync(emailAddress, settings.UserTemplateId, null));
        }
    }
}
