using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderItemTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(CatalogueItemId catalogueItemId)
    {
        var orderItem = new OrderItem(catalogueItemId);

        orderItem.CatalogueItemId.Should().Be(catalogueItemId);
    }

    [Theory]
    [MockAutoData]
    public static void TotalQuantity_WhenOrderItemIdsDiffer_FallsBackToCatalogueItemId(
        OrderItem orderItem,
        OrderItem linkedOrderItem,
        OrderSublocationRecipient recipient)
    {
        const int expectedQuantity = 7;

        orderItem.Id = 1;
        linkedOrderItem.Id = 2;
        linkedOrderItem.CatalogueItemId = orderItem.CatalogueItemId;
        recipient.OrderItemSublocationRecipients =
        [
            new OrderItemSublocationRecipient
            {
                OrderItemId = linkedOrderItem.Id,
                OrderItem = linkedOrderItem,
                Quantity = expectedQuantity,
            },
        ];

        orderItem.TotalQuantity([recipient]).Should().Be(expectedQuantity);
    }

    [Theory]
    [MockAutoData]
    public static void TotalQuantity_WhenOrderItemIdMatches_UsesOrderItemId(
        OrderItem orderItem,
        OrderItem linkedOrderItem,
        OrderSublocationRecipient recipient)
    {
        const int expectedQuantity = 5;

        orderItem.Id = 1;
        linkedOrderItem.Id = 2;
        linkedOrderItem.CatalogueItemId = orderItem.CatalogueItemId;
        recipient.OrderItemSublocationRecipients =
        [
            new OrderItemSublocationRecipient
            {
                OrderItemId = orderItem.Id,
                OrderItem = orderItem,
                Quantity = expectedQuantity,
            },
            new OrderItemSublocationRecipient
            {
                OrderItemId = linkedOrderItem.Id,
                OrderItem = linkedOrderItem,
                Quantity = 7,
            },
        ];

        orderItem.TotalQuantity([recipient]).Should().Be(expectedQuantity);
    }
}
