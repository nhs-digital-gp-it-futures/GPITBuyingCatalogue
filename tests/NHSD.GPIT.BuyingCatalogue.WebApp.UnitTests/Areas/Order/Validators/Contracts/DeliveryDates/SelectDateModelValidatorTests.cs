using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates
{
    public static class SelectDateModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(0)]
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(-10)]
        public static void Validate_PresentOrPastDate_ThrowsValidationError(
            int daysToAdd,
            SelectDateModel model,
            SelectDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(daysToAdd);

            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Day)
                .WithErrorMessage(SelectDateModelValidator.DeliveryDateInThePastErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DateBeforeCommencementDate_ThrowsValidationError(
            SelectDateModel model,
            SelectDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(1);

            model.CommencementDate = date.AddDays(1);
            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);

            var errorMessage = string.Format(
                SelectDateModelValidator.DeliveryDateBeforeCommencementDateErrorMessage,
                $"{model.CommencementDate:d MMMM yyyy}");

            result.ShouldHaveValidationErrorFor(x => x.Day).WithErrorMessage(errorMessage);
        }

        [Theory]
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(10)]
        [CommonInlineAutoData(100)]
        public static void Validate_FutureDate_NoErrors(
            int daysToAdd,
            SelectDateModel model,
            SelectDateModelValidator validator)
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
