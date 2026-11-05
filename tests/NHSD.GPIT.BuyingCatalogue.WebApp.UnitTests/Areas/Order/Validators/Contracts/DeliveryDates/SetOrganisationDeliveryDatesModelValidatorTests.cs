using System;
using FluentAssertions;
using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using Xunit;
using OrderEntity = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.DeliveryDates;

public static class SetOrganisationDeliveryDatesModelValidatorTests
{
    [Fact]
    public static void Validate_OrderItemsDeliveryDatesIsEmpty_ThrowsValidationError()
    {
        var model = new SetOrganisationDeliveryDatesModel
        {
            InternalOrgId = "internal-org-id",
            CallOffId = default,
            OrderItemId = 42,
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
            InternalOrgId = "internal-org-id",
            CallOffId = default,
            OrderItemId = 42,
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
            InternalOrgId = "internal-org-id",
            CallOffId = default,
            OrderItemId = 42,
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

    [Fact]
    public static void Validate_ModelCreatedByConstructor_WithValidDeliveryDate_HasNoValidationErrors()
    {
        var commencementDate = new DateTime(2025, 1, 1);
        var orderItem = new OrderItem
        {
            Id = 42,
            CatalogueItem = new CatalogueItem { Name = "Catalogue item" },
        };
        var order = new OrderEntity
        {
            CommencementDate = commencementDate,
            MaximumTerm = 12,
        };
        var recipientOrderItem = new OrderItemSublocationRecipient
        {
            Id = 10,
            DeliveryDate = commencementDate.AddDays(1),
            OrderItem = orderItem,
        };
        var model = new SetOrganisationDeliveryDatesModel(
            "internal-org-id",
            order.CallOffId,
            orderItem.Id,
            "recipient-org-id",
            [recipientOrderItem],
            order);

        var result = new SetOrganisationDeliveryDatesModelValidator().TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public static void Validate_ModelCreatedByConstructor_WithDeliveryDateBeforeCommencementDate_ThrowsValidationError()
    {
        var commencementDate = new DateTime(2025, 1, 1);
        var orderItem = new OrderItem
        {
            Id = 42,
            CatalogueItem = new CatalogueItem { Name = "Catalogue item" },
        };
        var order = new OrderEntity
        {
            CommencementDate = commencementDate,
            MaximumTerm = 12,
        };
        var recipientOrderItem = new OrderItemSublocationRecipient
        {
            Id = 10,
            DeliveryDate = commencementDate.AddDays(-1),
            OrderItem = orderItem,
        };
        var model = new SetOrganisationDeliveryDatesModel(
            "internal-org-id",
            order.CallOffId,
            orderItem.Id,
            "recipient-org-id",
            [recipientOrderItem],
            order);

        var result = new SetOrganisationDeliveryDatesModelValidator().TestValidate(model);

        result.ShouldHaveValidationErrorFor("OrderItemsDeliveryDates[0].Day");
    }

    [Fact]
    public static void Constructor_AssignsModelPropertiesCorrectly()
    {
        var orderItem = new OrderItem
        {
            Id = 42,
            CatalogueItem = new CatalogueItem { Name = "Catalogue item" },
        };
        var recipientOrganisation = new OdsOrganisation
        {
            Id = "recipient-org-id",
            Name = "Recipient organisation",
        };
        var recipient = new OrderSublocationRecipient
        {
            RecipientOdsOrganisation = recipientOrganisation,
        };
        var orderItemRecipient = new OrderItemSublocationRecipient
        {
            Id = 10,
            DeliveryDate = new DateTime(2025, 2, 1),
            OrderItem = orderItem,
            Recipient = recipient,
        };
        var order = new OrderEntity
        {
            CommencementDate = new DateTime(2025, 1, 1),
            MaximumTerm = 12,
        };

        var model = new SetOrganisationDeliveryDatesModel(
            "internal-org-id",
            order.CallOffId,
            orderItem.Id,
            recipientOrganisation.Id,
            [orderItemRecipient],
            order);

        model.InternalOrgId.Should().Be("internal-org-id");
        model.CallOffId.Should().Be(order.CallOffId);
        model.OrderItemId.Should().Be(orderItem.Id);
        model.RecipientOrganisationId.Should().Be(recipientOrganisation.Id);
        model.RecipientOrganisationName.Should().Be(recipientOrganisation.Name);
        model.OrderItemsDeliveryDates.Should().ContainSingle();
        model.OrderItemsDeliveryDates[0].OrderSublocationRecipientId.Should().Be(orderItemRecipient.Id);
        model.OrderItemsDeliveryDates[0].CatalogueItemName.Should().Be(orderItem.CatalogueItem.Name);
        model.OrderItemsDeliveryDates[0].Date.Should().Be(orderItemRecipient.DeliveryDate);
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
