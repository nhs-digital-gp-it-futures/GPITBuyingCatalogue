using System;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class TaskListProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_OrderIsNull_ThrowsException(
            RouteValues routeValues,
            TaskListProvider provider)
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
            TaskListProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(order, null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [CommonInlineAutoData(true)]
        [CommonInlineAutoData(false)]
        public void Process_ExpectedResult(
            bool associatedServicesOnly,
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            TaskListProvider provider)
        {
            order.AssociatedServicesOnly = associatedServicesOnly;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            if (!associatedServicesOnly)
            {
                order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            }

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.Review);
            result.ControllerName.Should().Be(Constants.Controllers.Review);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_IncompleteSolution_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            TaskListProvider provider)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            solution.OrderItemPrice = null;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.OrderDashboard);
            result.ControllerName.Should().Be(Constants.Controllers.Orders);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_IncompleteDeliveryDates_Amendment_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            TaskListProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 2);

            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x =>
            {
                x.Quantity = 1;
                x.OrderItemRecipients.ForEach(r => r.Quantity = 1);
                x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
                x.OrderItemRecipients.ForEach(r => r.DeliveryDate = null);
            });

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.OrderDashboard);
            result.ControllerName.Should().Be(Constants.Controllers.Orders);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_IncompleteQuantity_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            TaskListProvider provider)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x =>
            {
                x.Quantity = 0;
                x.OrderItemRecipients.ForEach(r => r.Quantity = 0);
                x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            });

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.OrderDashboard);
            result.ControllerName.Should().Be(Constants.Controllers.Orders);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_IncompleteServices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            TaskListProvider provider)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.ElementAt(1).OrderItemPrice = null;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.OrderDashboard);
            result.ControllerName.Should().Be(Constants.Controllers.Orders);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_AssociatedServicesOnly_WithIncompleteServices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            TaskListProvider provider)
        {
            order.AssociatedServicesOnly = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.ElementAt(1).OrderItemRecipients.Clear();
            order.OrderItems.ElementAt(2).OrderItemPrice = null;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.OrderDashboard);
            result.ControllerName.Should().Be(Constants.Controllers.Orders);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_AssociatedServicesOnly_WithCompleteServices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            TaskListProvider provider)
        {
            order.AssociatedServicesOnly = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, catalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.Review);
            result.ControllerName.Should().Be(Constants.Controllers.Review);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
