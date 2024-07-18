using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Pricing;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Pricing
{
    public static class PricingTierModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockInlineAutoData("zero")]
        [MockInlineAutoData("£1")]
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
        [MockAutoData]
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
        [MockInlineAutoData("1.00001")]
        [MockInlineAutoData("1.000001")]
        [MockInlineAutoData("1.0000001")]
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
        [MockAutoData]
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
        [MockAutoData]
        public static void Validate_AgreedPriceSameAsListPrice_NoErrors(
            PricingTierModel model,
            PricingTierModelValidator validator)
        {
            model.AgreedPrice = $"{model.ListPrice:#,##0.00##}";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
