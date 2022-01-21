using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices
{
    public sealed class SelectAdditionalServiceModelValidator : AbstractValidator<SelectAdditionalServiceModel>
    {
        public SelectAdditionalServiceModelValidator()
        {
            RuleFor(m => m.SelectedAdditionalServiceId)
                .NotNull()
                .WithMessage("Select an Additional Service");
        }
    }
}
