using System;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation
{
    public sealed class AddUserModelValidator : AbstractValidator<AddUserModel>
    {
        private readonly IUsersService usersService;

        public AddUserModelValidator(IUsersService usersService)
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
                .Must(NotBeDuplicateUserEmail)
                .WithMessage("A user with this email address is already registered on the Buying Catalogue");

            RuleFor(x => x.SelectedAccountType)
                .NotEmpty()
                .WithMessage("Select an account type");
        }

        private bool NotBeDuplicateUserEmail(string emailAddress) => !usersService.EmailAddressExists(emailAddress).GetAwaiter().GetResult();
    }
}
