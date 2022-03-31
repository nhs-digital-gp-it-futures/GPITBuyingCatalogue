using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Users
{
    public class PersonalDetailsModelValidator : AbstractValidator<PersonalDetailsModel>
    {
        public const string FirstNameMissingErrorMessage = "Enter a first name";
        public const string LastNameMissingErrorMessage = "Enter a last name";
        public const string EmailMissingErrorMessage = "Enter an email address";
        public const string EmailWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string EmailInUseErrorMessage = "A user with this email address is already registered on the Buying Catalogue";

        private readonly IUsersService usersService;

        public PersonalDetailsModelValidator(IUsersService usersService)
        {
            this.usersService = usersService;

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
                .Must((model, email) => NotBeDuplicateUserEmail(email, model.UserId))
                .WithMessage(EmailInUseErrorMessage);
        }

        private bool NotBeDuplicateUserEmail(string emailAddress, int userId)
        {
            return !usersService.EmailAddressExists(emailAddress, userId).Result;
        }
    }
}
