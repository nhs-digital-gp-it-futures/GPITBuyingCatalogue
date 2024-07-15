using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteOrderModelValidatorTests
    {
        [Theory]
        [MockAutoData]
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
        [MockAutoData]
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
        [MockAutoData]
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
        [MockInlineAutoData("1")]
        [MockInlineAutoData("12")]
        [MockInlineAutoData("123")]
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
        [MockInlineAutoData("99", "01", "2022")]
        [MockInlineAutoData("01", "99", "2022")]
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
        [MockInlineAutoData(59)]
        [MockInlineAutoData(20)]
        [MockInlineAutoData(1)]
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
        [MockInlineAutoData(-59)]
        [MockInlineAutoData(-20)]
        [MockInlineAutoData(-1)]
        public static void Validate_ApprovalDateBeforeOrderCreation_ThrowsValidationError(
            int days,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var now = DateTime.UtcNow;
            var date = now.AddDays(days);

            model.OrderCreationDate = now;
            model.ApprovalDay = date.Day.ToString();
            model.ApprovalMonth = date.Month.ToString();
            model.ApprovalYear = date.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ApprovalDay)
                .WithErrorMessage(string.Format(DeleteOrderModelValidator.ApprovalDateBeforeOrderCreationErrorMessage, now.ToString("d MMMM yyyy")));
        }

        [Theory]
        [MockInlineAutoData(0)]
        [MockInlineAutoData(-1)]
        [MockInlineAutoData(-20)]
        public static void Validate_ApprovalDateWithinThreshold_NoValidationError(
            int days,
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var now = DateTime.UtcNow;
            model.OrderCreationDate = now.AddDays(-20);

            var approvalDate = now.AddDays(days);
            model.ApprovalDay = approvalDate.Day.ToString();
            model.ApprovalMonth = approvalDate.Month.ToString();
            model.ApprovalYear = approvalDate.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.ApprovalDay);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
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
        [MockAutoData]
        public static void Validate_ValidModel_NoValidationError(
            DeleteOrderModel model,
            DeleteOrderModelValidator validator)
        {
            var validDate = DateTime.UtcNow.AddDays(-1).Date;

            model.OrderCreationDate = validDate;
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
