using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models.Registration;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Validators.Registration
{
    public class RegistrationDetailsModelValidator : AbstractValidator<RegistrationDetailsModel>
    {
        public const string EmailAddressMissingErrorMessage = "Enter your email address";
        public const string EmailAddressWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string FullNameErrorMessage = "Enter your full name";
        public const string OrganisationNameErrorMessage = "Enter the name of your organisation";
        public const string PrivacyPolicyErrorMessage = "Confirm you have read and understood our privacy policy";

        public RegistrationDetailsModelValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(FullNameErrorMessage);

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage(EmailAddressMissingErrorMessage)
                .EmailAddress()
                .WithMessage(EmailAddressWrongFormatErrorMessage);

            RuleFor(x => x.OrganisationName)
                .NotEmpty()
                .WithMessage(OrganisationNameErrorMessage);

            RuleFor(x => x.HasReadPrivacyPolicy)
                .NotEqual(false)
                .WithMessage(PrivacyPolicyErrorMessage);
        }
    }
}
