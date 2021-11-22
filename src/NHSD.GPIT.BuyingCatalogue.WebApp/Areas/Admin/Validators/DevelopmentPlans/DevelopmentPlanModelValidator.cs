using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DevelopmentPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.DevelopmentPlans
{
    public sealed class DevelopmentPlanModelValidator : AbstractValidator<DevelopmentPlanModel>
    {
        public DevelopmentPlanModelValidator(
            IUrlValidator urlValidator)
        {
            RuleFor(m => m.Link)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.Link));
        }
    }
}
