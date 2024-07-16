using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Quantity;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Quantity
{
    public static class ServiceRecipientQuantityModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_InputQuantityNotEntered_ThrowsValidationError(
            string inputQuantity,
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = inputQuantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(string.Format(ServiceRecipientQuantityModelValidator.ValueNotEnteredErrorMessage, model.Name));
        }

        [Theory]
        [MockInlineAutoData("zero")]
        [MockInlineAutoData("£1")]
        public static void Validate_InputQuantityNotNumeric_ThrowsValidationError(
            string inputQuantity,
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = inputQuantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(string.Format(ServiceRecipientQuantityModelValidator.ValueNotNumericErrorMessage, model.Name));
        }

        [Theory]
        [MockInlineAutoData("0.1")]
        [MockInlineAutoData("1.1")]
        [MockInlineAutoData("1.0001")]
        public static void Validate_InputQuantityNotAWholeNumber_ThrowsValidationError(
            string inputQuantity,
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = inputQuantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(string.Format(ServiceRecipientQuantityModelValidator.ValueNotAnIntegerErrorMessage, model.Name));
        }

        [Theory]
        [MockAutoData]
        public static void Validate_InputQuantityNegative_ThrowsValidationError(
            ServiceRecipientQuantityModel model,
            ServiceRecipientQuantityModelValidator validator)
        {
            model.InputQuantity = "-1";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.InputQuantity)
                .WithErrorMessage(string.Format(ServiceRecipientQuantityModelValidator.ValueNegativeErrorMessage, model.Name));
        }

        [Theory]
        [MockInlineAutoData("1")]
        [MockInlineAutoData("1234")]
        [MockInlineAutoData("999999")]
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
