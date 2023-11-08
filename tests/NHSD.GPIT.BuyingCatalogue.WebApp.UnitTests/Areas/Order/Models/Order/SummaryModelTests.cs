using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class SummaryModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            bool hasSubsequentRevisions,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            model.InternalOrgId.Should().Be(internalOrgId);
            model.Order.Should().BeEquivalentTo(order);
            model.HasSubsequentRevisions.Should().Be(hasSubsequentRevisions);
            model.CanBeTerminated.Should().Be(order.OrderStatus == OrderStatus.Completed && !hasSubsequentRevisions);
            model.CanBeAmended.Should().Be(!order.AssociatedServicesOnly && order.OrderStatus == OrderStatus.Completed && !hasSubsequentRevisions && !order.ContractExpired);
        }

        [Theory]
        [CommonAutoData]
        public static void FundingTypeDescription_PropertiesCorrectlySet(
            string internalOrgId,
            bool hasSubsequentRevisions,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId catalogueItemId)
        {
            var model = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, new ImplementationPlan());

            var result = model.FundingTypeDescription(catalogueItemId);
            var actual = result.Should().BeOfType<FundingTypeDescriptionModel>().Subject;
            actual.Should().NotBeNull();
        }
    }
}
