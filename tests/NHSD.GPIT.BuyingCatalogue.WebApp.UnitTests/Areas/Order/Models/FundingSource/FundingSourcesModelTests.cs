using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class FundingSourcesModelTests
    {
        [Theory]
        [InMemoryDbAutoData]
        public static void Constructor_NullOrder_ThrowsException(
            string internalOrgId,
            CallOffId id)
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new FundingSources(internalOrgId, id, null, 1));

            actual.ParamName.Should().Be("order");
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolution_LocalFundingOnly_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().CatalogueItem.Solution = solution;
            order.OrderItems.First().OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;

            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).ToList();

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.Local };

            var model = new FundingSources(internalOrgId, order.CallOffId, order, 1);

            model.Title.Should().Be("Funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeNull();
            model.CountOfOrderFrameworks.Should().Be(1);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolutionAndAdditionalService_LocalFundingOnly_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderingParty.OrganisationType = OrganisationType.IB;
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != CatalogueItemType.Solution).ToList().ForEach(oi => oi.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ToList().ForEach(oi => oi.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly);
            order.OrderItems.First().CatalogueItem.Solution = solution;

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.Local };

            var model = new FundingSources(internalOrgId, order.CallOffId, order, 1);

            model.Title.Should().Be("Funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeNull();
            model.CountOfOrderFrameworks.Should().Be(1);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolutionAndAdditionalService_GPPractice_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderingParty.OrganisationType = OrganisationType.GP;
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != CatalogueItemType.Solution).ToList().ForEach(oi => oi.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ToList().ForEach(oi => oi.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.MixedFunding);
            order.OrderItems.First().CatalogueItem.Solution = solution;

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.Local };

            var model = new FundingSources(internalOrgId, order.CallOffId, order, 1);

            model.Title.Should().Be("Funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeNull();
            model.CountOfOrderFrameworks.Should().Be(1);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolution_MixedFundingFrameworks_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.GPIT };

            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().CatalogueItem.Solution = solution;
            order.OrderItems.First().OrderItemFunding.OrderItemFundingType = OrderItemFundingType.MixedFunding;

            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).ToList();

            var model = new FundingSources(internalOrgId, order.CallOffId, order, 1);

            model.Title.Should().Be("Funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().BeNull();
            model.OrderItemsSelectable.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.CountOfOrderFrameworks.Should().Be(1);
        }
    }
}
