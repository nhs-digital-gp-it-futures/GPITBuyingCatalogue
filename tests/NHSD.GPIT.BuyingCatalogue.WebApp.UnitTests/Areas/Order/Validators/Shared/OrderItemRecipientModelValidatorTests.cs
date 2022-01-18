using System;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Shared
{
    public static class OrderItemRecipientModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_DeliveryDateNull_ThrowsValidationError(
            OrderItemRecipientModel model,
            OrderItemRecipientModelValidator validator)
        {
            model.Day = model.Month = model.Year = null;
            model.DeliveryDate = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage("Planned delivery date must be a real date");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DeliveryDateInPast_ThrowsValidationError(
            OrderItemRecipientModel model,
            OrderItemRecipientModelValidator validator)
        {
            var pastDate = DateTime.UtcNow.AddMonths(-5);

            model.Day = pastDate.Day.ToString();
            model.Month = pastDate.Month.ToString();
            model.Year = pastDate.Year.ToString();

            model.DeliveryDate = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage("Planned delivery date must be in the future");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DeliveryDateExceedsCommencementDate_ThrowsValidationError(
            [Frozen] DateTime? commencementDate,
            OrderItemRecipientModel model,
            OrderItemRecipientModelValidator validator)
        {
            var beyondCommencementDate = commencementDate.Value.AddMonths(43);

            model.Day = beyondCommencementDate.Day.ToString();
            model.Month = beyondCommencementDate.Month.ToString();
            model.Year = beyondCommencementDate.Year.ToString();

            model.DeliveryDate = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage($"Planned delivery date must be within {ValidationConstants.MaxDeliveryMonthsFromCommencement} months from the commencement date for this Call-off Agreement");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_QuantityNull_ThrowsValidationError(
            OrderItemRecipientModel model,
            OrderItemRecipientModelValidator validator)
        {
            model.Quantity = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Enter a quantity");
        }

        [Theory]
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(0)]
        public static void Validate_QuantityLessThanOrEqualTo0_ThrowsValidationError(
            int quantity,
            OrderItemRecipientModel model,
            OrderItemRecipientModelValidator validator)
        {
            model.Quantity = quantity;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Quantity)
                .WithErrorMessage("Quantity must be greater than 0");
        }
    }
}
