using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates
{
    public static class AmendDateModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(0)]
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(-10)]
        public static void Validate_PresentOrPastDate_ThrowsValidationError(
            int daysToAdd,
            AmendDateModel model,
            AmendDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(daysToAdd);

            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Day)
                .WithErrorMessage(AmendDateModelValidator.DeliveryDateInThePastErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DateBeforeCommencementDate_ThrowsValidationError(
            AmendDateModel model,
            AmendDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(1);

            model.CommencementDate = date.AddDays(1);
            model.Day = $"{date.Day}";
            model.Month = $"{date.Month}";
            model.Year = $"{date.Year}";

            var result = validator.TestValidate(model);

            var errorMessage = string.Format(
                AmendDateModelValidator.DeliveryDateBeforeCommencementDateErrorMessage,
                $"{model.CommencementDate:d MMMM yyyy}");

            result.ShouldHaveValidationErrorFor(x => x.Day).WithErrorMessage(errorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DateAfterContractEndDate_DirectAward_ThrowsValidationError(
            AmendDateModel model,
            AmendDateModelValidator validator)
        {
            model.CommencementDate = DateTime.UtcNow.AddDays(-1).Date;
            model.MaximumTerm = 1;
            model.TriageValue = OrderTriageValue.Under40K;

            var contractEndDate = model.CommencementDate.Value
                .AddMonths(model.MaximumTerm.Value)
                .AddDays(-1);

            var invalidDate = model.CommencementDate.Value
                .AddMonths(model.MaximumTerm.Value);

            model.Day = $"{invalidDate.Day}";
            model.Month = $"{invalidDate.Month}";
            model.Year = $"{invalidDate.Year}";

            var result = validator.TestValidate(model);

            var errorMessage = string.Format(
                AmendDateModelValidator.DeliveryDateAfterContractEndDateErrorMessage,
                $"{contractEndDate:d MMMM yyyy}");

            result.ShouldHaveValidationErrorFor(x => x.Day).WithErrorMessage(errorMessage);

            model.Day = $"{contractEndDate.Day}";
            model.Month = $"{contractEndDate.Month}";
            model.Year = $"{contractEndDate.Year}";

            result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonInlineAutoData(OrderTriageValue.Between40KTo250K)]
        [CommonInlineAutoData(OrderTriageValue.Over250K)]
        public static void Validate_DateAfterContractEndDate_CatalogueAward_ThrowsValidationError(
            OrderTriageValue triageValue,
            AmendDateModel model,
            AmendDateModelValidator validator)
        {
            model.CommencementDate = DateTime.UtcNow.AddDays(-1).Date;
            model.MaximumTerm = 1;
            model.TriageValue = triageValue;

            var contractEndDate = model.CommencementDate.Value
                .AddMonths(EndDate.MaximumTermForOnOffCatalogueOrders)
                .AddDays(-1);

            var invalidDate = model.CommencementDate.Value
                .AddMonths(EndDate.MaximumTermForOnOffCatalogueOrders);

            model.Day = $"{invalidDate.Day}";
            model.Month = $"{invalidDate.Month}";
            model.Year = $"{invalidDate.Year}";

            var result = validator.TestValidate(model);

            var errorMessage = string.Format(
                AmendDateModelValidator.DeliveryDateAfterContractEndDateErrorMessage,
                $"{contractEndDate:d MMMM yyyy}");

            result.ShouldHaveValidationErrorFor(x => x.Day).WithErrorMessage(errorMessage);

            model.Day = $"{contractEndDate.Day}";
            model.Month = $"{contractEndDate.Month}";
            model.Year = $"{contractEndDate.Year}";

            result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(10)]
        [CommonInlineAutoData(100)]
        public static void Validate_FutureDate_NoErrors(
            int daysToAdd,
            AmendDateModel model,
            AmendDateModelValidator validator)
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
