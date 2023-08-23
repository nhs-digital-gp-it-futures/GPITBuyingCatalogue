using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Services.Identity
{
    public class RequestAccountService : IRequestAccountService
    {
        public const string FullNameToken = "full_name";
        public const string PhoneNumberToken = "phone_number";
        public const string EmailAddressToken = "email_address";
        public const string OrganisationNameToken = "organisation_name";
        public const string OrganisationOdsCodeToken = "organisation_ods_code";
        public const string UserResearchConsentToken = "user_research_consent";

        public const string OdsCodeNotSupplied = "Not supplied";

        private readonly IGovNotifyEmailService emailService;
        private readonly RequestAccountMessageSettings settings;

        public RequestAccountService(
            IGovNotifyEmailService emailService,
            RequestAccountMessageSettings settings)
        {
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task RequestAccount(NewAccountDetails request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var odsCode = string.IsNullOrWhiteSpace(request.OdsCode)
                ? OdsCodeNotSupplied
                : request.OdsCode;

            var tokens = new Dictionary<string, dynamic>
            {
                { FullNameToken, request.FullName },
                { EmailAddressToken, request.EmailAddress },
                { OrganisationNameToken, request.OrganisationName },
                { OrganisationOdsCodeToken, odsCode },
                { UserResearchConsentToken, request.HasGivenUserResearchConsent.ToYesNo() },
            };

            await Task.WhenAll(
                emailService.SendEmailAsync(request.EmailAddress, settings.UserTemplateId, null),
                emailService.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, tokens));
        }
    }
}
