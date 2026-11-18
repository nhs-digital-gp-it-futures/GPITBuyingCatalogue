using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation
{
    public sealed class UserDetailsModelValidator : AbstractValidator<UserDetailsModel>
    {
        private readonly IUsersService usersService;

        public UserDetailsModelValidator(IUsersService usersService, IEmailDomainService emailDomainService)
        {
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));

            RuleFor(m => m.FirstName)
                .NotEmpty()
                .WithMessage("Enter a first name");

            RuleFor(m => m.LastName)
                .NotEmpty()
                .WithMessage("Enter a last name");

            RuleFor(m => m.EmailAddress)
                .NotEmpty()
                .WithMessage("Enter an email address")
                .EmailAddress()
                .WithMessage("Enter an email address in the correct format, like name@example.com")
                .Must((model, email) => NotBeDuplicateUserEmail(email, model.UserId))
                .WithMessage("A user with this email address is already registered on the Buying Catalogue")
                .Must(email => emailDomainService.IsAllowed(email).GetAwaiter().GetResult())
                .WithMessage("This email domain cannot be used to register a new user account as it is not on the allow list");

            RuleFor(x => x.SelectedAccountType)
                .NotEmpty()
                .WithMessage("Select an account type")
                .When(x => x.OrganisationId != OrganisationConstants.NhsDigitalOrganisationId);

            RuleFor(x => x.IsActive)
                .NotEmpty()
                .WithMessage("Select an account status");
        }

        private bool NotBeDuplicateUserEmail(string emailAddress, int userId) => !usersService.EmailAddressExists(emailAddress, userId).GetAwaiter().GetResult();
    }
}
