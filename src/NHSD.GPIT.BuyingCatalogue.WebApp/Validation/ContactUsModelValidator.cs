using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation
{
    public class ContactUsModelValidator : AbstractValidator<ContactUsModel>
    {
        public const string FullNameMissingErrorMessage = "Enter a full name";
        public const string MessageMissingErrorMessage = "Enter your message for us";
        public const string EmailAddressMissingErrorMessage = "Enter an email address";
        public const string EmailAddressWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string ContactMethodMissingErrorMessage = "Select your reason for contacting us";
        public const string PrivacyPolicyNotAcceptedErrorMessage = "Confirm you have read and understood the privacy policy";

        public ContactUsModelValidator()
        {
            RuleFor(m => m.FullName)
                .NotEmpty()
                .WithMessage(FullNameMissingErrorMessage);

            RuleFor(m => m.Message)
                .NotEmpty()
                .WithMessage(MessageMissingErrorMessage);

            RuleFor(m => m.EmailAddress)
                .NotEmpty()
                .WithMessage(EmailAddressMissingErrorMessage)
                .EmailAddress()
                .WithMessage(EmailAddressWrongFormatErrorMessage);

            RuleFor(m => m.PrivacyPolicyAccepted)
                .Equal(true)
                .WithMessage(PrivacyPolicyNotAcceptedErrorMessage);
        }
    }
}
