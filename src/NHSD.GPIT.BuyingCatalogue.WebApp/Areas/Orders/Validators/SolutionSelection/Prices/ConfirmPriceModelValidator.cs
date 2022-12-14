using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Prices
{
    public class ConfirmPriceModelValidator : AbstractValidator<ConfirmPriceModel>
    {
        public ConfirmPriceModelValidator()
        {
            RuleForEach(x => x.Tiers).SetValidator(new PricingTierModelValidator());
        }
    }
}
