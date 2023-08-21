using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Pricing
{
    public class ConfirmPriceModelValidator : AbstractValidator<ConfirmPriceModel>
    {
        public ConfirmPriceModelValidator()
        {
            RuleForEach(x => x.Tiers).SetValidator(new PricingTierModelValidator());
        }
    }
}
