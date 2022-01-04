using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Organisation
{
    public class FindOrganisationModelValidator : AbstractValidator<FindOrganisationModel>
    {
        public FindOrganisationModelValidator()
        {
            RuleFor(m => m.OdsCode)
                .NotNull()
                .WithMessage("Enter ODS code");
        }
    }
}
