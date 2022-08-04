using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Prices
{
    public static class ConfirmPriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_PriceNotEntered_ThrowsValidationError(
            ConfirmPriceModel model,
            ConfirmPriceModelValidator validator)
        {
            var tier = model.Tiers.FirstOrDefault();
            tier.AgreedPrice = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Tiers[0].AgreedPrice")
                .WithErrorMessage(PricingTierModelValidator.PriceNotEnteredErrorMessage);

            for (var i = 1; i < model.Tiers.Count(); i++)
            {
                result.ShouldNotHaveValidationErrorFor($"Tiers[{i}].AgreedPrice");
            }
        }
    }
}
