using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class OrderItemRecipientRowModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Construct_OrderItemRecipientRowModel(
            OrderSublocationRecipient orderSublocationRecipient,
            AmendOrderItemModel amendOrderItemModel,
            string callOffId)
        {
            var model = new OrderItemRecipientRowModel(
                orderSublocationRecipient,
                amendOrderItemModel,
                callOffId);

            model.ServiceRecipient.Should().Be(orderSublocationRecipient);
            model.IsAmendment.Should().Be(amendOrderItemModel.IsAmendment);
            model.CallOffId.Should().Be(callOffId);
            model.IsServiceRecipientAdded.Should().Be(
                amendOrderItemModel.IsServiceRecipientAdded(orderSublocationRecipient.RecipientOdsCode));
        }

        [Theory]
        [MockAutoData]
        public static void Construct_AssociatedService_UsesMatchingOrderItemId(
            CatalogueItemId catalogueItemId,
            CallOffId callOffId)
        {
            var expectedOrderItem = BuildOrderItem(1, catalogueItemId, CatalogueItemType.AssociatedService);
            var otherOrderItemWithSameCatalogueItem = BuildOrderItem(2, catalogueItemId, CatalogueItemType.AssociatedService);
            var recipient = BuildRecipient([expectedOrderItem, otherOrderItemWithSameCatalogueItem]);

            var amendOrderItemModel = new AmendOrderItemModel(
                new CallOffId(1, 2),
                OrderTypeEnum.Solution,
                [recipient],
                null,
                expectedOrderItem,
                null,
                null);

            var model = new OrderItemRecipientRowModel(recipient, amendOrderItemModel, callOffId.ToString());

            model.OrderItemId.Should().Be(expectedOrderItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static void Construct_NonAssociatedService_UsesMatchingCatalogueItemId(
            CatalogueItemId catalogueItemId,
            CallOffId callOffId)
        {
            var orderItem = BuildOrderItem(10, catalogueItemId, CatalogueItemType.Solution);
            var expectedOrderItem = BuildOrderItem(20, catalogueItemId, CatalogueItemType.Solution);
            var recipient = BuildRecipient([orderItem, expectedOrderItem]);

            var amendOrderItemModel = new AmendOrderItemModel(
                callOffId,
                OrderTypeEnum.Solution,
                [recipient],
                null,
                orderItem,
                null,
                null);

            var model = new OrderItemRecipientRowModel(recipient, amendOrderItemModel, callOffId.ToString());

            model.OrderItemId.Should().Be(expectedOrderItem.Id);
        }

        private static OrderItem BuildOrderItem(
            int id,
            CatalogueItemId catalogueItemId,
            CatalogueItemType catalogueItemType)
        {
            return new OrderItem
            {
                Id = id,
                CatalogueItemId = catalogueItemId,
                CatalogueItem = new CatalogueItem
                {
                    Id = catalogueItemId,
                    CatalogueItemType = catalogueItemType,
                },
            };
        }

        private static OrderSublocationRecipient BuildRecipient(List<OrderItem> orderItems)
        {
            var recipient = new OrderSublocationRecipient
            {
                OrderId = 1,
                ParentSublocationOdsCode = "AAAA",
                RecipientOdsCode = "BBBB",
            };

            orderItems.ForEach(orderItem => recipient.OrderItemSublocationRecipients.Add(
                new OrderItemSublocationRecipient(recipient.OrderId, recipient.RecipientOdsCode, orderItem)));

            return recipient;
        }
    }
}
