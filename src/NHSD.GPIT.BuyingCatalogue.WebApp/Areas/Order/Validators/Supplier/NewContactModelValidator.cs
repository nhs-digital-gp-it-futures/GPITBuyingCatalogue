using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier
{
    public class NewContactModelValidator : AbstractValidator<NewContactModel>
    {
        public const string DepartmentErrorMessage = "Enter a department name";
        public const string EmailErrorMessage = "Enter an email address";
        public const string EmailWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string FirstNameErrorMessage = "Enter a first name";
        public const string LastNameErrorMessage = "Enter a last name";

        public NewContactModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .WithMessage(FirstNameErrorMessage);

            RuleFor(x => x.LastName)
                .NotNull()
                .WithMessage(LastNameErrorMessage);

            RuleFor(x => x.Email)
                .NotNull()
                .WithMessage(EmailErrorMessage)
                .EmailAddress()
                .WithMessage(EmailWrongFormatErrorMessage);
        }
    }
}
