using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Pricing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Pricing
{
    public static class PricingTierModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_AgreedPriceNotEntered_ThrowsValidationError(
            string agreedPrice,
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            model.AgreedPrice = agreedPrice;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.AgreedPrice)
                .WithErrorMessage(PricingTierModelValidator.PriceNotEnteredErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("zero")]
        [CommonInlineAutoData("£1")]
        public static void Validate_AgreedPriceNotNumeric_ThrowsValidationError(
            string agreedPrice,
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            model.AgreedPrice = agreedPrice;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.AgreedPrice)
                .WithErrorMessage(PricingTierModelValidator.PriceNotNumericErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceNegative_ThrowsValidationError(
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            var price = model.ListPrice * -1;

            model.AgreedPrice = $"{price:#,##0.00##}";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.AgreedPrice)
                .WithErrorMessage(PricingTierModelValidator.PriceNegativeErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("1.00001")]
        [CommonInlineAutoData("1.000001")]
        [CommonInlineAutoData("1.0000001")]
        public static void Validate_AgreedPriceContainsMoreThanFourDecimalPlaces_ThrowsValidationError(
            string agreedPrice,
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            model.AgreedPrice = agreedPrice;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.AgreedPrice)
                .WithErrorMessage(PricingTierModelValidator.PriceNotWithinFourDecimalPlacesErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceHigherThanListPrice_ThrowsValidationError(
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            var price = model.ListPrice + 1M;

            model.AgreedPrice = $"{price:#,##0.00##}";

            var result = validator.TestValidate(model);

            var errorMessage = $"{PricingTierModelValidator.PriceHigherThanListPriceErrorMessage} (£{model.ListPrice:#,##0.00##})";

            result.ShouldHaveValidationErrorFor(x => x.AgreedPrice)
                .WithErrorMessage(errorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceSameAsListPrice_NoErrors(
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            model.AgreedPrice = $"{model.ListPrice:#,##0.00##}";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceLowerThanListPrice_NoErrors(
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            model.ListPrice = 1M;
            model.AgreedPrice = "0.5";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceZero_NoErrors(
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            model.AgreedPrice = "0";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
