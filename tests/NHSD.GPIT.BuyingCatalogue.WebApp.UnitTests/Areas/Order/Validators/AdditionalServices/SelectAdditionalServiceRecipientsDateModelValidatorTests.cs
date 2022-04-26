using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Constants;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServiceRecipientsDate;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AdditionalServices
{
    public static class SelectAdditionalServiceRecipientsDateModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_ValidDeliveryDate_NoValidationErrors(
            SelectAdditionalServiceRecipientsDateModel model,
            SelectAdditionalServiceRecipientsDateModelValidator validator)
        {
            var validDate = DateTime.UtcNow.AddDays(1);

            model.Day = validDate.Day.ToString();
            model.Month = validDate.Month.ToString();
            model.Year = validDate.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_DeliveryDateInPast_ThrowsValidationError(
            SelectAdditionalServiceRecipientsDateModel model,
            SelectAdditionalServiceRecipientsDateModelValidator validator)
        {
            var date = DateTime.UtcNow.AddDays(-1);

            model.Day = date.Day.ToString();
            model.Month = date.Month.ToString();
            model.Year = date.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage("Planned delivery date must be in the future");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_BeyondCommencementDateLimit_ThrowsValidationError(
            SelectAdditionalServiceRecipientsDateModel model,
            SelectAdditionalServiceRecipientsDateModelValidator validator)
        {
            var commencementDate = DateTime.UtcNow;
            var deliveryDate = commencementDate.AddMonths(43);

            model.CommencementDate = commencementDate;
            model.Day = deliveryDate.Day.ToString();
            model.Month = deliveryDate.Month.ToString();
            model.Year = deliveryDate.Year.ToString();

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage($"Planned delivery date must be within {ValidationConstants.MaxDeliveryMonthsFromCommencement} months from the commencement date for this Call-off Agreement");
        }

        [Theory]
        [CommonInlineAutoData("", "2", "2022")]
        [CommonInlineAutoData("2", "", "2022")]
        [CommonInlineAutoData("2", "2", "")]
        public static void Validate_InvalidDate_ThrowsValidationError(
            string day,
            string month,
            string year,
            SelectAdditionalServiceRecipientsDateModel model,
            SelectAdditionalServiceRecipientsDateModelValidator validator)
        {
            model.Day = day;
            model.Month = month;
            model.Year = year;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Day)
                .WithErrorMessage("Planned delivery date must be a real date");
        }
    }
}
