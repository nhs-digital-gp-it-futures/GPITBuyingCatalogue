using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AssociatedServices
{
    public static class EditAssociatedServiceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_QuantityNull_ThrowsValidationError(
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            model.OrderItem.ServiceRecipients[0].Quantity = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("OrderItem.ServiceRecipients[0].Quantity")
                .WithErrorMessage("Enter a quantity");
        }

        [Theory]
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(0)]
        public static void Validate_QuantityLessThanOrEqualTo0_ThrowsValidationError(
            int quantity,
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            model.OrderItem.ServiceRecipients[0].Quantity = quantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("OrderItem.ServiceRecipients[0].Quantity")
                .WithErrorMessage("Quantity must be greater than 0");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceNull_ThrowsValidationError(
               EditAssociatedServiceModel model,
               EditAssociatedServiceModelValidator validator)
        {
            model.OrderItem.AgreedPrice = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderItem.AgreedPrice)
                .WithErrorMessage("Enter an agreed price");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceNegative_ThrowsValidationError(
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            model.OrderItem.AgreedPrice = -1;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderItem.AgreedPrice)
                .WithErrorMessage("Price cannot be negative");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_AgreedPriceExceedsListPrice_ThrowsValidationError(
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            model.OrderItem.AgreedPrice = 10;
            model.OrderItem.CataloguePrice.Price = 5;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.OrderItem.AgreedPrice)
                .WithErrorMessage("Price cannot be greater than list price");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidAgreedPrice_NoValidationError(
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            model.OrderItem.AgreedPrice = 5;
            model.OrderItem.CataloguePrice.Price = 10;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.OrderItem.AgreedPrice);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_OnDemandNullEstimationPeriod_ThrowsValidationError(
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            model.EstimationPeriod = null;
            model.OrderItem.CataloguePrice.ProvisioningType = ProvisioningType.OnDemand;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.EstimationPeriod)
                .WithErrorMessage("Time unit is required");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_OnDemand_NoValidationError(
            EditAssociatedServiceModel model,
            EditAssociatedServiceModelValidator validator)
        {
            model.EstimationPeriod = TimeUnit.PerMonth;
            model.OrderItem.CataloguePrice.ProvisioningType = ProvisioningType.OnDemand;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(m => m.EstimationPeriod);
        }
    }
}
