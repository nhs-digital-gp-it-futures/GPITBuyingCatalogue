using System;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates;

public static class SetOrganisationDeliveryDatesModelValidatorTests
{
    [Fact]
    public static void Validate_OrderItemsDeliveryDatesIsEmpty_ThrowsValidationError()
    {
        var model = new SetOrganisationDeliveryDatesModel
        {
            OrderItemsDeliveryDates = [],
        };
        var validator = new SetOrganisationDeliveryDatesModelValidator();

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.OrderItemsDeliveryDates);
    }

    [Fact]
    public static void Validate_InvalidOrderItemDeliveryDate_ThrowsValidationErrorForItem()
    {
        var model = new SetOrganisationDeliveryDatesModel
        {
            OrderItemsDeliveryDates =
            [
                new OrderItemDeliveryDate
                {
                    Day = "31",
                    Month = "02",
                    Year = "2025",
                    CommencementDate = new DateTime(2025, 1, 1),
                    MaximumTerm = 12,
                },
            ],
        };
        var validator = new SetOrganisationDeliveryDatesModelValidator();

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor("OrderItemsDeliveryDates[0].Day");
    }

    [Fact]
    public static void Validate_ValidOrderItemDeliveryDate_HasNoValidationErrors()
    {
        var model = new SetOrganisationDeliveryDatesModel
        {
            OrderItemsDeliveryDates =
            [
                CreateOrderItemDeliveryDate(new DateTime(2025, 2, 1)),
                CreateOrderItemDeliveryDate(new DateTime(2025, 3, 1)),
            ],
        };
        var validator = new SetOrganisationDeliveryDatesModelValidator();

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static OrderItemDeliveryDate CreateOrderItemDeliveryDate(DateTime date)
    {
        return new OrderItemDeliveryDate
        {
            Day = date.Day.ToString(),
            Month = date.Month.ToString(),
            Year = date.Year.ToString(),
            CommencementDate = new DateTime(2025, 1, 1),
            MaximumTerm = 12,
        };
    }
}
