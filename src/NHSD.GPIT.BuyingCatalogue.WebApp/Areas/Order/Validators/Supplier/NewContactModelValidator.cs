using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier
{
    public class NewContactModelValidator : AbstractValidator<NewContactModel>
    {
        public const string ContactDetailsMissingErrorMessage = "Enter a telephone number or email address";
        public const string EmailAddressFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string FirstNameMissingErrorMessage = "Enter a first name";
        public const string LastNameMissingErrorMessage = "Enter a last name";
        public const string PersonalDetailsMissingErrorMessage = "Enter a contact name or department name";

        public NewContactModelValidator()
        {
            RuleFor(x => x)
                .Must(HaveEnteredPersonalDetails)
                .WithMessage(PersonalDetailsMissingErrorMessage)
                .OverridePropertyName(x => x.FirstName);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameMissingErrorMessage)
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(LastNameMissingErrorMessage)
                .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

            RuleFor(x => x)
                .Must(HaveEnteredContactDetails)
                .WithMessage(ContactDetailsMissingErrorMessage)
                .OverridePropertyName(x => x.PhoneNumber);

            RuleFor(m => m.Email)
                .EmailAddress()
                .WithMessage(EmailAddressFormatErrorMessage)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
        }

        private static bool HaveEnteredContactDetails(NewContactModel model)
        {
            return !string.IsNullOrWhiteSpace(model.PhoneNumber)
                || !string.IsNullOrWhiteSpace(model.Email);
        }

        private static bool HaveEnteredPersonalDetails(NewContactModel model)
        {
            return !string.IsNullOrWhiteSpace(model.FirstName)
                || !string.IsNullOrWhiteSpace(model.LastName)
                || !string.IsNullOrWhiteSpace(model.Department);
        }
    }
}
