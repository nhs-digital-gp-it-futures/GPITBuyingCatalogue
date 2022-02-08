﻿using System;
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
        public static void Validate_CommencementDateDayMissing_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.Day = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(CommencementDateModelValidator.CommencementDateDayMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_CommencementDateMonthMissing_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.Day = "01";
            model.Month = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(CommencementDateModelValidator.CommencementDateMonthMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_CommencementDateYearMissing_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(CommencementDateModelValidator.CommencementDateYearMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("1")]
        [CommonInlineAutoData("12")]
        [CommonInlineAutoData("123")]
        public static void Validate_CommencementDateYearTooShort_ThrowsValidationError(
            string year,
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.Day = "01";
            model.Month = "01";
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage(CommencementDateModelValidator.CommencementDateYearTooShortErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("99", "01", "2022")]
        [CommonInlineAutoData("01", "99", "2022")]
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
                .WithErrorMessage(CommencementDateModelValidator.CommencementDateInvalidErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(-59)]
        [CommonInlineAutoData(-20)]
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(0)]
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
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(20)]
        [CommonInlineAutoData(60)]
        [CommonInlineAutoData(180)]
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
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [CommonInlineAutoData("1 month")]
        [CommonInlineAutoData("one")]
        [CommonInlineAutoData("abcdef")]
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
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(0)]
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
        [CommonAutoData]
        public static void Validate_InitialPeriodTooHigh_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.InitialPeriod = $"{CommencementDateModelValidator.MaximumInitialPeriod + 1}";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.InitialPeriod)
                .WithErrorMessage(CommencementDateModelValidator.InitialPeriodTooHighErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
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
        [CommonInlineAutoData("1 month")]
        [CommonInlineAutoData("one")]
        [CommonInlineAutoData("abcdef")]
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
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(0)]
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
        [CommonAutoData]
        public static void Validate_MaximumTermTooHigh_ThrowsValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            model.MaximumTerm = $"{CommencementDateModelValidator.MaximumMaximumTerm + 1}";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.MaximumTerm)
                .WithErrorMessage(CommencementDateModelValidator.MaximumTermTooHighErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null, 1)]
        [CommonInlineAutoData(1, 1)]
        [CommonInlineAutoData(2, 1)]
        [CommonInlineAutoData(2, 2)]
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
                .WithErrorMessage(CommencementDateModelValidator.MaximumTermInvalidErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidModel_NoValidationError(
            CommencementDateModel model,
            CommencementDateModelValidator validator)
        {
            var validDate = DateTime.UtcNow.AddDays(20).Date;

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
