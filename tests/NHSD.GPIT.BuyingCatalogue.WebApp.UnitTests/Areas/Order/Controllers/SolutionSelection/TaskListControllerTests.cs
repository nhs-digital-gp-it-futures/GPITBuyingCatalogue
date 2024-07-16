using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class TaskListControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(TaskListController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(TaskListController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(TaskListController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task TaskList_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult backRoute,
            RoutingResult onwardRoute,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IRoutingService routingService,
            TaskListController controller)
        {
            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            solutionsService.GetSupplierSolutions(order.SupplierId, order.SelectedFrameworkId).Returns(Enumerable.Empty<CatalogueItem>().ToList());

            additionalServicesService
                .GetAdditionalServicesBySolutionId(
                    Arg.Any<CatalogueItemId?>(),
                    Arg.Any<bool>())
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            associatedServicesService
                .GetPublishedAssociatedServicesForSolution(
                    Arg.Any<CatalogueItemId?>(),
                    Arg.Any<PracticeReorganisationTypeEnum>())
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            routingService.GetRoute(RoutingPoint.TaskListBackLink, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>())
                .Returns(backRoute);

            routingService.GetRoute(RoutingPoint.TaskList, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>())
                .Returns(onwardRoute);

            var result = await controller.TaskList(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(order));

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.OnwardLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task TaskList_WithAdditionalServicesAvailable_ExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> additionalServices,
            RoutingResult backRoute,
            RoutingResult onwardRoute,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            [Frozen] IAdditionalServicesService mockAdditionalServicesService,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IRoutingService routingService,
            TaskListController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockAdditionalServicesService.GetAdditionalServicesBySolutionId(order.OrderItems.First().CatalogueItemId, true).Returns(additionalServices);

            solutionsService.GetSupplierSolutions(order.SupplierId, order.SelectedFrameworkId).Returns(Enumerable.Empty<CatalogueItem>().ToList());

            associatedServicesService
                .GetPublishedAssociatedServicesForSolution(
                    Arg.Any<CatalogueItemId?>(),
                    Arg.Any<PracticeReorganisationTypeEnum>())
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            routingService.GetRoute(RoutingPoint.TaskListBackLink, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>())
                .Returns(backRoute);

            routingService.GetRoute(RoutingPoint.TaskList, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>())
                .Returns(onwardRoute);

            var result = await controller.TaskList(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new TaskListModel(internalOrgId, callOffId, new OrderWrapper(order))
            {
                AdditionalServicesAvailable = true,
                UnselectedAdditionalServicesAvailable = true,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink).Excluding(m => m.OnwardLink));
        }

        [Theory]
        [MockInlineAutoData(1)]
        [MockInlineAutoData(2)]
        public static async Task TaskList_EnsureOrderItemsForAmendment_ExpectedResult(
            int revision,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult backRoute,
            RoutingResult onwardRoute,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IRoutingService routingService,
            TaskListController controller)
        {
            order.Revision = revision;
            order.OrderType = OrderTypeEnum.Solution;

            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            solutionsService.GetSupplierSolutions(order.SupplierId, order.SelectedFrameworkId).Returns(Enumerable.Empty<CatalogueItem>().ToList());

            additionalServicesService
                .GetAdditionalServicesBySolutionId(
                    Arg.Any<CatalogueItemId?>(),
                    Arg.Any<bool>())
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            associatedServicesService
                .GetPublishedAssociatedServicesForSolution(
                    Arg.Any<CatalogueItemId?>(),
                    Arg.Any<PracticeReorganisationTypeEnum>())
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            routingService.GetRoute(RoutingPoint.TaskListBackLink, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>())
                .Returns(backRoute);

            routingService.GetRoute(RoutingPoint.TaskList, Arg.Any<OrderWrapper>(), Arg.Any<RouteValues>())
                .Returns(onwardRoute);

            _ = await controller.TaskList(internalOrgId, callOffId);

            await mockOrderService.Received().GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);

            if (revision == 1)
            {
                await mockOrderService.DidNotReceive().EnsureOrderItemsForAmendment(internalOrgId, callOffId);
            }
            else
            {
                await mockOrderService.Received().EnsureOrderItemsForAmendment(internalOrgId, callOffId);
            }
        }
    }
}
