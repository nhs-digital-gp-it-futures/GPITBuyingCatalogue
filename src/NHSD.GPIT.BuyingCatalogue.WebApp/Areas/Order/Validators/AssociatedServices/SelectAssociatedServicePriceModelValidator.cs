using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices
{
    public sealed class SelectAssociatedServicePriceModelValidator : AbstractValidator<SelectAssociatedServicePriceModel>
    {
        public SelectAssociatedServicePriceModelValidator()
        {
            RuleFor(m => m.SelectedPrice)
                .NotNull()
                .WithMessage("Select a price");
        }
    }
}
