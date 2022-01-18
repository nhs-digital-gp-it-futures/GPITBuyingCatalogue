using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.NominateOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.NominateOrganisation
{
    public class NominateOrganisationDetailsModelValidator : AbstractValidator<NominateOrganisationDetailsModel>
    {
        public const string OrganisationNameErrorMessage = "Enter the name of the organisation you’re nominating";
        public const string HasReadPrivacyPolicyErrorMessage = "Confirm you have read and understood our privacy policy";

        public NominateOrganisationDetailsModelValidator()
        {
            RuleFor(x => x.OrganisationName)
                .NotEmpty()
                .WithMessage(OrganisationNameErrorMessage);

            RuleFor(x => x.HasReadPrivacyPolicy)
                .NotEqual(false)
                .WithMessage(HasReadPrivacyPolicyErrorMessage);
        }
    }
}
