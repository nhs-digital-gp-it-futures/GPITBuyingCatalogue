using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
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
            model.Tiers.ToList().ForEach(t => t.AgreedPrice = string.Empty);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Tiers[0].AgreedPrice")
                .WithErrorMessage(PricingTierModelValidator.PriceNotEnteredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PriceNotNumber_ThrowsValidationError(
            ConfirmPriceModel model,
            ConfirmPriceModelValidator validator)
        {
            model.Tiers.ToList().ForEach(t => t.AgreedPrice = "abc");

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Tiers[0].AgreedPrice")
                .WithErrorMessage(PricingTierModelValidator.PriceNotNumericErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PriceNegativeNumber_ThrowsValidationError(
            ConfirmPriceModel model,
            ConfirmPriceModelValidator validator)
        {
            model.Tiers.ToList().ForEach(t => t.AgreedPrice = "-1");

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Tiers[0].AgreedPrice")
                .WithErrorMessage(PricingTierModelValidator.PriceNegativeErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_PriceHasMoreThanFourDecimalPlaces_ThrowsValidationError(
            ConfirmPriceModel model,
            ConfirmPriceModelValidator validator)
        {
            model.Tiers.ToList().ForEach(t => t.AgreedPrice = "1.00001");

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Tiers[0].AgreedPrice")
                .WithErrorMessage(PricingTierModelValidator.PriceNotWithinFourDecimalPlacesErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(1000)]
        [CommonInlineAutoData(100000)]
        [CommonInlineAutoData(99999.9999)]
        [CommonInlineAutoData(int.MaxValue)]
        public static void Validate_PriceHigherThanListPrice_ThrowsValidationError(
            string agreedPrice,
            ConfirmPriceModel model,
            ConfirmPriceModelValidator validator)
        {
            model.Tiers.ToList().ForEach(t => t.ListPrice = 1.2M);
            model.Tiers.ToList().ForEach(t => t.AgreedPrice = agreedPrice);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("Tiers[0].AgreedPrice")
                .WithErrorMessage($"{PricingTierModelValidator.PriceHigherThanListPriceErrorMessage} (£{model.Tiers[0].ListPrice:#,##0.00##})");
        }
    }
}
