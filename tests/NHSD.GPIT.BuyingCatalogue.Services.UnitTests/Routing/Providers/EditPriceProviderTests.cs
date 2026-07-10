using System;
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
    public class EditPriceProviderTests
    {
        [Theory]
        [MockAutoData]
        public void Process_OrderWrapperIsNull_ThrowsException(
            RouteValues routeValues,
            EditPriceProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(null, routeValues))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("orderWrapper");
        }

        [Theory]
        [MockAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            Order order,
            EditPriceProvider provider)
        {
            var orderWrapper = new OrderWrapper(order);

            FluentActions
                .Invoking(() => provider.Process(new OrderWrapper(order), null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");

            var routeValues = new RouteValues
            {
                CatalogueItemId = null,
            };

            FluentActions
                .Invoking(() => provider.Process(orderWrapper, routeValues))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [MockAutoData]
        public void Process_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            EditPriceProvider provider)
        {
            var catalogueItemId = order.OrderItems.First().CatalogueItemId;
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
            OrderItem parent,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EditPriceProvider provider)
        {
            var associatedService = order.OrderItems.First();
            associatedService.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
            associatedService.Parent = parent;
            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, catalogueItemId)
            {
                Source = RoutingSource.ManageAssociatedServices,
                OrderItemId = associatedService.Id,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                catalogueItemId = parent.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.ManageAssociatedServices);
            result.ControllerName.Should().Be(Constants.Controllers.AssociatedServices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
