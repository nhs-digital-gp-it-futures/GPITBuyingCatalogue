using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Routing.Providers
{
    public class SelectQuantityProviderTests
    {
        [Theory]
        [MockAutoData]
        public void Process_OrderWrapperIsNull_ThrowsException(
            RouteValues routeValues,
            SelectQuantityProvider provider)
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
            SelectQuantityProvider provider)
        {
            FluentActions
                .Invoking(() => provider.Process(new OrderWrapper(order), null))
                .Should().Throw<ArgumentNullException>()
                .WithParameterName("routeValues");
        }

        [Theory]
        [MockAutoData]
        public void Process_FromTaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            Order order,
            SelectQuantityProvider provider)
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
        public void Process_FromAmendment_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            Order order,
            SelectQuantityProvider provider)
        {
            var associatedServices = new List<CatalogueItem>();

            order.OrderItems.ElementAt(0).CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            mockAssociatedServicesService.GetPublishedAssociatedServicesForSolution(order.OrderItems.First().CatalogueItemId, PracticeReorganisationTypeEnum.None).Returns(associatedServices);

            callOffId = new CallOffId(callOffId.OrderNumber, 2);

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, catalogueItemId));

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
        [MockInlineAutoData(CatalogueItemType.AdditionalService)]
        [MockInlineAutoData(CatalogueItemType.AssociatedService)]
        public void Process_OrderHasServiceWithNoPrice_MultipleAvailable_ExpectedResult(
            CatalogueItemType catalogueItemType,
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems = order.OrderItems.Take(2).ToList();

            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var additionalService = order.OrderItems.ElementAt(1);
            additionalService.CatalogueItem.CatalogueItemType = catalogueItemType;
            additionalService.OrderItemPrice = null;

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                additionalService.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectPrice);
            result.ControllerName.Should().Be(Constants.Controllers.Prices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockInlineAutoData(CatalogueItemType.AdditionalService)]
        [MockInlineAutoData(CatalogueItemType.AssociatedService)]
        public void Process_OrderHasAssociatedServiceWithNoPrice_SingleAvailable_ExpectedResult(
            CatalogueItemType catalogueItemType,
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems = order.OrderItems.Take(2).ToList();

            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var service = order.OrderItems.ElementAt(1);
            service.CatalogueItem.CataloguePrices = service.CatalogueItem.CataloguePrices.Take(1).ToList();
            service.CatalogueItem.CatalogueItemType = catalogueItemType;
            service.OrderItemPrice = null;

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                service.CatalogueItemId,
            };

            result.ActionName.Should().Be(Constants.Actions.ConfirmPrice);
            result.ControllerName.Should().Be(Constants.Controllers.Prices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
        public void Process_OrderHasNoAdditionalServicesWithNoServiceRecipients_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            Order order,
            SelectQuantityProvider provider)
        {
            var associatedServices = new List<CatalogueItem>();

            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems.ToList().ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            mockAssociatedServicesService.GetPublishedAssociatedServicesForSolution(order.OrderItems.First().CatalogueItemId, PracticeReorganisationTypeEnum.None).Returns(associatedServices);

            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var result = provider.Process(new OrderWrapper(order), new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId));

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
        public void Process_OrderHasNoAssociatedServices_AndSomeAreAvailable_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            List<CatalogueItem> associatedServices,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SelectQuantityProvider provider)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.ElementAt(0).CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockAssociatedServicesService.GetPublishedAssociatedServicesForSolution(order.OrderItems.First().CatalogueItemId, PracticeReorganisationTypeEnum.None).Returns(associatedServices);

            var result = provider.Process(new OrderWrapper(order), new RouteValues(
                internalOrgId,
                callOffId,
                order.OrderItems.First().CatalogueItemId));

            mockAssociatedServicesService.Received().GetPublishedAssociatedServicesForSolution(order.OrderItems.First().CatalogueItemId, PracticeReorganisationTypeEnum.None);

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.SelectAssociatedServices);
            result.ControllerName.Should().Be(Constants.Controllers.AssociatedServices);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MockAutoData]
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

            var result = provider.Process(new OrderWrapper(order), new RouteValues(
                internalOrgId,
                callOffId,
                order.OrderItems.First().CatalogueItemId));

            var expected = new
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            result.ActionName.Should().Be(Constants.Actions.TaskList);
            result.ControllerName.Should().Be(Constants.Controllers.TaskList);
            result.RouteValues.Should().BeEquivalentTo(expected);
        }
    }
}
