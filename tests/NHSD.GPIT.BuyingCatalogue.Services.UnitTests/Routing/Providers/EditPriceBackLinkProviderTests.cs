using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class EditPriceBackLinkProviderTests
    {
        [Theory]
        [MockAutoData]
        public void Process_OrderWrapperIsNull_ThrowsException(
            string internalOrgId,
            CallOffId callOffId,
            EditPriceBackLinkProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(null, new RouteValues(internalOrgId, callOffId)))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("orderWrapper");
        }

        [Theory]
        [MockAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            Order order,
            EditPriceBackLinkProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(new OrderWrapper(order), null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [MockAutoData]
        public void Process_MultiplePrices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            EditPriceBackLinkProvider provider)
        {
            var orderItem = order.OrderItems.First();

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                orderItem.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectPrice);
            result.ControllerName.Should().Be(Constants.Controllers.Prices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public void Process_SinglePrice_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            CatalogueItemId catalogueItemId,
            EditPriceBackLinkProvider provider)
        {
            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, catalogueItemId)
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
        [MockAutoData]
        public void Process_FromManageAssociatedServices_ExpectedResult(
            string internalOrgId,
            Order order,
            OrderItem solution,
            CallOffId callOffId,
            EditPriceBackLinkProvider provider)
        {
            var associatedService = order.OrderItems.First();
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedService.Parent = solution;
            associatedService.CatalogueItem.CataloguePrices = new List<CataloguePrice>
            {
                associatedService.CatalogueItem.CataloguePrices.First(),
            };

            order.OrderItems.Add(solution);

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, associatedService.CatalogueItemId)
            {
                Source = RoutingSource.ManageAssociatedServices,
                OrderItemId = associatedService.Id,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                catalogueItemId = solution.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.ManageAssociatedServices);
            result.ControllerName.Should().Be(Constants.Controllers.AssociatedServices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
