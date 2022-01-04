using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation
{
    public class AddAnOrganisationModelValidator : AbstractValidator<AddAnOrganisationModel>
    {
        public AddAnOrganisationModelValidator()
        {
            RuleFor(m => m.SelectedOrganisation)
                .NotNull()
                .WithMessage("Select a related organisation");
        }
    }
}
