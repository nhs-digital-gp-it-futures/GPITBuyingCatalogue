using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Organisation
{
    public class AddNominatedOrganisationModelValidator : AbstractValidator<AddNominatedOrganisationModel>
    {
        public const string OrganisationMissingError = "Select an organisation name";

        public AddNominatedOrganisationModelValidator()
        {
            RuleFor(x => x.SelectedOrganisationId)
                .NotEmpty()
                .WithMessage(OrganisationMissingError);
        }
    }
}
