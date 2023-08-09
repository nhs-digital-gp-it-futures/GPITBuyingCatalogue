using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Quantity
{
    public static class SelectOrderItemQuantityModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [CommonInlineAutoData("zero")]
        [CommonInlineAutoData("£1")]
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
        [CommonInlineAutoData("0.1")]
        [CommonInlineAutoData("1.1")]
        [CommonInlineAutoData("1.0001")]
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
        [CommonAutoData]
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
        [CommonInlineAutoData("1")]
        [CommonInlineAutoData("1234")]
        [CommonInlineAutoData("999999")]
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
