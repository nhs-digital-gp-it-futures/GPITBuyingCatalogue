using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering;

public static class OrderTests
{
    [Fact]
    public static void OrderStatus_Deleted()
    {
        var order = new Order { IsDeleted = true };

        order.OrderStatus.Should().Be(OrderStatus.Deleted);
    }

    [Fact]
    public static void OrderStatus_Completed()
    {
        var order = new Order { Completed = DateTime.UtcNow };

        order.OrderStatus.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public static void OrderStatus_InProgress()
    {
        var order = new Order();

        order.OrderStatus.Should().Be(OrderStatus.InProgress);
    }

    [Fact]
    public static void OrderStatus_Expired()
    {
        var order = new Order()
        {
            CommencementDate = new(2023, 02, 04), MaximumTerm = 6,
        };

        order.OrderStatus.Should().Be(OrderStatus.Expired);
    }

    [Theory]
    [MockAutoData]
    public static void Order_GetSolutions_ReturnsExpectedResult(
        List<OrderItem> orderItems)
    {
        foreach (var item in orderItems)
        {
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
        }

        orderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

        var order = new Order { OrderItems = orderItems };

        var result = order.GetSolutions().As<IEnumerable<OrderItem>>();
        result.Should().NotBeNullOrEmpty();
        result.Count().Should().Be(orderItems.Count - 1);
    }

    [Fact]
    public static void ContractExpired_NoEndDate_PropertyCorrectlySet()
    {
        var order = new Order { MaximumTerm = null };

        order.ContractExpired.Should().Be(false);
    }

    [Theory]
    [MockInlineAutoData(-1, true)]
    [MockInlineAutoData(0, false)]
    [MockInlineAutoData(1, false)]
    public static void ContractExpired_PropertyCorrectlySet(
        int remainingDaysOfContract,
        bool value)
    {
        const int maxTerm = 36;

        var order = new Order { MaximumTerm = maxTerm, CommencementDate = DateTime.UtcNow.AddMonths(-maxTerm).AddDays(1 + remainingDaysOfContract) };

        order.ContractExpired.Should().Be(value);
    }

    [Theory]
    [MockAutoData]
    public static void OrderRecipients_DeterminesRecipientsForOrder_DoesNotExist(
        Order previousOrder,
        Order currentOrder,
        CatalogueItemId catalogueItemId)
    {
        previousOrder.OrderNumber = currentOrder.OrderNumber;

        previousOrder.Revision = 1;
        currentOrder.Revision = 2;

        ICollection<OrderSublocationRecipient> result = currentOrder.DetermineOrderRecipients(
            previousOrder,
            catalogueItemId);

        Assert.Equal([], result);
    }

    [Theory]
    [MockAutoData]
    public static void OrderRecipients_DeterminesRecipientsForOrder_ExistsInCurrentButNotPreviousOrder(
        Order previousOrder,
        Order currentOrder,
        OrderItem orderItem)
    {
        previousOrder.OrderNumber = currentOrder.OrderNumber;

        previousOrder.Revision = 1;
        currentOrder.Revision = 2;

        currentOrder.OrderItems.Add(orderItem);

        ICollection<OrderSublocationRecipient> result = currentOrder.DetermineOrderRecipients(
            previousOrder,
            orderItem.CatalogueItemId);

        result.Should().BeEquivalentTo(currentOrder.FlattenedRecipients);
    }

    public static IEnumerable<object[]> OrderRecipientsWithOrderItemsAndExpectedResults()
    {
        var framework = new Catalogue.Models.Framework { Id = new Random().Next().ToString() };

        var orderingPartyId = 667;

        var catalogueItemId = new CatalogueItemId(556, "334");

        var orderItem = new OrderItem { CatalogueItemId = catalogueItemId };

        return
        [
            [

                // Catalogue Item did not exist in previous order
                new Order
                {
                    Id = 1001,
                    OrderNumber = 5555,
                    Revision = 1,
                    Description = "My order",
                    OrderingPartyId = orderingPartyId,
                    SelectedFramework = framework,
                    OrderItems = [new OrderItem { CatalogueItemId = catalogueItemId, OrderId = 1001 }],
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            OrderId = 1001,
                            SublocationOdsCode = "XXXX",
                            OwnerOdsCode = "AA-FF",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1001,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAA",
                                    OrderItemSublocationRecipients =
                                        [],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1001,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAB",
                                    OrderItemSublocationRecipients =
                                        [],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1001,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAC",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1001, "AAAC", catalogueItemId)],
                                },
                            ],
                        },
                    ],
                },
                new Order
                {
                    Id = 1002,
                    OrderNumber = 5555,
                    Revision = 2,
                    Description = "My order",
                    OrderingPartyId = orderingPartyId,
                    SelectedFramework = framework,
                    OrderItems = [new OrderItem { CatalogueItemId = catalogueItemId, OrderId = 1002 }],
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            OrderId = 1002,
                            SublocationOdsCode = "XXXX",
                            OwnerOdsCode = "AA-FF",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAG",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAG", catalogueItemId)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAH",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAH", catalogueItemId)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAC",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAC", catalogueItemId)],
                                },
                            ],
                        },
                    ],
                },
                catalogueItemId,
                new List<OrderSublocationRecipient>
                {
                    new()
                    {
                        OrderId = 1002,
                        ParentSublocationOdsCode = "XXXX",
                        RecipientOdsCode = "AAAG",
                        OrderItemSublocationRecipients =
                            [new OrderItemSublocationRecipient(1002, "AAAG", catalogueItemId)],
                    },
                    new()
                    {
                        OrderId = 1002,
                        ParentSublocationOdsCode = "XXXX",
                        RecipientOdsCode = "AAAH",
                        OrderItemSublocationRecipients =
                            [new OrderItemSublocationRecipient(1002, "AAAH", catalogueItemId)],
                    },
                },
            ],
            [

                // Recipient did not exist in previous order
                new Order
                {
                    Id = 1001,
                    OrderNumber = 5555,
                    Revision = 1,
                    Description = "My order",
                    OrderingPartyId = orderingPartyId,
                    SelectedFramework = framework,
                    OrderItems = [new OrderItem { CatalogueItemId = catalogueItemId, OrderId = 1001 }],
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            OrderId = 1001,
                            SublocationOdsCode = "XXXX",
                            OwnerOdsCode = "AA-FF",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1001,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAA",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1001, "AAAA", catalogueItemId)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1001,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAB",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1001, "AAAB", catalogueItemId)],
                                },
                            ],
                        },
                    ],
                },
                new Order
                {
                    Id = 1002,
                    OrderNumber = 5555,
                    Revision = 2,
                    Description = "My order",
                    OrderingPartyId = orderingPartyId,
                    SelectedFramework = framework,
                    OrderItems = [new OrderItem { CatalogueItemId = catalogueItemId, OrderId = 1002 }],
                    OrderSublocations =
                    [
                        new OrderSublocation
                        {
                            OrderId = 1002,
                            SublocationOdsCode = "XXXX",
                            OwnerOdsCode = "AA-FF",
                            SublocationRecipients =
                            [
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAA",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAA", catalogueItemId)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAB",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAB", catalogueItemId)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAC",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAC", catalogueItemId)],
                                },
                            ],
                        },
                    ],
                },
                catalogueItemId,
                new List<OrderSublocationRecipient>
                {
                    new()
                    {
                        OrderId = 1002,
                        ParentSublocationOdsCode = "XXXX",
                        RecipientOdsCode = "AAAC",
                        OrderItemSublocationRecipients =
                            [new OrderItemSublocationRecipient(1002, "AAAC", catalogueItemId)],
                    },
                },
            ],
        ];
    }

    [Theory]
    [MockMemberAutoData(nameof(OrderRecipientsWithOrderItemsAndExpectedResults))]
    public static void OrderRecipients_DeterminesRecipientsForOrder_Scenarios(
        Order previousOrder,
        Order currentOrder,
        CatalogueItemId catalogueItemId,
        List<OrderSublocationRecipient> expectedRecipients)
    {
        ICollection<OrderSublocationRecipient> result = currentOrder.DetermineOrderRecipients(
            previousOrder,
            catalogueItemId);

        result.Should().BeEquivalentTo(expectedRecipients);
    }
}
