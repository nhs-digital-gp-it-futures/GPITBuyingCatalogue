using System;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class DeliveryDatesBackLinkProviderTests
    {
        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
        public void Process_RouteValuesCatalogueItemIdIsNull_ThrowsException(
            Order order,
            RouteValues routeValues,
            DeliveryDatesBackLinkProvider provider)
        {
            routeValues.CatalogueItemId = null;

            FluentActions
                .Invoking(() => provider.Process(new OrderWrapper(order), null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
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

            result.ActionName.Should().Be(Constants.Actions.ReviewDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_NoPreviousOrderItem_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
        {
            order.SetupCatalogueSolution();

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, order.OrderItems.First().CatalogueItemId));

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
        [CommonAutoData]
        public void Process_PreviousItemExists_AssociatedServicesOnly_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            DeliveryDatesBackLinkProvider provider)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.ElementAt(0).CatalogueItem.Name = "A";
            order.OrderItems.ElementAt(1).CatalogueItem.Name = "B";
            order.OrderItems.ElementAt(2).CatalogueItem.Name = "C";

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, order.OrderItems.ElementAt(1).CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.EditDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
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

            order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(solution.CatalogueItemId, deliveryDate));

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, order.OrderItems.ElementAt(1).CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                catalogueItemId = solution.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.EditDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
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
            order.OrderRecipients.ForEach(r => r.SetDeliveryDateForItem(solution.CatalogueItemId, deliveryDate.AddDays(1)));

            var catalogueItemId = order.OrderItems.ElementAt(1).CatalogueItemId;

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                catalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.MatchDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
