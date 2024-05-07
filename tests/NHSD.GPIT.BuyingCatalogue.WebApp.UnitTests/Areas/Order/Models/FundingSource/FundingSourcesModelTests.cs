using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class FundingSourcesModelTests
    {
        [Theory]
        [MockInMemoryDbAutoData]
        public static void Constructor_NullOrderWrapper_ThrowsException(
            string internalOrgId,
            CallOffId id)
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new FundingSources(internalOrgId, id, null));

            actual.ParamName.Should().Be("orderWrapper");
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static void Constructor_NullOrder_ThrowsException(
            string internalOrgId,
            CallOffId id)
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new FundingSources(internalOrgId, id, new OrderWrapper()));

            actual.ParamName.Should().Be("orderWrapper");
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_CatalogueSolution_SingleFundingType_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().CatalogueItem.Solution = solution;

            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).ToList();

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.LocalFunding };

            var orderWrapper = new OrderWrapper(order);
            var model = new FundingSources(internalOrgId, order.CallOffId, orderWrapper);

            model.Title.Should().Be("Funding source");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsSingleFundingType.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_CatalogueSolution_GpPRactice_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderingParty.OrganisationType = OrganisationType.GP;

            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().CatalogueItem.Solution = solution;

            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).ToList();

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.LocalFunding, FundingType.Gpit };
            var orderWrapper = new OrderWrapper(order);

            var model = new FundingSources(internalOrgId, order.CallOffId, orderWrapper);

            model.Title.Should().Be("Funding source");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsSingleFundingType.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_CatalogueSolutionAndAdditionalService_SingleFundingType_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != CatalogueItemType.Solution).ToList().ForEach(oi => oi.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ToList().ForEach(oi => oi.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.Gpit);
            order.OrderItems.First().CatalogueItem.Solution = solution;

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.Gpit };
            var orderWrapper = new OrderWrapper(order);

            var model = new FundingSources(internalOrgId, order.CallOffId, orderWrapper);

            model.Title.Should().Be("Funding source");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsSingleFundingType.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_CatalogueSolutionAndAdditionalService_GPPractice_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderingParty.OrganisationType = OrganisationType.GP;
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != CatalogueItemType.Solution).ToList().ForEach(oi => oi.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ToList().ForEach(oi => oi.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.Gpit);
            order.OrderItems.First().CatalogueItem.Solution = solution;

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.LocalFunding, FundingType.Gpit };
            var orderWrapper = new OrderWrapper(order);

            var model = new FundingSources(internalOrgId, order.CallOffId, orderWrapper);

            model.Title.Should().Be("Funding source");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsSingleFundingType.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void WithValidArguments_CatalogueSolution_MultipleFundingFrameworks_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().CatalogueItem.Solution = solution;
            order.OrderItems.First().OrderItemFunding.OrderItemFundingType = OrderItemFundingType.Gpit;

            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).ToList();
            var orderWrapper = new OrderWrapper(order);

            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.LocalFunding, FundingType.Gpit };

            var model = new FundingSources(internalOrgId, order.CallOffId, orderWrapper);

            model.Title.Should().Be("Funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsSingleFundingType.Should().BeNull();
            model.OrderItemsSelectable.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
        }
    }
}
