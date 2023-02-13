using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class SelectQuantityProviderTests
    {
        [Theory]
        [CommonAutoData]
        public void Process_OrderIsNull_ThrowsException(
            RouteValues routeValues,
            SelectQuantityProvider provider)
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
            SelectQuantityProvider provider)
        {
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
            SelectQuantityProvider provider)
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

            result.ActionName.Should().Be(Constants.Actions.TaskList);
            result.ControllerName.Should().Be(Constants.Controllers.TaskList);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_OrderHasAdditionalServiceWithNoServiceRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var additionalService = order.OrderItems.ElementAt(1);
            additionalService.OrderItemRecipients.Clear();

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                additionalService.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.AddServiceRecipients);
            result.ControllerName.Should().Be(Constants.Controllers.ServiceRecipients);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_OrderHasAssociatedServiceWithNoServiceRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var service = order.OrderItems.ElementAt(1);
            service.OrderItemRecipients.Clear();

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                service.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.AddServiceRecipients);
            result.ControllerName.Should().Be(Constants.Controllers.ServiceRecipients);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_OrderHasNoAdditionalServicesWithNoServiceRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var result = provider.Process(order, new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

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
        public void Process_OrderHasNoAssociatedServices_AndSomeAreAvailable_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            List<CatalogueItem> associatedServices,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ElementAt(0).CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSolution(order.OrderItems.First().CatalogueItemId))
                .ReturnsAsync(associatedServices);

            var result = provider.Process(order, new RouteValues(
                internalOrgId,
                callOffId,
                order.OrderItems.First().CatalogueItemId));

            mockAssociatedServicesService.VerifyAll();

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.AddAssociatedServices);
            result.ControllerName.Should().Be(Constants.Controllers.AssociatedServices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public void Process_OrderHasPopulatedAdditionalAndAssociatedServices_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems.ElementAt(0).CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.ElementAt(1).CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.OrderItems.ElementAt(2).CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            var result = provider.Process(order, new RouteValues(
                internalOrgId,
                callOffId,
                order.OrderItems.First().CatalogueItemId));

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
