using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Orders
{
    public static class OrderWrapperTests
    {
        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_RolledUp_Uses_Old_Order_Data_Apart_From_Revision(IFixture fixture)
        {
            var order = fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .Create();

            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.Description = $"Edited-{order.Description}";

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });

            orderWrapper.Previous.Revision.Should().Be(1);
            orderWrapper.Previous.Description.Should().Be(order.Description);
            orderWrapper.Order.Revision.Should().Be(2);
            orderWrapper.Order.Description.Should().Be($"Edited-{order.Description}");
            orderWrapper.RolledUp.Revision.Should().Be(2);
            orderWrapper.RolledUp.Description.Should().Be(order.Description);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_RolledUp_Uses_Old_OrderItem_Data(CatalogueItem catalogueItem, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(fixture, new[] { orderItem });
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = new HashSet<OrderItem> { amendedOrderItem };

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });

            orderWrapper.Previous.OrderItems.Count.Should().Be(1);
            orderWrapper.Previous.OrderItems.First().OrderItemRecipients.Count.Should().Be(1);
            orderWrapper.Previous.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);
            orderWrapper.Order.OrderItems.Count.Should().Be(1);
            orderWrapper.Order.OrderItems.First().OrderItemRecipients.Count.Should().Be(1);
            orderWrapper.Order.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.MixedFunding);
            orderWrapper.RolledUp.OrderItems.Count.Should().Be(1);
            orderWrapper.RolledUp.OrderItems.First().OrderItemRecipients.Count.Should().Be(2);
            orderWrapper.Previous.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);
        }

        private static Order BuildOrder(IFixture fixture, OrderItem[] orderItems)
        {
            return fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderItems, new HashSet<OrderItem>(orderItems))
                .Create();
        }

        private static OrderItem BuildOrderItem(
            IFixture fixture,
            CatalogueItem catalogueItem,
            OrderItemFundingType fundingType)
        {
            var quantity = 1;

            var itemPrice = fixture.Build<OrderItemPrice>()
                .Without(p => p.OrderItem)
                .With(p => p.OrderItemPriceTiers, new HashSet<OrderItemPriceTier>())
                .Create();

            var recipient = fixture.Build<OrderItemRecipient>()
                .With(r => r.Quantity, itemPrice.IsPerServiceRecipient() ? quantity : null)
                .Create();

            var funding = fixture.Build<OrderItemFunding>()
                .Without(p => p.OrderItem)
                .With(f => f.OrderItemFundingType, fundingType)
                .Create();

            var orderItem = fixture.Build<OrderItem>()
                .Without(i => i.OrderItemFunding)
                .With(i => i.CatalogueItem, catalogueItem)
                .With(i => i.CatalogueItemId, catalogueItem.Id)
                .With(i => i.OrderItemPrice, itemPrice)
                .With(i => i.Quantity, itemPrice.IsPerServiceRecipient() ? null : quantity)
                .With(i => i.OrderItemRecipients, new HashSet<OrderItemRecipient> { recipient })
                .With(i => i.OrderItemFunding, funding)
                .Create();

            return orderItem;
        }
    }
}
