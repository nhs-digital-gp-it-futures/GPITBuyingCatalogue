using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Validation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class InteroperabilityModelValidator : AbstractValidator<InteroperabilityModel>
    {
        public InteroperabilityModelValidator(
            IUrlValidator urlValidator)
        {
            RuleFor(m => m.Link)
                .IsValidUrl(urlValidator)
                .Unless(m => string.IsNullOrWhiteSpace(m.Link));
        }
    }
}
