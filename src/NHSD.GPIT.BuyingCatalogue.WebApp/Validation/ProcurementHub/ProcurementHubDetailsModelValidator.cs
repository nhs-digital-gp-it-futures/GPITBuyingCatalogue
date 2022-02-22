using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.ProcurementHub;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.ProcurementHub
{
    public class ProcurementHubDetailsModelValidator : AbstractValidator<ProcurementHubDetailsModel>
    {
        public const string FullNameMissingErrorMessage = "Enter your full name";
        public const string EmailAddressMissingErrorMessage = "Enter your email address";
        public const string EmailAddressWrongFormatErrorMessage = "Enter an email address in the correct format, like name@example.com";
        public const string OrganisationNameMissingErrorMessage = "Enter the name of your organisation";
        public const string QueryMissingErrorMessage = "Enter a message";
        public const string PrivacyPolicyErrorMessage = "Confirm you have read and understood our privacy policy";

        public ProcurementHubDetailsModelValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(FullNameMissingErrorMessage);

            RuleFor(x => x.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(EmailAddressMissingErrorMessage)
                .EmailAddress()
                .WithMessage(EmailAddressWrongFormatErrorMessage);

            RuleFor(x => x.OrganisationName)
                .NotEmpty()
                .WithMessage(OrganisationNameMissingErrorMessage);

            RuleFor(x => x.Query)
                .NotEmpty()
                .WithMessage(QueryMissingErrorMessage);

            RuleFor(x => x.HasReadPrivacyPolicy)
                .NotEqual(false)
                .WithMessage(PrivacyPolicyErrorMessage);
        }
    }
}
