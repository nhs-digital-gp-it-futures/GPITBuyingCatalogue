using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates
{
    public static class RecipientDateModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(0)]
        [MockInlineAutoData(-1)]
        [MockInlineAutoData(-10)]
        public static void Validate_PresentOrPastDate_ThrowsValidationError(
            int daysToAdd,
            RecipientDateModel model,
            RecipientDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(daysToAdd);

            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);
            var errorMessage = string.Format(
                RecipientDateModelValidator.DeliveryDateInThePastErrorMessage,
                model.Description);

            result.ShouldHaveValidationErrorFor(x => x.Day)
                .WithErrorMessage(errorMessage);
        }

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
        public static void Validate_FutureDate_NoErrors(
            int daysToAdd,
            RecipientDateModel model,
            RecipientDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(daysToAdd);

            model.CommencementDate = date.AddDays(-1);
            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
