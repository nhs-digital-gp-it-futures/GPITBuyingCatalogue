using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Quantity
{
    public static class SelectOrderItemQuantityModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_QuantityNotEntered_ThrowsValidationError(
            string quantity,
            SelectOrderItemQuantityModel model,
            SelectOrderItemQuantityModelValidator validator)
        {
            model.Quantity = quantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                .WithErrorMessage(SelectOrderItemQuantityModelValidator.QuantityNotEnteredErrorMessage);
        }

        [Theory]
        [MockInlineAutoData("zero")]
        [MockInlineAutoData("£1")]
        public static void Validate_QuantityNotNumeric_ThrowsValidationError(
            string quantity,
            SelectOrderItemQuantityModel model,
            SelectOrderItemQuantityModelValidator validator)
        {
            model.Quantity = quantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                .WithErrorMessage(SelectOrderItemQuantityModelValidator.QuantityNotANumberErrorMessage);
        }

        [Theory]
        [MockInlineAutoData("0.1")]
        [MockInlineAutoData("1.1")]
        [MockInlineAutoData("1.0001")]
        public static void Validate_QuantityNotAWholeNumber_ThrowsValidationError(
            string quantity,
            SelectOrderItemQuantityModel model,
            SelectOrderItemQuantityModelValidator validator)
        {
            model.Quantity = quantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                .WithErrorMessage(SelectOrderItemQuantityModelValidator.QuantityNotAWholeNumberErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_QuantityNegative_ThrowsValidationError(
            SelectOrderItemQuantityModel model,
            SelectOrderItemQuantityModelValidator validator)
        {
            model.Quantity = "-1";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                .WithErrorMessage(SelectOrderItemQuantityModelValidator.QuantityNegativeErrorMessage);
        }

        [Theory]
        [MockInlineAutoData("1")]
        [MockInlineAutoData("1234")]
        [MockInlineAutoData("999999")]
        public static void Validate_QuantityValid_NoErrors(
            string quantity,
            SelectOrderItemQuantityModel model,
            SelectOrderItemQuantityModelValidator validator)
        {
            model.Quantity = quantity;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
