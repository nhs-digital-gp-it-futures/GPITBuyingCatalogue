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
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem])]),
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
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem])]),
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
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem])]),
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
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem])]),
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
                        BuildOrderSublocationRecipient(fixture, "XXXX", [amendedOrderItem]),
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
                        [BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem])]),
                    BuildOrderSublocation(
                        fixture,
                        "XXXY",
                        [BuildOrderSublocationRecipient(fixture, "XXXY", [orderItem])]),
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
                        BuildOrderSublocationRecipient(fixture, "XXXX", [amendedOrderItem]),
                    ]),
                BuildOrderSublocation(
                    fixture,
                    "XXXY",
                    [
                        BuildOrderSublocationRecipient(fixture, "XXXX", [amendedOrderItem]),
                    ]),
                BuildOrderSublocation(
                    fixture,
                    "XXXZ",
                    [
                        BuildOrderSublocationRecipient(fixture, "XXXX", [amendedOrderItem]),
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
        public static void GetCallOffIdForPreviousRecipient_Returns_ExpectedCallOffId(
            IFixture fixture,
            OrderItem orderItem1,
            OrderItem orderItem2,
            Organisation organisation)
        {
            var sublocationRecipient = BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem1]);
            var addedRecipient = BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem1, orderItem2]);
            var finalRecipient = BuildOrderSublocationRecipient(fixture, "XXXX", [orderItem1, orderItem2]);
            orderItem1.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            orderItem2.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            OrderItem orderItem = BuildOrderItem(fixture, orderItem1.CatalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, orderItem2.CatalogueItem, OrderItemFundingType.LocalFunding);

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
            orderItem.OrderId = order.Id;
            Order amendedOrder = order.BuildAmendment(2);
            amendedOrder.Id = 1;
            amendedOrderItem.OrderId = amendedOrder.Id;
            amendedOrder.OrderItems = [orderItem, amendedOrderItem];
            amendedOrder.OrderSublocations =
            [
                BuildOrderSublocation(
                    fixture,
                    "XXXX",
                    [sublocationRecipient, addedRecipient]),
            ];
            Order finalAmendedOrder = amendedOrder.BuildAmendment(3);
            finalAmendedOrder.Id = 3;
            finalAmendedOrder.OrderItems = [orderItem, amendedOrderItem];
            finalAmendedOrder.OrderSublocations =
            [
                BuildOrderSublocation(
                    fixture,
                    "XXXX",
                    [sublocationRecipient, addedRecipient, finalRecipient]),
            ];

            var orderWrapper = new OrderWrapper(finalAmendedOrder, [order, amendedOrder]);

            var callOffIdForInitialRecipient = orderWrapper.GetCallOffIdForPreviousRecipient(sublocationRecipient, order.Id);
            var callOffIdForAddedRecipient = orderWrapper.GetCallOffIdForPreviousRecipient(
                    addedRecipient,
                    amendedOrder.OrderItems.FirstOrDefault(oi => oi.CatalogueItemId == orderItem2.CatalogueItemId)?.OrderId);

            callOffIdForInitialRecipient.Should().Be(order.CallOffId.ToString());
            callOffIdForInitialRecipient.Should().NotBe(amendedOrder.CallOffId.ToString());

            callOffIdForAddedRecipient.Should().Be(amendedOrder.CallOffId.ToString());
            callOffIdForAddedRecipient.Should().NotBe(order.CallOffId.ToString());
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
            OrderItem[] orderItems = null,
            int id = 0)
        {
            OrderSublocationRecipient recipient = fixture.Build<OrderSublocationRecipient>()
                .Without(r => r.OrderItemSublocationRecipients)
                .With(r => r.ParentSublocationOdsCode, sublocationOdsCode)
                .With(r => r.OrderId, id)
                .Create();

            UpdateRecipientToItem(recipient, orderItems);

            return recipient;
        }

        private static void UpdateRecipientToItem(
            OrderSublocationRecipient recipient,
            OrderItem[] orderItems)
        {
            if (orderItems != null)
            {
                foreach (var orderItem in orderItems)
                {
                    recipient.SetQuantityForItem(orderItem, 1);
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
