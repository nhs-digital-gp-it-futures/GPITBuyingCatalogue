using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation
{
    public sealed class AddUserModelValidator : AbstractValidator<AddUserModel>
    {
        public const string FirstNameError = "Enter a first name";
        public const string LastNameError = "Enter a last name";
        public const string NoEmailError = "Enter an email address";
        public const string EmailFormatError = "Enter an email address in the correct format, like name@example.com";
        public const string UserAlreadyExistsError = "A user with this email address is already registered on the Buying Catalogue";

        private readonly IUsersService usersService;

        public AddUserModelValidator(IUsersService usersService)
        {
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));

            RuleFor(m => m.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameError);

            RuleFor(m => m.LastName)
                .NotEmpty()
                .WithMessage(LastNameError);

            RuleFor(m => m.EmailAddress)
                .NotEmpty()
                .WithMessage(NoEmailError)
                .EmailAddress()
                .WithMessage(EmailFormatError)
                .Must(NotBeDuplicateUserEmail)
                .WithMessage(UserAlreadyExistsError);
        }

        private bool NotBeDuplicateUserEmail(string emailAddress) => !usersService.EmailAddressExists(emailAddress).GetAwaiter().GetResult();
    }
}
