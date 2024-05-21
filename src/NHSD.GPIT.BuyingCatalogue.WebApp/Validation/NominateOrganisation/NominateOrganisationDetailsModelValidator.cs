using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.NominateOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.NominateOrganisation
{
    public class NominateOrganisationDetailsModelValidator : AbstractValidator<NominateOrganisationDetailsModel>
    {
        public const string OrganisationNameErrorMessage = "Enter an organisation name";

        public NominateOrganisationDetailsModelValidator()
        {
            RuleFor(x => x.OrganisationName)
                .NotEmpty()
                .WithMessage(OrganisationNameErrorMessage);
        }
    }
}
