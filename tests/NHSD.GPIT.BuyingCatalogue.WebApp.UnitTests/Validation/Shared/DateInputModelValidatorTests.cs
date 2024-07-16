using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Validation.Shared
{
    public static class DateInputModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_DayMissing_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Description = null;
            model.Day = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(DateInputModelValidator.DayMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_DayMissing_WithDescription_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = string.Empty;

            var result = validator.TestValidate(model);
            var message = string.Format(DateInputModelValidator.DayMissingWithDescriptionErrorMessage, model.Description);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(message);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MonthMissing_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Description = null;
            model.Day = "01";
            model.Month = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Month)
                .WithErrorMessage(DateInputModelValidator.MonthMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MonthMissing_WithDescription_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = "01";
            model.Month = string.Empty;

            var result = validator.TestValidate(model);
            var message = string.Format(DateInputModelValidator.MonthMissingWithDescriptionErrorMessage, model.Description);

            result.ShouldHaveValidationErrorFor(m => m.Month)
                .WithErrorMessage(message);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_YearMissing_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Description = null;
            model.Day = "01";
            model.Month = "01";
            model.Year = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(DateInputModelValidator.YearMissingErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_YearMissing_WithDescription_ThrowsValidationError(
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = string.Empty;

            var result = validator.TestValidate(model);
            var message = string.Format(DateInputModelValidator.YearMissingWithDescriptionErrorMessage, model.Description);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(message);
        }

        [Theory]
        [MockInlineAutoData("1")]
        [MockInlineAutoData("12")]
        [MockInlineAutoData("123")]
        public static void Validate_YearTooShort_ThrowsValidationError(
            string year,
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Description = null;
            model.Day = "01";
            model.Month = "01";
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(DateInputModelValidator.YearWrongLengthErrorMessage);
        }

        [Theory]
        [MockInlineAutoData("1")]
        [MockInlineAutoData("12")]
        [MockInlineAutoData("123")]
        public static void Validate_YearTooShort_WithDescription_ThrowsValidationError(
            string year,
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = year;

            var result = validator.TestValidate(model);
            var message = string.Format(DateInputModelValidator.YearWrongLengthWithDescriptionErrorMessage, model.Description);

            result.ShouldHaveValidationErrorFor(m => m.Year)
                .WithErrorMessage(message);
        }

        [Theory]
        [MockInlineAutoData("99", "01", "2022")]
        [MockInlineAutoData("01", "99", "2022")]
        public static void Validate_InvalidDate_ThrowsValidationError(
            string day,
            string month,
            string year,
            DateInputModel model,
            DateInputModelValidator validator)
        {
            model.Description = null;
            model.Day = day;
            model.Month = month;
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(DateInputModelValidator.DateInvalidErrorMessage);
        }

        [Theory]
        [MockInlineAutoData("99", "01", "2022")]
        [MockInlineAutoData("01", "99", "2022")]
        public static void Validate_InvalidDate_WithDescription_ThrowsValidationError(
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
            var message = string.Format(DateInputModelValidator.DateInvalidWithDescriptionErrorMessage, model.Description);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(message);
        }

        [Theory]
        [MockAutoData]
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
