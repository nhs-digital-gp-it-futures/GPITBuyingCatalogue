using FluentAssertions;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.SolutionSelection.Quantity
{
    public static class ServiceRecipientQuantityModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_InputQuantityNotEntered_ThrowsValidationError(
            string inputQuantity,
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = inputQuantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueNotEnteredErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("zero")]
        [CommonInlineAutoData("£1")]
        public static void Validate_InputQuantityNotNumeric_ThrowsValidationError(
            string inputQuantity,
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = inputQuantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueNotNumericErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("0.1")]
        [CommonInlineAutoData("1.1")]
        [CommonInlineAutoData("1.0001")]
        public static void Validate_InputQuantityNotAWholeNumber_ThrowsValidationError(
            string inputQuantity,
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = inputQuantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueNotAnIntegerErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_InputQuantityNegative_ThrowsValidationError(
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = "-1";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(ServiceRecipientQuantityModelValidator.ValueNegativeErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("1")]
        [CommonInlineAutoData("1234")]
        [CommonInlineAutoData("999999")]
        public static void Validate_InputQuantityValid_NoErrors(
            string inputQuantity,
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = inputQuantity;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
