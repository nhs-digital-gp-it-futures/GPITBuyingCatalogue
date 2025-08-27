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
        public static void Constructor_Requires_OrderItem(
            CallOffId callOffId,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            FluentActions.Invoking(() => new AmendOrderItemModel(
                    callOffId,
                    OrderTypeEnum.Solution,
                    [],
                    null,
                    null,
                    null,
                    fundingTypeDescription))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Theory]
        [MockAutoData]
        public static void IsOrderItemAdded_True_When_Previous_OrderItem_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(
                callOffId,
                OrderTypeEnum.Solution,
                [],
                null,
                orderItem,
                null,
                fundingTypeDescription);

            model.IsOrderItemAdded.Should().BeTrue();
        }

        [Theory]
        [MockAutoData]
        public static void PreviousTotalQuantity_0_When_Previous_OrderItem_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(
                callOffId,
                OrderTypeEnum.Solution,
                [],
                null,
                orderItem,
                null,
                fundingTypeDescription);
            model.PreviousTotalQuantity.Should().Be(0);
        }

        [Theory]
        [MockAutoData]
        public static void IsOrderItemAdded_False_When_Previous_OrderItem_NotNull(
            CallOffId callOffId,
            OrderItem orderItem,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(
                callOffId,
                OrderTypeEnum.Solution,
                [],
                null,
                orderItem,
                orderItem,
                fundingTypeDescription);
            model.IsOrderItemAdded.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void IsServiceRecipientAdded_True_When_Previous_Recipients_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            OrderSublocationRecipient[] recipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var amendmentCallOffId = new CallOffId(callOffId.OrderNumber, 2);
            recipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Now));

            var model = new AmendOrderItemModel(amendmentCallOffId, OrderTypeEnum.Solution, recipients, null, orderItem, orderItem, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.RecipientOdsCode).Should().BeTrue());
        }

        [Theory]
        [MockAutoData]
        public static void IsServiceRecipientAdded_True_When_Previous_Recipients_Does_Not_Include_Recipient(
            CallOffId callOffId,
            OrderItem orderItem,
            OrderSublocationRecipient[] recipients,
            OrderSublocationRecipient[] previousRecipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var amendmentCallOffId = new CallOffId(callOffId.OrderNumber, 2);
            recipients.ForEach(r => r.SetDeliveryDateForItem(orderItem.CatalogueItemId, DateTime.Now));

            var model = new AmendOrderItemModel(amendmentCallOffId, OrderTypeEnum.Solution, recipients, previousRecipients, orderItem, orderItem, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.RecipientOdsCode).Should().BeTrue());
            previousRecipients.ForEach(x => model.IsServiceRecipientAdded(x.RecipientOdsCode).Should().BeFalse());
        }

        [Theory]
        [MockAutoData]
        public static void IsServiceRecipientAdded_False_When_Previous_Recipients_Same(
            CallOffId callOffId,
            OrderItem orderItem,
            OrderSublocationRecipient[] recipients,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var amendmentCallOffId = new CallOffId(callOffId.OrderNumber, 2);
            var model = new AmendOrderItemModel(amendmentCallOffId, OrderTypeEnum.Solution, recipients, recipients, orderItem, orderItem, fundingTypeDescription);
            recipients.ForEach(x => model.IsServiceRecipientAdded(x.RecipientOdsCode).Should().BeFalse());
        }

        [Theory]
        [MockAutoData]
        public static void PropertiesCorrectlySet(
            CallOffId callOffId,
            OrderItem orderItem,
            OrderItem previousOrderItem,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(
                callOffId,
                OrderTypeEnum.Solution,
                [],
                null,
                orderItem,
                previousOrderItem,
                fundingTypeDescription);

            model.CallOffId.Should().Be(callOffId);
            model.IsAmendment.Should().Be(callOffId.IsAmendment);
            model.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            model.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            model.RolledUpRecipientsForItem.Should().BeEquivalentTo(Array.Empty<OrderSublocationRecipient>());
            model.RolledUpTotalQuantity.Should().Be(orderItem.TotalQuantity(null));
            model.PreviousTotalQuantity.Should().Be(previousOrderItem.TotalQuantity(null));
            model.FundingTypeDescription.Should().Be(fundingTypeDescription.Value(orderItem.CatalogueItem.CatalogueItemType.DisplayName()));
        }
    }
}
