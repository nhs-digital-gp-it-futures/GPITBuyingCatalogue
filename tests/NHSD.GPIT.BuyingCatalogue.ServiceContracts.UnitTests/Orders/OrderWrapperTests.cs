using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Orders
{
    public static class OrderWrapperTests
    {
        [Fact]
        public static void OrderWrapper_Create_Throws_No_Order()
        {
            var action = () => new OrderWrapper(null, []);
            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void OrderWrapper_Create(Order order)
        {
            order.Revision = 1;

            var orderWrapper = new OrderWrapper(order, []);
            orderWrapper.Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void OrderWrapper_RolledUp_Uses_Old_Order_Data_Apart_From_Revision(IFixture fixture)
        {
            var order = fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .Create();

            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.Description = $"Edited-{order.Description}";

            var orderWrapper = new OrderWrapper(amendedOrder, [order]);

            orderWrapper.Previous.Revision.Should().Be(1);
            orderWrapper.Previous.Description.Should().Be(order.Description);
            orderWrapper.Order.Revision.Should().Be(2);
            orderWrapper.Order.Description.Should().Be($"Edited-{order.Description}");
            orderWrapper.RolledUp.Revision.Should().Be(2);
            orderWrapper.RolledUp.Description.Should().Be(order.Description);
        }

        [Theory]
        [MockAutoData]
        public static void OrderWrapper_FundingTypesForItem_Returns_None_When_No_Funding(
            CatalogueItem catalogueItem,
            IFixture fixture,
            Organisation organisation)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, null);

            Order order = BuildOrder(
                fixture,
                [orderItem],
                [
                    BuildOrderSublocation(
                        fixture,
                        "XXXX",
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem.CatalogueItemId])]),
                ],
                organisation);

            var orderWrapper = new OrderWrapper(order);

            var result = orderWrapper.FundingTypesForItem(catalogueItem.Id);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo([OrderItemFundingType.None]);
        }

        [Theory]
        [MockAutoData]
        public static void OrderWrapper_FundingTypesForItem_Returns_Multiple(
            CatalogueItem catalogueItem,
            IFixture fixture,
            Organisation organisation)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(
                fixture,
                [orderItem],
                [
                    BuildOrderSublocation(
                        fixture,
                        "XXXX",
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem.CatalogueItemId])]),
                ],
                organisation);
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = [amendedOrderItem,];

            var orderWrapper = new OrderWrapper(amendedOrder, [order]);

            var result = orderWrapper.FundingTypesForItem(catalogueItem.Id);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo([OrderItemFundingType.LocalFunding, OrderItemFundingType.MixedFunding]);
        }

        [Theory]
        [MockAutoData]
        public static void OrderWrapper_FundingTypesForItem_Distinct_Result(
            CatalogueItem catalogueItem,
            Organisation organisation,
            IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);

            Order order = BuildOrder(
                fixture,
                [orderItem],
                [
                    BuildOrderSublocation(
                        fixture,
                        "XXXX",
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem.CatalogueItemId])]),
                ],
                organisation);
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = [amendedOrderItem];

            var orderWrapper = new OrderWrapper(amendedOrder, [order]);

            var result = orderWrapper.FundingTypesForItem(catalogueItem.Id);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo([OrderItemFundingType.LocalFunding]);
        }

        [Theory]
        [MockAutoData]
        public static void OrderWrapper_RolledUp_Uses_Old_OrderItem_Data_Single_sublocation(
            CatalogueItem catalogueItem,
            IFixture fixture,
            Organisation organisation)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(
                fixture,
                [orderItem],
                [
                    BuildOrderSublocation(
                        fixture,
                        "XXXX",
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem.CatalogueItemId])]),
                ],
                organisation);
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = [amendedOrderItem];
            amendedOrder.OrderSublocations =
            [
                BuildOrderSublocation(
                    fixture,
                    "XXXX",
                    [
                        BuildOrderSublocationRecipient(fixture, "XXXX", [catalogueItem.Id]),
                    ]),
            ];

            var orderWrapper = new OrderWrapper(amendedOrder, [order]);

            orderWrapper.Previous.OrderItems.Count.Should().Be(1);
            orderWrapper.Previous.FlattenedRecipients.Count().Should().Be(1);
            orderWrapper.Previous.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);

            orderWrapper.Order.OrderItems.Count.Should().Be(1);
            orderWrapper.Order.FlattenedRecipients.Count().Should().Be(1);
            orderWrapper.Order.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.MixedFunding);

            orderWrapper.RolledUp.OrderItems.Count.Should().Be(1);
            orderWrapper.RolledUp.FlattenedRecipients.Count().Should().Be(2);
            orderWrapper.RolledUp.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);
        }

        [Theory]
        [MockAutoData]
        public static void OrderWrapper_RolledUp_Uses_Old_OrderItem_Data_Multiple_sublocations(
            CatalogueItem catalogueItem,
            IFixture fixture,
            Organisation organisation)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(
                fixture,
                [orderItem],
                [
                    BuildOrderSublocation(
                        fixture,
                        "XXXX",
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem.CatalogueItemId])]),
                    BuildOrderSublocation(
                        fixture,
                        "XXXY",
                        [BuildOrderSublocationRecipient(fixture, "XXXY", [orderItem.CatalogueItemId])]),
                ],
                organisation);
            Order amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = [amendedOrderItem];
            amendedOrder.OrderSublocations =
            [
                BuildOrderSublocation(
                    fixture,
                    "XXXX",
                    [
                        BuildOrderSublocationRecipient(fixture, "XXXX", [catalogueItem.Id]),
                    ]),
                BuildOrderSublocation(
                    fixture,
                    "XXXY",
                    [
                        BuildOrderSublocationRecipient(fixture, "XXXX", [catalogueItem.Id]),
                    ]),
                BuildOrderSublocation(
                    fixture,
                    "XXXZ",
                    [
                        BuildOrderSublocationRecipient(fixture, "XXXX", [catalogueItem.Id]),
                    ]),
            ];

            var orderWrapper = new OrderWrapper(amendedOrder, [order]);

            orderWrapper.Previous.OrderItems.Count.Should().Be(1);
            orderWrapper.Previous.FlattenedRecipients.Count().Should().Be(2);
            orderWrapper.Previous.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);

            orderWrapper.Order.OrderItems.Count.Should().Be(1);
            orderWrapper.Order.FlattenedRecipients.Count().Should().Be(3);
            orderWrapper.Order.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.MixedFunding);

            orderWrapper.RolledUp.OrderItems.Count.Should().Be(1);
            orderWrapper.RolledUp.FlattenedRecipients.Count().Should().Be(5);
            orderWrapper.RolledUp.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);
        }

        [Theory]
        [MockAutoData]
        public static void GetCallOffIdForRecipient_Returns_ExpectedCallOffId(
            IFixture fixture,
            CatalogueItem catalogueItem,
            Organisation organisation)
        {
            var sublocationRecipient = BuildOrderSublocationRecipient(fixture, "XXXX", [catalogueItem.Id]);
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(
                fixture,
                [orderItem],
                [
                    BuildOrderSublocation(
                        fixture,
                        "XXXX",
                        [sublocationRecipient]),
                ],
                organisation);
            Order amendedOrder = order.BuildAmendment(2);
            amendedOrder.Id = 1;
            amendedOrder.OrderItems = [amendedOrderItem];
            amendedOrder.OrderSublocations =
            [
                BuildOrderSublocation(
                    fixture,
                    "XXXX",
                    [sublocationRecipient]),
            ];

            var orderWrapper = new OrderWrapper(amendedOrder, [order]);
            var previousOrdersDictionary = orderWrapper.PreviousOrders.ToDictionary(o => o.Id);

            orderWrapper.GetCallOffIdForRecipient(previousOrdersDictionary, sublocationRecipient).Should().Be(order.CallOffId.ToString());
            orderWrapper.GetCallOffIdForRecipient(previousOrdersDictionary, sublocationRecipient).Should().NotBe(amendedOrder.CallOffId.ToString());
        }

        private static OrderSublocation BuildOrderSublocation(
            IFixture fixture,
            string sublocationOdsCode,
            OrderSublocationRecipient[] orderSublocationRecipients)
        {
            OrderSublocation sublocation = fixture.Build<OrderSublocation>()
                .With(s => s.SublocationRecipients, new List<OrderSublocationRecipient>(orderSublocationRecipients))
                .With(s => s.SublocationOdsCode, sublocationOdsCode)
                .Create();

            return sublocation;
        }

        private static OrderSublocationRecipient BuildOrderSublocationRecipient(
            IFixture fixture,
            string sublocationOdsCode,
            CatalogueItemId[] catalogueItemIds = null,
            int id = 0)
        {
            OrderSublocationRecipient recipient = fixture.Build<OrderSublocationRecipient>()
                .Without(r => r.OrderItemSublocationRecipients)
                .With(r => r.ParentSublocationOdsCode, sublocationOdsCode)
                .With(r => r.OrderId, id)
                .Create();

            UpdateRecipientToItem(recipient, catalogueItemIds);

            return recipient;
        }

        private static void UpdateRecipientToItem(
            OrderSublocationRecipient recipient,
            CatalogueItemId[] catalogueItemIds)
        {
            if (catalogueItemIds != null)
            {
                foreach (var catalogueItemId in catalogueItemIds)
                {
                    recipient.SetQuantityForItem(catalogueItemId, 1);
                }
            }
        }

        private static Order BuildOrder(
            IFixture fixture,
            OrderItem[] orderItems,
            OrderSublocation[] orderSublocations,
            Organisation organisation,
            int id = 0)
        {
            return fixture.Build<Order>()
                .With(o => o.OrderingParty, organisation)
                .With(o => o.OrderSublocations, new List<OrderSublocation>(orderSublocations))
                .With(o => o.Revision, 1)
                .With(o => o.OrderItems, new HashSet<OrderItem>(orderItems))
                .With(o => o.OrderNumber, new Random().Next(0, 999999))
                .With(o => o.Id, 0)
                .Create();
        }

        private static OrderItem BuildOrderItem(
            IFixture fixture,
            CatalogueItem catalogueItem,
            OrderItemFundingType? fundingType,
            CataloguePriceQuantityCalculationType cataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient)
        {
            IPrice itemPrice = fixture.Build<OrderItemPrice>()
                .Without(p => p.OrderItem)
                .With(p => p.OrderItemPriceTiers, [])
                .With(p => p.CataloguePriceQuantityCalculationType, cataloguePriceQuantityCalculationType)
                .Create();

            var funding = fundingType.HasValue
                ? fixture.Build<OrderItemFunding>()
                .Without(p => p.OrderItem)
                .With(f => f.OrderItemFundingType, fundingType)
                .Create()
                : null;

            var orderItem = fixture.Build<OrderItem>()
                .Without(i => i.Order)
                .With(i => i.CatalogueItem, catalogueItem)
                .With(i => i.CatalogueItemId, catalogueItem.Id)
                .With(i => i.OrderItemPrice, itemPrice)
                .With(i => i.OrderItemFunding, funding)
                .Create();

            return orderItem;
        }
    }
}
