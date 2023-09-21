using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using LinqKit;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
        public static void OrderWrapper_FundingTypesForItem_Returnds_None_When_No_Funding(CatalogueItem catalogueItem, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, null);

            Order order = BuildOrder(fixture, new[] { orderItem }, new[] { BuildOrderRecipient(fixture, new[] { orderItem.CatalogueItemId }) });

            var orderWrapper = new OrderWrapper(new[] { order });

            var result = orderWrapper.FundingTypesForItem(catalogueItem.Id);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(new OrderItemFundingType[] { OrderItemFundingType.None });
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_FundingTypesForItem_Returnds_Multiple(CatalogueItem catalogueItem, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(fixture, new[] { orderItem }, new[] { BuildOrderRecipient(fixture, new[] { orderItem.CatalogueItemId }) });
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = new HashSet<OrderItem> { amendedOrderItem, };

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });

            var result = orderWrapper.FundingTypesForItem(catalogueItem.Id);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(new OrderItemFundingType[] { OrderItemFundingType.LocalFunding, OrderItemFundingType.MixedFunding });
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_FundingTypesForItem_Distinct_Result(CatalogueItem catalogueItem, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);

            Order order = BuildOrder(fixture, new[] { orderItem }, new[] { BuildOrderRecipient(fixture, new[] { orderItem.CatalogueItemId }) });
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = new HashSet<OrderItem> { amendedOrderItem };

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });

            var result = orderWrapper.FundingTypesForItem(catalogueItem.Id);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(new OrderItemFundingType[] { OrderItemFundingType.LocalFunding });
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_RolledUp_Uses_Old_OrderItem_Data(CatalogueItem catalogueItem, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(fixture, new[] { orderItem }, new[] { BuildOrderRecipient(fixture, new[] { orderItem.CatalogueItemId }) });
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = new HashSet<OrderItem> { amendedOrderItem };
            amendedOrder.OrderRecipients = new HashSet<OrderRecipient> { BuildOrderRecipient(fixture, new[] { catalogueItem.Id }) };

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });
            var rolledUp = orderWrapper.RolledUp;
            Console.WriteLine(rolledUp);

            orderWrapper.Previous.OrderItems.Count.Should().Be(1);
            orderWrapper.Previous.OrderRecipients.Count.Should().Be(1);
            orderWrapper.Previous.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);

            orderWrapper.Order.OrderItems.Count.Should().Be(1);
            orderWrapper.Order.OrderRecipients.Count.Should().Be(1);
            orderWrapper.Order.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.MixedFunding);

            orderWrapper.RolledUp.OrderItems.Count.Should().Be(1);
            orderWrapper.RolledUp.OrderRecipients.Count.Should().Be(2);
            orderWrapper.RolledUp.OrderItems.First().FundingType.Should().Be(OrderItemFundingType.LocalFunding);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_RolledUp_New_Recipients_For_Existing_Order_Item(CatalogueItem catalogueItem, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, catalogueItem, OrderItemFundingType.LocalFunding);

            Order order = BuildOrder(fixture, new[] { orderItem }, new[] { BuildOrderRecipient(fixture, new[] { orderItem.CatalogueItemId }) });
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderRecipients.Add(BuildOrderRecipient(fixture, new[] { catalogueItem.Id }));

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });
            var rolledUp = orderWrapper.RolledUp;
            Console.WriteLine(rolledUp);

            orderWrapper.Previous.OrderItems.Count.Should().Be(1);
            orderWrapper.Previous.OrderRecipients.Count.Should().Be(1);

            orderWrapper.Order.OrderItems.Count.Should().Be(1);
            orderWrapper.Order.OrderRecipients.Count.Should().Be(2);
            orderWrapper.AddedOrderRecipients().Count.Should().Be(1);
            orderWrapper.DetermineOrderRecipients(catalogueItem.Id).Count.Should().Be(1);

            orderWrapper.RolledUp.OrderItems.Count.Should().Be(1);
            orderWrapper.RolledUp.OrderRecipients.Count.Should().Be(2);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_RolledUp_New_Recipients_For_New_Order_Item(CatalogueItem originalCatalogueItem, CatalogueItem addedCatalogueItem, IFixture fixture)
        {
            OrderItem orderItem = BuildOrderItem(fixture, originalCatalogueItem, OrderItemFundingType.LocalFunding);
            OrderItem amendedOrderItem = BuildOrderItem(fixture, addedCatalogueItem, OrderItemFundingType.MixedFunding);

            Order order = BuildOrder(fixture, new[] { orderItem }, new[] { BuildOrderRecipient(fixture, new[] { originalCatalogueItem.Id }) });
            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems.Add(amendedOrderItem);
            amendedOrder.OrderRecipients.Add(BuildOrderRecipient(fixture, new[] { addedCatalogueItem.Id }));

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });

            orderWrapper.Previous.OrderItems.Count.Should().Be(1);
            orderWrapper.Previous.OrderRecipients.Count.Should().Be(1);

            orderWrapper.Order.OrderItems.Count.Should().Be(2);
            orderWrapper.Order.OrderRecipients.Count.Should().Be(2);
            orderWrapper.AddedOrderRecipients().Count.Should().Be(1);
            orderWrapper.DetermineOrderRecipients(originalCatalogueItem.Id).Count.Should().Be(1);
            orderWrapper.DetermineOrderRecipients(addedCatalogueItem.Id).Count.Should().Be(2);

            orderWrapper.RolledUp.OrderItems.Count.Should().Be(2);
            orderWrapper.RolledUp.OrderRecipients.Count.Should().Be(2);
        }

        private static OrderRecipient BuildOrderRecipient(IFixture fixture, CatalogueItemId[] catalogueItemIds = null)
        {
            var recipient = fixture.Build<OrderRecipient>()
                .Without(r => r.OrderItemRecipients)
                .Create();

            UpdateRecipientToItem(recipient, catalogueItemIds);

            return recipient;
        }

        private static void UpdateRecipientToItem(OrderRecipient recipient, CatalogueItemId[] catalogueItemIds)
        {
            if (catalogueItemIds != null)
            {
                foreach (var catalogueItemId in catalogueItemIds)
                {
                    recipient.SetQuantityForItem(catalogueItemId, 1);
                }
            }
        }

        private static Order BuildOrder(IFixture fixture, OrderItem[] orderItems, OrderRecipient[] recipients)
        {
            return fixture.Build<Order>()
                .With(o => o.OrderRecipients, new HashSet<OrderRecipient>(recipients))
                .With(o => o.Revision, 1)
                .With(o => o.OrderItems, new HashSet<OrderItem>(orderItems))
                .Create();
        }

        private static OrderItem BuildOrderItem(
            IFixture fixture,
            CatalogueItem catalogueItem,
            OrderItemFundingType? fundingType,
            CataloguePriceQuantityCalculationType cataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient)
        {
            var itemPrice = fixture.Build<OrderItemPrice>()
                .Without(p => p.OrderItem)
                .With(p => p.OrderItemPriceTiers, new HashSet<OrderItemPriceTier>())
                .With(p => p.CataloguePriceQuantityCalculationType, cataloguePriceQuantityCalculationType)
                .Create() as IPrice;

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
