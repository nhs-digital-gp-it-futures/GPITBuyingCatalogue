using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class ConfirmPriceBackLinkProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_OrderWrapperIsNull_ThrowsException(
            RouteValues routeValues,
            ConfirmPriceBackLinkProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(null, routeValues))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("orderWrapper");
        }

        [Theory]
        [CommonAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            Order order,
            ConfirmPriceBackLinkProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(new OrderWrapper(order), null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_WithSinglePrice_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            ConfirmPriceBackLinkProvider provider)
        {
            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.CatalogueItem.CataloguePrices = new List<CataloguePrice>
            {
                solution.CatalogueItem.CataloguePrices.First(),
            };

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId)
            {
                Source = RoutingSource.TaskList,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.TaskList);
            result.ControllerName.Should().Be(Constants.Controllers.TaskList);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_WithMultiplePrices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            ConfirmPriceBackLinkProvider provider)
        {
            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                solution.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectPrice);
            result.ControllerName.Should().Be(Constants.Controllers.Prices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
