using System;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class AmendOrderItemModelTest
    {
        [Theory]
        [MockAutoData]
        public static void Contructor_Requires_OrderItem(
            CallOffId callOffId,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            FluentActions.Invoking(() => new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, Array.Empty<OrderRecipient>(), null, null, null, isAmendment, fundingTypeDescription))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void IsOrderItemAdded_True_When_Previous_OrderItem_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, Array.Empty<OrderRecipient>(), null, orderItem, null, isAmendment, fundingTypeDescription);

            model.IsOrderItemAdded.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void PreviousTotalQuantity_0_When_Previous_OrderItem_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, Array.Empty<OrderRecipient>(), null, orderItem, null, isAmendment, fundingTypeDescription);
            model.PreviousTotalQuantity.Should().Be(0);
        }

        [Theory]
        [MockAutoData]
        public static void IsOrderItemAdded_False_When_Previous_OrderItem_NotNull(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, Array.Empty<OrderRecipient>(), null, orderItem, orderItem, isAmendment, fundingTypeDescription);
            model.IsOrderItemAdded.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void IsServiceRecipientAdded_True_When_Previous_Recipients_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            OrderRecipient[] recipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            recipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Now));

            var model = new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, recipients, null, orderItem, orderItem, isAmendment, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeTrue());
        }

        [Theory]
        [MockAutoData]
        public static void IsServiceRecipientAdded_True_When_Previous_Recipients_Does_Not_Include_Recipient(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            OrderRecipient[] recipients,
            OrderRecipient[] previousRecipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            recipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Now));

            var model = new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, recipients, previousRecipients, orderItem, orderItem, isAmendment, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeTrue());
            previousRecipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeFalse());
        }

        [Theory]
        [MockAutoData]
        public static void IsServiceRecipientAdded_False_When_Previous_Recipients_Same(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            OrderRecipient[] recipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, recipients, recipients, orderItem, orderItem, isAmendment, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.OdsCode).Should().BeFalse());
        }

        [Theory]
        [MockAutoData]
        public static void PropertiesCorrectlySet(
            CallOffId callOffId,
            OrderItem orderItem,
            OrderItem previousOrderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, OrderTypeEnum.Solution, Array.Empty<OrderRecipient>(), null, orderItem, previousOrderItem, isAmendment, fundingTypeDescription);

            model.CallOffId.Should().Be(callOffId);
            model.IsAmendment.Should().Be(isAmendment);
            model.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            model.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            model.RolledUpRecipientsForItem.Should().BeEquivalentTo(Array.Empty<OrderRecipient>());
            model.RolledUpTotalQuantity.Should().Be(orderItem.TotalQuantity(null));
            model.PreviousTotalQuantity.Should().Be(previousOrderItem.TotalQuantity(null));
            model.FundingTypeDescription.Should().Be(fundingTypeDescription.Value(orderItem.CatalogueItem.CatalogueItemType.DisplayName()));
        }
    }
}
