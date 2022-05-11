using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class FundingSourceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void FundingSource_WithArguments_FundingNull_SetsCorrectly(
            string internalOrgId,
            [Frozen] CallOffId callOffId,
            OrderItem orderItem)
        {
            orderItem.OrderItemFunding = null;

            var model = new WebApp.Areas.Order.Models.FundingSources.FundingSource(internalOrgId, callOffId, orderItem);

            model.Title.Should().Be($"{orderItem.CatalogueItem.Name} funding source");
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {callOffId}");
            model.SelectedFundingType.Should().Be(OrderItemFundingType.None);
            model.AmountOfCentralFunding.Should().BeNull();
            model.TotalCost.Should().Be(orderItem.OrderItemPrice.CalculateTotalCost(orderItem.GetTotalRecipientQuantity()));
        }

        [Theory]
        [CommonAutoData]
        public static void FundingSource_WithArguments_FundingSet_SetsCorrectly(
            string internalOrgId,
            [Frozen] CallOffId callOffId,
            OrderItem orderItem)
        {
            var model = new WebApp.Areas.Order.Models.FundingSources.FundingSource(internalOrgId, callOffId, orderItem);

            model.Title.Should().Be($"{orderItem.CatalogueItem.Name} funding source");
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {callOffId}");
            model.SelectedFundingType.Should().Be(orderItem.CurrentFundingType());
            model.AmountOfCentralFunding.Should().Be(orderItem.OrderItemFunding.CentralAllocation);
            model.TotalCost.Should().Be(orderItem.OrderItemPrice.CalculateTotalCost(orderItem.GetTotalRecipientQuantity()));
        }
    }
}
