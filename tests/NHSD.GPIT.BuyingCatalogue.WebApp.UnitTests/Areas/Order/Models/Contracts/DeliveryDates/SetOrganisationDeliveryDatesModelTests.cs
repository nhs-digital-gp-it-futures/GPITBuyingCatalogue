using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using Xunit;
using OrderEntity = NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Contracts.DeliveryDates;

public static class SetOrganisationDeliveryDatesModelTests
{
    [Fact]
    public static void Constructor_WithValidArguments_PropertiesCorrectlySet()
    {
        var commencementDate = new DateTime(2025, 1, 1);
        var deliveryDate = new DateTime(2025, 2, 1);
        var order = new OrderEntity
        {
            OrderNumber = 123,
            Revision = 1,
            CommencementDate = commencementDate,
            MaximumTerm = 12,
        };
        var orderItem = new OrderItem
        {
            Id = 42,
            CatalogueItem = new CatalogueItem { Name = "Catalogue item" },
        };
        var recipient = new OrderSublocationRecipient
        {
            RecipientOdsOrganisation = new OdsOrganisation { Name = "Recipient organisation" },
        };
        var orderItemRecipient = new OrderItemSublocationRecipient
        {
            Id = 10,
            DeliveryDate = deliveryDate,
            OrderItem = orderItem,
            Recipient = recipient,
        };

        var model = new SetOrganisationDeliveryDatesModel(
            "internal-org-id",
            order.CallOffId,
            orderItem.Id,
            "recipient-org-id",
            [orderItemRecipient],
            order);

        model.InternalOrgId.Should().Be("internal-org-id");
        model.CallOffId.Should().Be(order.CallOffId);
        model.OrderItemId.Should().Be(orderItem.Id);
        model.RecipientOrganisationId.Should().Be("recipient-org-id");
        model.RecipientOrganisationName.Should().Be("Recipient organisation");
        model.OrderItemsDeliveryDates.Should().ContainSingle();

        var deliveryDateModel = model.OrderItemsDeliveryDates[0];
        deliveryDateModel.OrderSublocationRecipientId.Should().Be(orderItemRecipient.Id);
        deliveryDateModel.CatalogueItemName.Should().Be(orderItem.CatalogueItem.Name);
        deliveryDateModel.Date.Should().Be(deliveryDate);
        deliveryDateModel.CommencementDate.Should().Be(commencementDate);
        deliveryDateModel.MaximumTerm.Should().Be(order.MaximumTerm);
        deliveryDateModel.IsAmend.Should().BeFalse();
    }

    [Fact]
    public static void Constructor_WithNoRecipientOrganisation_UsesEmptyOrganisationName()
    {
        var order = new OrderEntity();
        var orderItem = new OrderItem
        {
            CatalogueItem = new CatalogueItem { Name = "Catalogue item" },
        };
        var orderItemRecipient = new OrderItemSublocationRecipient
        {
            OrderItem = orderItem,
        };

        var model = new SetOrganisationDeliveryDatesModel(
            "internal-org-id",
            order.CallOffId,
            orderItem.Id,
            "recipient-org-id",
            [orderItemRecipient],
            order);

        model.RecipientOrganisationName.Should().BeEmpty();
    }

    [Fact]
    public static void GetPageTitle_ReturnsRecipientOrganisationName()
    {
        var model = new SetOrganisationDeliveryDatesModel
        {
            InternalOrgId = "internal-org-id",
            CallOffId = default,
            OrderItemId = 42,
            RecipientOrganisationName = "Recipient organisation",
        };

        model.GetPageTitle().Should().Be("Change delivery dates for Recipient organisation");
    }
}
