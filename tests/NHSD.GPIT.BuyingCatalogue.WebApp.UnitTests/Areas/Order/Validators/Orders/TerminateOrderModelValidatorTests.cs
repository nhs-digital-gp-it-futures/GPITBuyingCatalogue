using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.DeleteOrder;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Orders
{
    public static class TerminateOrderModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_DayMissing_ThrowsValidationError(
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            model.Day = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(TerminateOrderModelValidator.TerminationDateDayMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MonthMissing_ThrowsValidationError(
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            model.Day = "01";
            model.Month = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Month)
                .WithErrorMessage(TerminateOrderModelValidator.TerminationDateMonthMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_YearMissing_ThrowsValidationError(
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(TerminateOrderModelValidator.TerminationDateYearMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("1")]
        [CommonInlineAutoData("12")]
        [CommonInlineAutoData("123")]
        public static void Validate_YearTooShort_ThrowsValidationError(
            string year,
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(TerminateOrderModelValidator.TerminationDateYearTooShortErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("99", "01", "2022")]
        [CommonInlineAutoData("01", "99", "2022")]
        public static void Validate_InvalidDate_ThrowsValidationError(
            string day,
            string month,
            string year,
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            model.Day = day;
            model.Month = month;
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(TerminateOrderModelValidator.TerminationDateInvalidErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(0)]
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(-10)]
        public static void Validate_DateInPast_ThrowsValidationError(
            int days,
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            var date = DateTime.Now.AddDays(days);

            model.Day = date.Day.ToString();
            model.Month = date.Month.ToString();
            model.Year = date.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(TerminateOrderModelValidator.TerminationDateInThePastErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ReasonNull_SetsModelError(
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            model.Reason = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Reason)
                .WithErrorMessage(TerminateOrderModelValidator.ReasonEmptyErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Confirm_SetsModelError(
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            model.Confirm = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Confirm)
                .WithErrorMessage(TerminateOrderModelValidator.ConfirmErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidModel_NoValidationError(
            TerminateOrderModel model,
            TerminateOrderModelValidator validator)
        {
            var validDate = DateTime.UtcNow.AddDays(20).Date;

            model.Day = validDate.Day.ToString();
            model.Month = validDate.Month.ToString();
            model.Year = validDate.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
