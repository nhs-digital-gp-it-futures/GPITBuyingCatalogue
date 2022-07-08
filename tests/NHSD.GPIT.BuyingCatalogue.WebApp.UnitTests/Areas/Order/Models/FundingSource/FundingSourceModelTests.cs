﻿using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            EntityFramework.Ordering.Models.Order order)
        {
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemFunding = null;

            var model = new WebApp.Areas.Order.Models.FundingSources.FundingSource(internalOrgId, callOffId, order, orderItem);

            model.Title.Should().Be("Funding source");
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be(orderItem.CatalogueItem.Name);
            model.SelectedFundingType.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [CommonAutoData]
        public static void FundingSource_WithArguments_FundingSet_SetsCorrectly(
            string internalOrgId,
            [Frozen] CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order)
        {
            var orderItem = order.OrderItems.First();

            var model = new WebApp.Areas.Order.Models.FundingSources.FundingSource(internalOrgId, callOffId, order, orderItem);

            model.Title.Should().Be("Funding source");
            model.CallOffId.Should().Be(callOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be(orderItem.CatalogueItem.Name);
            model.SelectedFundingType.Should().Be(orderItem.FundingType);
        }
    }
}
