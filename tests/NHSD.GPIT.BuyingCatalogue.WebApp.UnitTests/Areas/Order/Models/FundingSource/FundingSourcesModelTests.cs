using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.FundingSource
{
    public static class FundingSourcesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_AssociatedServicesOnly_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService).ToList();

            var model = new FundingSources(internalOrgId, order.CallOffId, order);

            model.Title.Should().Be("Select funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().BeEmpty();
            model.OrderItemsSelectable.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
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

            var model = new FundingSources(internalOrgId, order.CallOffId, order);

            model.Title.Should().Be("Select funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolutionAndAdditionalService_LocalFundingOnly_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != CatalogueItemType.Solution).ToList().ForEach(oi => oi.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ToList().ForEach(oi => oi.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly);
            order.OrderItems.First().CatalogueItem.Solution = solution;

            var model = new FundingSources(internalOrgId, order.CallOffId, order);

            model.Title.Should().Be("Select funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
            model.OrderItemsSelectable.Should().BeEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_CatalogueSolution_MixedFundingFrameworks_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            solution.FrameworkSolutions.First().Framework.LocalFundingOnly = true;

            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().CatalogueItem.Solution = solution;
            order.OrderItems.First().OrderItemFunding.OrderItemFundingType = OrderItemFundingType.MixedFunding;

            order.OrderItems = order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution).ToList();

            var model = new FundingSources(internalOrgId, order.CallOffId, order);

            model.Title.Should().Be("Select funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().BeEmpty();
            model.OrderItemsSelectable.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count);
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_MixedSolutionTypes_LocalOnly_SetsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Solution solution)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;
            order.OrderItems.Where(oi => oi.CatalogueItem.CatalogueItemType != CatalogueItemType.Solution).ToList().ForEach(oi => oi.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.First().CatalogueItem.Solution = solution;

            var model = new FundingSources(internalOrgId, order.CallOffId, order);

            model.Title.Should().Be("Select funding sources");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Caption.Should().Be($"Order {order.CallOffId}");
            model.OrderItemsLocalOnly.Should().NotBeEmpty().And.HaveCount(1);
            model.OrderItemsSelectable.Should().NotBeEmpty().And.HaveCount(order.OrderItems.Count - 1);
        }
    }
}
