using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates
{
    public static class RecipientDateModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_DateBeforeCommencementDate_ThrowsValidationError(
            RecipientDateModel model,
            RecipientDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(1);

            model.CommencementDate = date.AddDays(1);
            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);

            var errorMessage = string.Format(
                RecipientDateModelValidator.DeliveryDateBeforeCommencementDateErrorMessage,
                model.Description,
                $"{model.CommencementDate:d MMMM yyyy}");

            result.ShouldHaveValidationErrorFor(x => x.Day).WithErrorMessage(errorMessage);
        }

        [Theory]
        [MockInlineAutoData(1)]
        [MockInlineAutoData(10)]
        [MockInlineAutoData(100)]
        public static void Validate_ValidDate_NoErrors(
            int daysToAdd,
            RecipientDateModel model,
            RecipientDateModelValidator validator)
        {
            var date = model.CommencementDate.AddDays(daysToAdd);

            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
