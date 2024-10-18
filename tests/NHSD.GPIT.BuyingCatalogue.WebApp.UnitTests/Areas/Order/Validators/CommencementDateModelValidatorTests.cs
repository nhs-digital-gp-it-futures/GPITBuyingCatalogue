using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.CommencementDate;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class CommencementDateModelValidatorTests
    {
        [Theory]
        [MockInlineAutoData(-93)]
        [MockInlineAutoData(-150)]
        [MockInlineAutoData(-200)]
        public static void Validate_CommencementDateOutsideThreshold_ThrowsValidationError(
            int days,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(days);

            model.Day = date.Day.ToString();
            model.Month = date.Month.ToString();
            model.Year = date.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(CommencementDateModelValidator.CommencementDateInThePastErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(-59)]
        [MockInlineAutoData(-20)]
        [MockInlineAutoData(-1)]
        [MockInlineAutoData(0)]
        [MockInlineAutoData(1)]
        [MockInlineAutoData(20)]
        [MockInlineAutoData(60)]
        [MockInlineAutoData(180)]
        public static void Validate_CommencementDateWithinThreshold_NoValidationError(
            int days,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(days);

            model.Day = date.Day.ToString();
            model.Month = date.Month.ToString();
            model.Year = date.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.Day);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_InitialPeriodNotEntered_ThrowsValidationError(
            string initialPeriod,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.InitialPeriod = initialPeriod;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InitialPeriod)
                .WithErrorMessage(CommencementDateModelValidator.InitialPeriodMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData("1 month")]
        [MockInlineAutoData("one")]
        [MockInlineAutoData("abcdef")]
        public static void Validate_InitialPeriodNotANumber_ThrowsValidationError(
            string initialPeriod,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.InitialPeriod = initialPeriod;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InitialPeriod)
                .WithErrorMessage(CommencementDateModelValidator.InitialPeriodNotANumberErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(-1)]
        [MockInlineAutoData(0)]
        public static void Validate_InitialPeriodTooLow_ThrowsValidationError(
            string initialPeriod,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.InitialPeriod = initialPeriod;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InitialPeriod)
                .WithErrorMessage(CommencementDateModelValidator.InitialPeriodTooLowErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void Validate_MaximumTermNotEntered_ThrowsValidationError(
            string maximumTerm,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.MaximumTerm = maximumTerm;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.MaximumTerm)
                .WithErrorMessage(CommencementDateModelValidator.MaximumTermMissingErrorMessage);
        }

        [Theory]
        [MockInlineAutoData("1 month")]
        [MockInlineAutoData("one")]
        [MockInlineAutoData("abcdef")]
        public static void Validate_MaximumTermNotANumber_ThrowsValidationError(
            string maximumTerm,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.MaximumTerm = maximumTerm;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.MaximumTerm)
                .WithErrorMessage(CommencementDateModelValidator.MaximumTermNotANumberErrorMessage);
        }

        [Theory]
        [MockInlineAutoData(-1)]
        [MockInlineAutoData(0)]
        public static void Validate_MaximumTermTooLow_ThrowsValidationError(
            string maximumTerm,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.MaximumTerm = maximumTerm;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.MaximumTerm)
                .WithErrorMessage(CommencementDateModelValidator.MaximumTermTooLowErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_MaximumTermTooHigh_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.MaxumimTermUpperLimit = 36;
            model.MaximumTerm = "37";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.MaximumTerm)
                .WithErrorMessage(string.Format(CommencementDateModelValidator.MaximumTermTooHighErrorMessage, "36"));
        }

        [Theory]
        [MockInlineAutoData(null, 1)]
        [MockInlineAutoData(1, 1)]
        [MockInlineAutoData(2, 1)]
        [MockInlineAutoData(2, 2)]
        public static void Validate_InvalidMaximumTerm_ThrowsValidationError(
            string initialPeriod,
            string maximumTerm,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.InitialPeriod = initialPeriod;
            model.MaximumTerm = maximumTerm;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.MaximumTerm)
                .WithErrorMessage(CommencementDateModelValidator.DurationInvalidErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ValidModel_NoValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            var validDate = DateTime.UtcNow.AddDays(20).Date;
            model.MaxumimTermUpperLimit = 24;

            model.Day = validDate.Day.ToString();
            model.Month = validDate.Month.ToString();
            model.Year = validDate.Year.ToString();
            model.InitialPeriod = "3";
            model.MaximumTerm = "12";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
