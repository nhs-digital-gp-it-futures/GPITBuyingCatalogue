using System;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class EditDeliveryDatesProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_OrderIsNull_ThrowsException(
            RouteValues routeValues,
            EditDeliveryDatesProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(null, routeValues))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("order");
        }

        [Theory]
        [CommonAutoData]
        public void Process_RouteValuesIsNull_ThrowsException(
            Order order,
            EditDeliveryDatesProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(order, null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonAutoData]
        public void Process_RouteValuesCatalogueItemIdIsNull_ThrowsException(
            Order order,
            RouteValues routeValues,
            EditDeliveryDatesProvider provider)
        {
            routeValues.CatalogueItemId = null;

            FluentActions
                .Invoking(() => provider.Process(order, null))
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
            EditDeliveryDatesProvider provider)
        {
            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId)
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
        public void Process_NoSubsequentOrderItemAvailable_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            EditDeliveryDatesProvider provider)
        {
            order.SetupCatalogueSolution();

            var catalogueItemId = order.GetAdditionalServices().Last().CatalogueItemId;
            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

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
        public void Process_SubsequentOrderItemAvailable_AssociatedServicesOnly_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            EditDeliveryDatesProvider provider)
        {
            order.AssociatedServicesOnly = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.ElementAt(0).CatalogueItem.Name = "A";
            order.OrderItems.ElementAt(1).CatalogueItem.Name = "B";
            order.OrderItems.ElementAt(2).CatalogueItem.Name = "C";

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;
            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            catalogueItemId = order.OrderItems.ElementAt(1).CatalogueItemId;

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                catalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.EditDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_SubsequentOrderItemAvailable_SolutionMatchesPrimaryDeliveryDate_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            EditDeliveryDatesProvider provider)
        {
            var deliveryDate = DateTime.Today;

            order.SetupCatalogueSolution();
            order.DeliveryDate = deliveryDate;
            order.OrderItems.First().OrderItemRecipients.ForEach(r => r.DeliveryDate = deliveryDate);

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;
            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            catalogueItemId = order.GetAdditionalServices().First().CatalogueItemId;

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                catalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.EditDeliveryDates);
            result.ControllerName.Should().Be(Constants.Controllers.DeliveryDates);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_SubsequentOrderItemAvailable_SolutionDoesNotMatchPrimaryDeliveryDate_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            EditDeliveryDatesProvider provider)
        {
            var deliveryDate = DateTime.Today;

            order.SetupCatalogueSolution();
            order.DeliveryDate = deliveryDate;
            order.OrderItems.First().OrderItemRecipients.ForEach(r => r.DeliveryDate = deliveryDate.AddDays(1));

            var catalogueItemId = order.OrderItems.First().CatalogueItemId;
            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            catalogueItemId = order.GetAdditionalServices().First().CatalogueItemId;

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
