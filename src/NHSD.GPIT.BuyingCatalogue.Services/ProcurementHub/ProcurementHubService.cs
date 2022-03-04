using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ProcurementHub;

namespace NHSD.GPIT.BuyingCatalogue.Services.ProcurementHub
{
    public class ProcurementHubService : IProcurementHubService
    {
        public const string FullNameToken = "full_name";
        public const string EmailAddressToken = "email_address";
        public const string OrganisationNameToken = "organisation_name";
        public const string OrganisationOdsCodeToken = "organisation_ods";
        public const string QueryToken = "query";

        public const string OdsCodeNotSupplied = "Not supplied";

        private readonly ProcurementHubMessageSettings settings;
        private readonly IGovNotifyEmailService emailService;

        public ProcurementHubService(
            ProcurementHubMessageSettings settings,
            IGovNotifyEmailService emailService)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task ContactProcurementHub(ProcurementHubRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var odsCode = string.IsNullOrWhiteSpace(request.OdsCode)
                ? OdsCodeNotSupplied
                : request.OdsCode;

            var tokens = new Dictionary<string, dynamic>
            {
                { FullNameToken, request.FullName },
                { EmailAddressToken, request.Email },
                { OrganisationNameToken, request.OrganisationName },
                { OrganisationOdsCodeToken, odsCode },
                { QueryToken, request.Query },
            };

            await emailService.SendEmailAsync(settings.Recipient.Address, settings.TemplateId, tokens);
        }
    }
}
