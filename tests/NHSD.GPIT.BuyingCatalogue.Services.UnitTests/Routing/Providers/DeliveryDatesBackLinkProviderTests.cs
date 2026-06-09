using System;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class DeliveryDatesBackLinkProviderTests
    {
        [Theory]
        [MockAutoData]
        public void Process_OrderWrapperIsNull_ThrowsException(
            RouteValues routeValues,
            DeliveryDatesBackLinkProvider provider)
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
            DeliveryDatesBackLinkProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(new OrderWrapper(order), null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [MockAutoData]
        public void Process_RouteValuesCatalogueItemIdIsNull_ThrowsException(
            Order order,
            RouteValues routeValues,
            DeliveryDatesBackLinkProvider provider)
        {
            routeValues.OrderItemId = null;

            FluentActions
                .Invoking(() => provider.Process(new OrderWrapper(order), routeValues))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [MockAutoData]
        public void Process_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            int orderItemId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
        {
            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, catalogueItemId)
            {
                Source = RoutingSource.TaskList,
                OrderItemId = orderItemId,
            });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.ReviewDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public void Process_NoPreviousOrderItem_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
        {
            order.SetupCatalogueSolution();

            var orderItem = order.OrderItems.First();
            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { OrderItemId = orderItem.Id });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectDeliveryDate);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public void Process_PreviousItemExists_AssociatedServicesOnly_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
        {
            var orderItem = order.OrderItems.ElementAt(1);
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.ElementAt(0).CatalogueItem.Name = "A";
            orderItem.CatalogueItem.Name = "B";
            order.OrderItems.ElementAt(2).CatalogueItem.Name = "C";

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { OrderItemId = orderItem.Id });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                orderItemId = order.OrderItems.ElementAt(0).Id,
            };

            result.ActionName.Should().Be(Constants.Actions.EditDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public void Process_PreviousItemExists_SolutionMatchesPrimaryDeliveryDate_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
        {
            var deliveryDate = DateTime.Today;

            order.SetupCatalogueSolution();
            order.DeliveryDate = deliveryDate;

            var solution = order.OrderItems.First();
            var orderItem = order.OrderItems.ElementAt(1);

            order.FlattenedRecipients.ForEach(r => r.SetDeliveryDateForItem(solution, deliveryDate));

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { OrderItemId = orderItem.Id });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                orderItemId = solution.Id,
            };

            result.ActionName.Should().Be(Constants.Actions.EditDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public void Process_PreviousItemExists_SolutionDoesNotMatchPrimaryDeliveryDate_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
        {
            var deliveryDate = DateTime.Today;

            order.SetupCatalogueSolution();
            order.DeliveryDate = deliveryDate;

            var solution = order.OrderItems.First();
            order.FlattenedRecipients.ForEach(r => r.SetDeliveryDateForItem(
                solution,
                deliveryDate.AddDays(1)));

            var orderItem = order.OrderItems.ElementAt(1);

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { OrderItemId = orderItem.Id });

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                orderItemId = orderItem.Id,
            };

            result.ActionName.Should().Be(Constants.Actions.MatchDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
