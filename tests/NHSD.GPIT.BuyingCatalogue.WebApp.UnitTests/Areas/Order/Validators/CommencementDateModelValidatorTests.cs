using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class CommencementDateModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_NullCommencementDate_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.Day = model.Month = model.Year = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage("Commencement date must be a real date");
        }

        [Theory]
        [CommonInlineAutoData("", "2", "2022")]
        [CommonInlineAutoData("2", "", "2022")]
        [CommonInlineAutoData("2", "2", "")]
        public static void Validate_InvalidCommencementDate_ThrowsValidationError(
            string day,
            string month,
            string year,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.Day = day;
            model.Month = month;
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage("Commencement date must be a real date");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_CommencementBeyond60DaysDateInPast_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            var invalidDate = DateTime.UtcNow.AddDays(-61);

            model.Day = invalidDate.Day.ToString();
            model.Month = invalidDate.Month.ToString();
            model.Year = invalidDate.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage("Commencement date must be in the future or within the last 60 days");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_CommencementDate20DaysInPast_NoValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            var invalidDate = DateTime.UtcNow.AddDays(-20);

            model.Day = invalidDate.Day.ToString();
            model.Month = invalidDate.Month.ToString();
            model.Year = invalidDate.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidModel_NoValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            var invalidDate = DateTime.UtcNow.AddDays(20);

            model.Day = invalidDate.Day.ToString();
            model.Month = invalidDate.Month.ToString();
            model.Year = invalidDate.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
