using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class EditTierPriceModelValidator : AbstractValidator<EditTierPriceModel>
    {
        public EditTierPriceModelValidator()
        {
            RuleFor(m => m.Price)
                .IsValidPrice();
        }
    }
}
