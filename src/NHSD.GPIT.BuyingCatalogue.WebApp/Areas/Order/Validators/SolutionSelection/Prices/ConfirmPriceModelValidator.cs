using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Prices
{
    public class ConfirmPriceModelValidator : AbstractValidator<ConfirmPriceModel>
    {
        public ConfirmPriceModelValidator()
        {
            RuleForEach(x => x.Tiers).SetValidator(new PricingTierModelValidator());
        }
    }
}
