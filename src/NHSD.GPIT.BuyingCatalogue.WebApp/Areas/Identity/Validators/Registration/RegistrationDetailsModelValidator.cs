using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration
{
    public class RegistrationDetailsModelValidator : AbstractValidator<RegistrationDetailsModel>
    {
        public const string EmailAddressMissingErrorMessage = "Enter your email address";
        public const string EmailAddressWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string FirstNameErrorMessage = "Enter your first name";
        public const string LastNameErrorMessage = "Enter your last name";
        public const string OdsCodeErrorMessage = "Enter your organisation's ODS code";
        public const string JustificationErrorMessage = "Enter a reason for your account request";
        public const string PrivacyPolicyErrorMessage = "Confirm you have read and understood our privacy policy";

        public RegistrationDetailsModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameErrorMessage);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(LastNameErrorMessage);

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage(EmailAddressMissingErrorMessage)
                .EmailAddress()
                .WithMessage(EmailAddressWrongFormatErrorMessage);

            RuleFor(x => x.OdsCode)
                .NotEmpty()
                .WithMessage(OdsCodeErrorMessage);

            RuleFor(x => x.Justification)
                .NotEmpty()
                .WithMessage(JustificationErrorMessage);

            RuleFor(x => x.HasReadPrivacyPolicy)
                .NotEqual(false)
                .WithMessage(PrivacyPolicyErrorMessage);
        }
    }
}
