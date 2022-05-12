using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users
{
    public class AddModelValidator : AbstractValidator<AddModel>
    {
        public const string AccountTypeMissingErrorMessage = "Select an account type";
        public const string EmailInUseErrorMessage = "A user with this email address already exists on the Buying Catalogue.";
        public const string EmailMissingErrorMessage = "Enter an email address";
        public const string EmailWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string FirstNameMissingErrorMessage = "Enter a first name";
        public const string LastNameMissingErrorMessage = "Enter a last name";
        public const string MustBelongToNhsDigitalErrorMessage = "Admins must be a member of NHS Digital";
        public const string OrganisationMissingErrorMessage = "Select an organisation";

        private readonly IUsersService usersService;

        public AddModelValidator(IUsersService usersService)
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
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(EmailMissingErrorMessage)
                .EmailAddress()
                .WithMessage(EmailWrongFormatErrorMessage)
                .Must(NotBeDuplicateUserEmail)
                .WithMessage(EmailInUseErrorMessage);

            RuleFor(x => x.SelectedAccountType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(AccountTypeMissingErrorMessage)
                .Must((model, accountType) => BelongToCorrectOrganisation(accountType, model.SelectedOrganisationId))
                .WithMessage(MustBelongToNhsDigitalErrorMessage);
        }

        private static bool BelongToCorrectOrganisation(string accountType, string selectedOrganisationId)
        {
            if (accountType != OrganisationFunction.AuthorityName)
                return true;

            return selectedOrganisationId == $"{OrganisationConstants.NhsDigitalOrganisationId}";
        }

        private bool NotBeDuplicateUserEmail(string emailAddress)
        {
            return !usersService.EmailAddressExists(emailAddress).Result;
        }
    }
}
