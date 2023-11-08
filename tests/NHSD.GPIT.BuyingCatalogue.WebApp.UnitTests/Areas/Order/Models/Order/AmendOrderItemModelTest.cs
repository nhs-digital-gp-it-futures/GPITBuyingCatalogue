using System;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class AmendOrderItemModelTest
    {
        [Theory]
        [CommonAutoData]
        public static void IsOrderItemAdded_True_When_Previous_OrderItem_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, System.Array.Empty<OrderRecipient>(), null, orderItem, null, isAmendment, fundingTypeDescription);
            model.IsOrderItemAdded.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void IsOrderItemAdded_False_When_Previous_OrderItem_NotNull(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, System.Array.Empty<OrderRecipient>(), null, orderItem, orderItem, isAmendment, fundingTypeDescription);
            model.IsOrderItemAdded.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsServiceRecipientAdded_True_When_Previous_Recipients_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            OrderRecipient[] recipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            recipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Now));

            var model = new AmendOrderItemModel(callOffId, recipients, null, orderItem, orderItem, isAmendment, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeTrue());
        }

        [Theory]
        [CommonAutoData]
        public static void IsServiceRecipientAdded_False_When_Previous_Recipients_Same(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            OrderRecipient[] recipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, recipients, recipients, orderItem, orderItem, isAmendment, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeFalse());
        }

        [Theory]
        [CommonAutoData]
        public static void PropertiesCorrectlySet(
            CallOffId callOffId,
            OrderItem orderItem,
            OrderItem previousOrderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, System.Array.Empty<OrderRecipient>(), null, orderItem, previousOrderItem, isAmendment, fundingTypeDescription);

            model.CallOffId.Should().Be(callOffId);
            model.IsAmendment.Should().Be(isAmendment);
            model.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            model.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            model.RolledUpRecipientsForItem.Should().BeEquivalentTo(System.Array.Empty<OrderRecipient>());
            model.RolledUpTotalQuantity.Should().Be(orderItem.TotalQuantity(null));
            model.PreviousTotalQuantity.Should().Be(previousOrderItem.TotalQuantity(null));
            model.FundingTypeDescription.Should().Be(fundingTypeDescription.Value(orderItem.CatalogueItem.CatalogueItemType.DisplayName()));
        }
    }
}
