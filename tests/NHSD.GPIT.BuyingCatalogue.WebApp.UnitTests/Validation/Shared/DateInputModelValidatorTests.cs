using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class DateInputModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_DayMissing_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(DateInputModelValidator.DayMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_MonthMissing_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = "01";
            model.Month = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Month)
                .WithErrorMessage(DateInputModelValidator.MonthMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_YearMissing_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(DateInputModelValidator.YearMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("1")]
        [CommonInlineAutoData("12")]
        [CommonInlineAutoData("123")]
        public static void Validate_YearTooShort_ThrowsValidationError(
            string year,
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(DateInputModelValidator.YearWrongLengthErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("99", "01", "2022")]
        [CommonInlineAutoData("01", "99", "2022")]
        public static void Validate_InvalidDate_ThrowsValidationError(
            string day,
            string month,
            string year,
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = day;
            model.Month = month;
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(DateInputModelValidator.DateInvalidErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidModel_NoValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
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
