using FluentAssertions;
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
            var model = new AmendOrderItemModel(callOffId, orderItem, null, isAmendment, fundingTypeDescription);
            model.IsOrderItemAdded.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static void IsOrderItemAdded_False_When_Previous_OrderItem_Null(
            CallOffId callOffId,
            OrderItem orderItem,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            var model = new AmendOrderItemModel(callOffId, orderItem, orderItem, isAmendment, fundingTypeDescription);
            model.IsOrderItemAdded.Should().BeFalse();
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
            var model = new AmendOrderItemModel(callOffId, orderItem, previousOrderItem, isAmendment, fundingTypeDescription);

            model.CallOffId.Should().Be(callOffId);
            model.IsAmendment.Should().Be(isAmendment);
            model.OrderItemPrice.Should().Be(orderItem.OrderItemPrice);
            model.CatalogueItem.Should().Be(orderItem.CatalogueItem);
            model.OrderItemRecipients.Should().BeEquivalentTo(orderItem.OrderItemRecipients);
            model.RolledUpTotalQuantity.Should().Be(orderItem.TotalQuantity);
            model.PreviousTotalQuantity.Should().Be(previousOrderItem.TotalQuantity);
            model.FundingTypeDescription.Should().Be(fundingTypeDescription.Value(orderItem.CatalogueItem.CatalogueItemType.DisplayName()));
        }
    }
}
