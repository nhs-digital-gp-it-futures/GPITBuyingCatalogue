using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Shared
{
    public static class CreateOrderItemModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceNull_ThrowsValidationError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.AgreedPrice = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AgreedPrice)
                .WithErrorMessage("Enter an agreed price");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceNegative_ThrowsValidationError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.AgreedPrice = -1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AgreedPrice)
                .WithErrorMessage("Price cannot be negative");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceExceedsListPrice_ThrowsValidationError(
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.AgreedPrice = 10;
            model.CataloguePrice.Price = 5;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AgreedPrice)
                .WithErrorMessage("Price cannot be greater than list price");
        }

        [Theory]
        [CommonInlineAutoData(0)]
        [CommonInlineAutoData(5)]
        public static void Validate_ValidAgreedPrice_NoValidationError(
            int agreedPrice,
            CreateOrderItemModel model,
            CreateOrderItemModelValidator validator)
        {
            model.AgreedPrice = agreedPrice;
            model.CataloguePrice.Price = 10;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.AgreedPrice);
        }
    }
}
