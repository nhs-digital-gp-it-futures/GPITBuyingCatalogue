using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices
{
    public sealed class SelectAdditionalServicePriceModelValidator : AbstractValidator<SelectAdditionalServicePriceModel>
    {
        public SelectAdditionalServicePriceModelValidator()
        {
            RuleFor(m => m.SelectedPrice)
                .NotNull()
                .WithMessage("Select a price");
        }
    }
}
