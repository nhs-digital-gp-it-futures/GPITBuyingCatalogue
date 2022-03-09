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
