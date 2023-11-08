using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
        [CommonInlineAutoData(OrderTriageValue.Under40K)]
        [CommonInlineAutoData(OrderTriageValue.Between40KTo250K)]
        [CommonInlineAutoData(OrderTriageValue.Over250K)]
        public static void Validate_DateAfterContractEndDate_ThrowsValidationError(
            OrderTriageValue triageValue,
            SelectDateModel model,
            SelectDateModelValidator validator)
        {
            model.CommencementDate = DateTime.UtcNow.AddDays(-1).Date;
            model.MaximumTerm = 1;
            model.TriageValue = triageValue;
            model.IsAmend = false;

            var contractEndDate = new EndDate(model.CommencementDate, model.MaximumTerm).DateTime.Value;
            var invalidDate = contractEndDate
                .AddMonths(model.MaximumTerm.Value);

            model.Day = $"{invalidDate.Day}";
            model.Month = $"{invalidDate.Month}";
            model.Year = $"{invalidDate.Year}";

            var result = validator.TestValidate(model);

            var errorMessage = string.Format(
                SelectDateModelValidator.DeliveryDateAfterContractEndDateErrorMessage,
                $"{contractEndDate:d MMMM yyyy}");

            result.ShouldHaveValidationErrorFor(x => x.Day).WithErrorMessage(errorMessage);

            model.Day = $"{contractEndDate.Day}";
            model.Month = $"{contractEndDate.Month}";
            model.Year = $"{contractEndDate.Year}";

            result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonInlineAutoData(OrderTriageValue.Under40K)]
        [CommonInlineAutoData(OrderTriageValue.Between40KTo250K)]
        [CommonInlineAutoData(OrderTriageValue.Over250K)]
        public static void Validate_DateAfterContractEndDate_Amend_ThrowsValidationError(
            OrderTriageValue triageValue,
            SelectDateModel model,
            SelectDateModelValidator validator)
        {
            model.CommencementDate = DateTime.UtcNow.AddDays(-1).Date;
            model.MaximumTerm = 1;
            model.TriageValue = triageValue;
            model.IsAmend = true;

            var contractEndDate = new EndDate(model.CommencementDate, model.MaximumTerm).DateTime.Value;
            var invalidDate = contractEndDate
                .AddMonths(model.MaximumTerm.Value);

            model.Day = $"{invalidDate.Day}";
            model.Month = $"{invalidDate.Month}";
            model.Year = $"{invalidDate.Year}";

            var result = validator.TestValidate(model);

            var errorMessage = string.Format(
                SelectDateModelValidator.AmendDeliveryDateAfterContractEndDateErrorMessage,
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
