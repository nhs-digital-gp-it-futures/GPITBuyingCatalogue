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
        int orderItemId)
    {
        previousOrder.OrderNumber = currentOrder.OrderNumber;

        previousOrder.Revision = 1;
        currentOrder.Revision = 2;

        ICollection<OrderSublocationRecipient> result = currentOrder.DetermineOrderRecipients(
            previousOrder,
            orderItemId);

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
            orderItem.Id);

        result.Should().BeEquivalentTo(currentOrder.FlattenedRecipients);
    }

    [Theory]
    [MockAutoData]
    public static void OrderRecipients_DeterminesRecipientsForNewCatalogueItemOnExistingRecipient(
        Order previousOrder,
        Order currentOrder,
        OrderItem previousOrderItem,
        OrderItem currentOrderItem)
    {
        previousOrder.OrderNumber = currentOrder.OrderNumber;
        previousOrder.Revision = 1;
        currentOrder.Revision = 2;

        previousOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
        currentOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
        previousOrderItem.CatalogueItemId = new CatalogueItemId(1, "Prev");
        currentOrderItem.CatalogueItemId = new CatalogueItemId(2, "Curr");
        currentOrderItem.Parent = previousOrderItem;
        currentOrderItem.ParentId = previousOrderItem.Id;

        var recipient = new OrderSublocationRecipient
        {
            OrderId = currentOrder.Id,
            ParentSublocationOdsCode = "XXXX",
            RecipientOdsCode = "AAAA",
            OrderItemSublocationRecipients =
            [new OrderItemSublocationRecipient(currentOrder.Id, "AAAA", currentOrderItem)],
        };
        var previousRecipient = new OrderSublocationRecipient
        {
            OrderId = previousOrder.Id,
            ParentSublocationOdsCode = "XXXX",
            RecipientOdsCode = "AAAA",
            OrderItemSublocationRecipients =
            [new OrderItemSublocationRecipient(previousOrder.Id, "AAAA", previousOrderItem)],
        };
        previousOrder.OrderItems = [previousOrderItem];
        previousOrder.OrderSublocations =
        [
            new OrderSublocation
            {
                SublocationRecipients = [previousRecipient],
            },
        ];
        currentOrder.OrderItems = [currentOrderItem];
        currentOrder.OrderSublocations =
        [
            new OrderSublocation
            {
                SublocationRecipients = [recipient],
            },
        ];

        var result = currentOrder.DetermineOrderRecipientsWithNewServices(previousOrder, currentOrderItem.Id);

        result.Should().ContainSingle().Which.Should().Be(recipient);
    }

    [Fact]
    public static void OrderRecipientsWithNewServices_ReturnsAllCurrentRecipientsWhenOrderHasNewService()
    {
        var existingCatalogueItemId = new CatalogueItemId(1, "Exist");
        var newCatalogueItemId = new CatalogueItemId(1, "New");
        var previousOrderItem = new OrderItem
        {
            Id = 1,
            CatalogueItemId = existingCatalogueItemId,
            CatalogueItem = new CatalogueItem { Id = existingCatalogueItemId },
        };
        var currentOrderItem = new OrderItem
        {
            Id = 2,
            CatalogueItemId = newCatalogueItemId,
            CatalogueItem = new CatalogueItem { Id = newCatalogueItemId },
        };
        var previousRecipient = CreateRecipient("AAAA", previousOrderItem, 1);
        var currentRecipient = CreateRecipient("AAAA", currentOrderItem, 2);
        var newCurrentRecipient = CreateRecipient("AAAB", currentOrderItem, 2);
        var previousOrder = new Order
        {
            OrderItems = [previousOrderItem],
            OrderSublocations = [new OrderSublocation { SublocationRecipients = [previousRecipient] }],
        };
        var currentOrder = new Order
        {
            OrderItems = [previousOrderItem, currentOrderItem],
            OrderSublocations =
            [
                new OrderSublocation
                {
                    SublocationRecipients = [currentRecipient, newCurrentRecipient],
                },
            ],
        };

        var result = currentOrder.DetermineOrderRecipientsWithNewServices(previousOrder, currentOrderItem.Id);

        result.Should().BeEquivalentTo([currentRecipient, newCurrentRecipient]);
    }

    [Fact]
    public static void OrderRecipientsWithNewServices_WithNoPreviousOrder_ReturnsAllCurrentRecipients()
    {
        var catalogueItemId = new CatalogueItemId(1, "Service");
        var orderItem = new OrderItem
        {
            Id = 1,
            CatalogueItemId = catalogueItemId,
            CatalogueItem = new CatalogueItem { Id = catalogueItemId },
        };
        var firstRecipient = CreateRecipient("AAAA", orderItem, 1);
        var secondRecipient = CreateRecipient("AAAB", orderItem, 1);
        var currentOrder = new Order
        {
            OrderItems = [orderItem],
            OrderSublocations =
            [
                new OrderSublocation
                {
                    SublocationRecipients = [firstRecipient, secondRecipient],
                },
            ],
        };

        var result = currentOrder.DetermineOrderRecipientsWithNewServices(null, orderItem.Id);

        result.Should().BeEquivalentTo([firstRecipient, secondRecipient]);
    }

    [Fact]
    public static void OrderRecipientsWithNewServices_WithUnknownOrderItem_ReturnsNoRecipients()
    {
        var currentOrder = new Order
        {
            OrderItems = [],
            OrderSublocations =
            [new OrderSublocation { SublocationRecipients = [new OrderSublocationRecipient()] }],
        };

        var result = currentOrder.DetermineOrderRecipientsWithNewServices(new Order(), 1);

        result.Should().BeEmpty();
    }

    private static OrderSublocationRecipient CreateRecipient(
        string recipientOdsCode,
        OrderItem orderItem,
        int orderId)
    {
        return new OrderSublocationRecipient
        {
            OrderId = orderId,
            RecipientOdsCode = recipientOdsCode,
            OrderItemSublocationRecipients =
            [new OrderItemSublocationRecipient(orderId, recipientOdsCode, orderItem)],
        };
    }

    public static IEnumerable<object[]> OrderRecipientsWithOrderItemsAndExpectedResults()
    {
        var framework = new Catalogue.Models.Framework { Id = new Random().Next().ToString() };

        var orderingPartyId = 667;

        var catalogueItemId = new CatalogueItemId(556, "Id");
        var catalogueItem = new CatalogueItem
        {
            Id = catalogueItemId,
            CatalogueItemType = CatalogueItemType.AssociatedService,
        };

        var orderItemId1 = 1;
        var orderItemId2 = 2;

        var orderItem1 = new OrderItem
        {
            Id = orderItemId1,
            CatalogueItem = catalogueItem,
            CatalogueItemId = catalogueItemId,
            OrderId = 1001,
        };
        var orderItem2 = new OrderItem
        {
            Id = orderItemId2,
            CatalogueItem = catalogueItem,
            CatalogueItemId = catalogueItemId,
            OrderId = 1002,
        };

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
                    OrderItems = [orderItem1],
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
                                        [new OrderItemSublocationRecipient(1001, "AAAC", orderItem1)],
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
                    OrderItems = [orderItem2],
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
                                        [new OrderItemSublocationRecipient(1002, "AAAA", orderItem2)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAH",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAB", orderItem2)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAC",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAC", orderItem2)],
                                },
                            ],
                        },
                    ],
                },
                orderItemId2,
                new List<OrderSublocationRecipient>
                {
                    new()
                    {
                        OrderId = 1002,
                        ParentSublocationOdsCode = "XXXX",
                        RecipientOdsCode = "AAAG",
                        OrderItemSublocationRecipients =
                            [new OrderItemSublocationRecipient(1002, "AAAA", orderItem2)],
                    },
                    new()
                    {
                        OrderId = 1002,
                        ParentSublocationOdsCode = "XXXX",
                        RecipientOdsCode = "AAAH",
                        OrderItemSublocationRecipients =
                            [new OrderItemSublocationRecipient(1002, "AAAB", orderItem2)],
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
                    OrderItems = [orderItem1],
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
                                        [new OrderItemSublocationRecipient(1001, "AAAA", orderItem1)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1001,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAB",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1001, "AAAB", orderItem1)],
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
                    OrderItems = [orderItem2],
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
                                        [new OrderItemSublocationRecipient(1002, "AAAA", orderItem2)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAB",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAB", orderItem2)],
                                },
                                new OrderSublocationRecipient
                                {
                                    OrderId = 1002,
                                    ParentSublocationOdsCode = "XXXX",
                                    RecipientOdsCode = "AAAC",
                                    OrderItemSublocationRecipients =
                                        [new OrderItemSublocationRecipient(1002, "AAAC", orderItem2)],
                                },
                            ],
                        },
                    ],
                },
                orderItemId2,
                new List<OrderSublocationRecipient>
                {
                    new()
                    {
                        OrderId = 1002,
                        ParentSublocationOdsCode = "XXXX",
                        RecipientOdsCode = "AAAC",
                        OrderItemSublocationRecipients =
                            [new OrderItemSublocationRecipient(1002, "AAAC", orderItem2)],
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
        int orderItemId,
        List<OrderSublocationRecipient> expectedRecipients)
    {
        ICollection<OrderSublocationRecipient> result = currentOrder.DetermineOrderRecipients(
            previousOrder,
            orderItemId);

        result.Should().BeEquivalentTo(expectedRecipients);
    }
}
