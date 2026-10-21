using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates;

public static class OrderItemDeliveryDateValidatorTests
{
    [Fact]
    public static void Validate_DateBeforeCommencementDate_ThrowsValidationError()
    {
        var commencementDate = new DateTime(2025, 1, 1);
        var model = CreateModel(commencementDate.AddDays(-1), commencementDate, 12);
        var validator = new OrderItemDeliveryDateValidator();

        var result = validator.TestValidate(model);

        var errorMessage = string.Format(
            OrderItemDeliveryDateValidator.DeliveryDateBeforeCommencementDateErrorMessage,
            $"{commencementDate:d MMMM yyyy}");

        result.ShouldHaveValidationErrorFor(x => x.Day)
            .WithErrorMessage(errorMessage);
    }

    [Fact]
    public static void Validate_DateAfterContractEndDate_ThrowsValidationError()
    {
        var commencementDate = new DateTime(2025, 1, 1);
        var contractEndDate = new EndDate(commencementDate, 12);
        var model = CreateModel(contractEndDate.DateTime.Value.AddDays(1), commencementDate, 12);
        var validator = new OrderItemDeliveryDateValidator();

        var result = validator.TestValidate(model);

        var errorMessage = string.Format(
            OrderItemDeliveryDateValidator.DeliveryDateAfterContractEndDateErrorMessage,
            $"{contractEndDate.DateTime:d MMMM yyyy}");

        result.ShouldHaveValidationErrorFor(x => x.Day)
            .WithErrorMessage(errorMessage);
    }

    [Fact]
    public static void Validate_AmendmentDateAfterContractEndDate_UsesAmendmentErrorMessage()
    {
        var commencementDate = new DateTime(2025, 1, 1);
        var contractEndDate = new EndDate(commencementDate, 12);
        var model = CreateModel(contractEndDate.DateTime.Value.AddDays(1), commencementDate, 12);
        model.IsAmend = true;
        var validator = new OrderItemDeliveryDateValidator();

        var result = validator.TestValidate(model);

        var errorMessage = string.Format(
            OrderItemDeliveryDateValidator.AmendDeliveryDateAfterContractEndDateErrorMessage,
            $"{contractEndDate.DateTime:d MMMM yyyy}");

        result.ShouldHaveValidationErrorFor(x => x.Day)
            .WithErrorMessage(errorMessage);
    }

    [Fact]
    public static void Validate_DateWithinContractDates_HasNoValidationErrors()
    {
        var commencementDate = new DateTime(2025, 1, 1);
        var model = CreateModel(commencementDate.AddDays(1), commencementDate, 12);
        var validator = new OrderItemDeliveryDateValidator();

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public static void Validate_InvalidDate_ThrowsValidationError()
    {
        var model = new OrderItemDeliveryDate
        {
            Day = "31",
            Month = "02",
            Year = "2025",
            CommencementDate = new DateTime(2025, 1, 1),
            MaximumTerm = 12,
        };
        var validator = new OrderItemDeliveryDateValidator();

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Day)
            .WithErrorMessage(DateInputModelValidator.DateInvalidErrorMessage);
    }

    private static OrderItemDeliveryDate CreateModel(
        DateTime deliveryDate,
        DateTime commencementDate,
        int maximumTerm)
    {
        return new OrderItemDeliveryDate
        {
            Day = deliveryDate.Day.ToString(),
            Month = deliveryDate.Month.ToString(),
            Year = deliveryDate.Year.ToString(),
            CommencementDate = commencementDate,
            MaximumTerm = maximumTerm,
        };
    }
}
