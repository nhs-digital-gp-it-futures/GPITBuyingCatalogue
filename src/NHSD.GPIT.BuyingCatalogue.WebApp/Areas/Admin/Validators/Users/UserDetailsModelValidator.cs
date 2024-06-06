using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users
{
    public class UserDetailsModelValidator : AbstractValidator<UserDetailsModel>
    {
        public const string AccountTypeMissingErrorMessage = "Select an account type";
        public const string AccountStatusMissingErrorMessage = "Select an account status";
        public const string EmailInUseErrorMessage = "A user with this email address already exists on the Buying Catalogue.";
        public const string EmailMissingErrorMessage = "Enter an email address";
        public const string EmailWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string EmailDomainInvalid = "This email domain cannot be used to register a new user account as it is not on the allow list";
        public const string FirstNameMissingErrorMessage = "Enter a first name";
        public const string LastNameMissingErrorMessage = "Enter a last name";
        public const string MustBelongToNhsDigitalErrorMessage = "Admins must be a member of the NHS";
        public const string MustNotExceedAccountManagerLimit = "There are already {0} active account managers for this organisation which is the maximum allowed";
        public const string OrganisationMissingErrorMessage = "Select an organisation";

        private readonly IUsersService usersService;

        public UserDetailsModelValidator(
            IUsersService usersService,
            IEmailDomainService emailDomainService,
            AccountManagementSettings accountManagementSettings)
        {
            this.usersService = usersService;

            RuleFor(x => x.SelectedOrganisationId)
                .NotEmpty()
                .WithMessage(OrganisationMissingErrorMessage);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameMissingErrorMessage);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(LastNameMissingErrorMessage);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(EmailMissingErrorMessage)
                .EmailAddress()
                .WithMessage(EmailWrongFormatErrorMessage)
                .Must((model, email) => NotBeDuplicateUserEmail(email, model.UserId))
                .WithMessage(EmailInUseErrorMessage)
                .Must(email => emailDomainService.IsAllowed(email).GetAwaiter().GetResult())
                .WithMessage(EmailDomainInvalid);

            RuleFor(x => x.SelectedAccountType)
                .NotEmpty()
                .WithMessage(AccountTypeMissingErrorMessage)
                .Must((model, accountType) => BelongToCorrectOrganisation(accountType, model.SelectedOrganisationId))
                .WithMessage(MustBelongToNhsDigitalErrorMessage)
                .Must((model, accountType) => AccountManagerLimit(accountType, model.SelectedOrganisationId, model.UserId, !model.IsActive.GetValueOrDefault(false)))
                .WithMessage(string.Format(MustNotExceedAccountManagerLimit, accountManagementSettings.MaximumNumberOfAccountManagers));

            RuleFor(x => x.IsActive)
                .NotEmpty()
                .WithMessage(AccountStatusMissingErrorMessage);
        }

        private static bool BelongToCorrectOrganisation(string accountType, int? selectedOrganisationId)
        {
            if (accountType != OrganisationFunction.Authority.Name)
                return true;

            return selectedOrganisationId == OrganisationConstants.NhsDigitalOrganisationId;
        }

        private bool AccountManagerLimit(string accountType, int? selectedOrganisationId, int userId, bool disabled)
        {
            return accountType != OrganisationFunction.AccountManager.Name || disabled
                || !usersService.IsAccountManagerLimit(selectedOrganisationId.GetValueOrDefault(), userId).Result;
        }

        private bool NotBeDuplicateUserEmail(string emailAddress, int userId)
        {
            return !usersService.EmailAddressExists(emailAddress, userId).Result;
        }
    }
}
