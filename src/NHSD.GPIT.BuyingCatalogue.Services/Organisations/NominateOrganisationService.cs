using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.Services.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class NominateOrganisationService : INominateOrganisationService
    {
        public const string FullNameToken = "full_name";
        public const string PhoneNumberToken = "phone_number";
        public const string EmailAddressToken = "email_address";
        public const string OrganisationNameToken = "organisation_name";
        public const string OrganisationOdsCodeToken = "organisation_ods";
        public const string NominatedOrganisationNameToken = "nominated_organisation_name";
        public const string NominatedOrganisationOdsCodeToken = "nominated_organisation_ods";

        public const string OdsCodeNotSupplied = "Not supplied";

        private readonly NominateOrganisationMessageSettings settings;
        private readonly IGovNotifyEmailService emailService;
        private readonly IOrganisationsService organisationsService;
        private readonly IUsersService usersService;

        public NominateOrganisationService(
            NominateOrganisationMessageSettings settings,
            IGovNotifyEmailService emailService,
            IOrganisationsService organisationsService,
            IUsersService usersService)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        public async Task NominateOrganisation(int userId, NominateOrganisationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var user = await usersService.GetUser(userId);

            if (user == null)
                throw new ArgumentOutOfRangeException(nameof(userId), userId.ToString(CultureInfo.InvariantCulture));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException($"User id {userId} does not have an email address set");

            var organisation = await organisationsService.GetOrganisation(user.PrimaryOrganisationId);

            if (organisation == null)
                throw new ArgumentException($"User id {userId} does not have a primary organisation set");

            var odsCode = string.IsNullOrWhiteSpace(request.OdsCode)
                ? OdsCodeNotSupplied
                : request.OdsCode;

            var tokens = new Dictionary<string, dynamic>
            {
                { FullNameToken, user.FullName },
                { PhoneNumberToken, user.PhoneNumber ?? string.Empty },
                { EmailAddressToken, user.Email },
                { OrganisationNameToken, organisation.Name },
                { OrganisationOdsCodeToken, organisation.ExternalIdentifier },
                { NominatedOrganisationNameToken, request.OrganisationName },
                { NominatedOrganisationOdsCodeToken, odsCode },
            };

            await Task.WhenAll(
                emailService.SendEmailAsync(user.Email, settings.UserTemplateId, null),
                emailService.SendEmailAsync(settings.AdminRecipient.Address, settings.AdminTemplateId, tokens));
        }

        public async Task<bool> IsGpPractice(int userId)
        {
            var user = await usersService.GetUser(userId);

            if (user == null)
                throw new ArgumentOutOfRangeException(nameof(userId), userId.ToString(CultureInfo.InvariantCulture));
            
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException($"User id {userId} does not have an email address set");

            if (user.PrimaryOrganisation == null)
                throw new ArgumentException($"User id {userId} does not have a primary organisation set");

            return user.PrimaryOrganisation.OrganisationType == OrganisationType.GP;
        }
    }
}
