using System;
using Bogus.DataSets;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteOrderModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_DeleteOrderDayMissing_ThrowsValidationError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.ApprovalDay = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApprovalDay)
                .WithErrorMessage(DeleteOrderModelValidator.ApprovalDateDayMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DeleteOrderMonthMissing_ThrowsValidationError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.ApprovalDay = "01";
            model.ApprovalMonth = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApprovalMonth)
                .WithErrorMessage(DeleteOrderModelValidator.ApprovalDateMonthMissingErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DeleteOrderYearMissing_ThrowsValidationError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.ApprovalDay = "01";
            model.ApprovalMonth = "01";
            model.ApprovalYear = string.Empty;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApprovalYear)
                .WithErrorMessage(DeleteOrderModelValidator.ApprovalDateYearMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("1")]
        [CommonInlineAutoData("12")]
        [CommonInlineAutoData("123")]
        public static void Validate_DeleteOrderYearTooShort_ThrowsValidationError(
            string year,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.ApprovalDay = "01";
            model.ApprovalMonth = "01";
            model.ApprovalYear = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApprovalYear)
                .WithErrorMessage(DeleteOrderModelValidator.ApprovalDateYearTooShortErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData("99", "01", "2022")]
        [CommonInlineAutoData("01", "99", "2022")]
        public static void Validate_InvalidDate_ThrowsValidationError(
            string day,
            string month,
            string year,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.ApprovalDay = day;
            model.ApprovalMonth = month;
            model.ApprovalYear = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApprovalDay)
                .WithErrorMessage(DeleteOrderModelValidator.ApprovalDateInvalidErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(59)]
        [CommonInlineAutoData(20)]
        [CommonInlineAutoData(1)]
        public static void Validate_ApprovalDateInTheFuture_ThrowsValidationError(
            int days,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(days);

            model.ApprovalDay = date.Day.ToString();
            model.ApprovalMonth = date.Month.ToString();
            model.ApprovalYear = date.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApprovalDay)
                .WithErrorMessage(DeleteOrderModelValidator.ApprovalDateInTheFutureErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(0)]
        [CommonInlineAutoData(-1)]
        [CommonInlineAutoData(-20)]
        public static void Validate_ApprovalDateWithinThreshold_NoValidationError(
            int days,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(days);

            model.ApprovalDay = date.Day.ToString();
            model.ApprovalMonth = date.Month.ToString();
            model.ApprovalYear = date.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.ApprovalDay);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_NameOfApproverNotEntered_ThrowsValidationError(
            string name,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.NameOfApprover = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.NameOfApprover)
                .WithErrorMessage(DeleteOrderModelValidator.NameOfApproverMissingErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_NameOfRequesterNotEntered_ThrowsValidationError(
            string name,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            model.NameOfRequester = name;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.NameOfRequester)
                .WithErrorMessage(DeleteOrderModelValidator.NameOfRequesterMissingErrorMessage);
        }

 

        [Theory]
        [CommonAutoData]
        public static void Validate_ValidModel_NoValidationError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var validDate = DateTime.UtcNow.AddDays(-1).Date;

            model.ApprovalDay = validDate.Day.ToString();
            model.ApprovalMonth = validDate.Month.ToString();
            model.ApprovalYear = validDate.Year.ToString();
            model.NameOfApprover = "Test";
            model.NameOfRequester = "Test";

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
