using System;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class OrderItemRecipientRowModelTest
    {
        [Theory]
        [MockAutoData]
        public static void Construct_OrderItemRecipientRowModel(
            OrderSublocationRecipient orderSublocationRecipient,
            AmendOrderItemModel amendOrderItemModel,
            string callOffId,
            bool hasServiceRecipientQuantities)
        {
            var model = new OrderItemRecipientRowModel(
                orderSublocationRecipient,
                amendOrderItemModel,
                callOffId,
                hasServiceRecipientQuantities);

            model.ServiceRecipient.Should().Be(orderSublocationRecipient);
            model.IsAmendment.Should().Be(amendOrderItemModel.IsAmendment);
            model.CallOffId.Should().Be(callOffId);
            model.CatalogueItemId.Should().Be(amendOrderItemModel.CatalogueItem.Id);
            model.HasServiceRecipientQuantities.Should().Be(hasServiceRecipientQuantities);
            model.IsServiceRecipientAdded.Should().Be(
                amendOrderItemModel.IsServiceRecipientAdded(orderSublocationRecipient.RecipientOdsCode));
        }
    }
}
