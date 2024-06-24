using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class AssociatedServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AssociatedServicesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AssociatedServicesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static async Task Get_SelectAssociatedServices_ReturnsExpectedResult(
            OrderType orderType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            order.OrderType = orderType;
            var orderWrapper = new OrderWrapper(order);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(orderWrapper);

            mockAssociatedServicesService.GetPublishedAssociatedServicesForSolution(order.GetSolutionId(), orderType.ToPracticeReorganisationType).Returns(services);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId);

            await mockOrderService.Received().GetOrderThin(callOffId, internalOrgId);
            await mockAssociatedServicesService.Received().GetPublishedAssociatedServicesForSolution(order.GetSolutionId(), orderType.ToPracticeReorganisationType);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var currentItems = orderWrapper.Order?.GetAssociatedServices().Select(x => x.CatalogueItem)
                ?? Enumerable.Empty<CatalogueItem>();

            var expected = new SelectServicesModel(currentItems, services)
            {
                InternalOrgId = internalOrgId,
                AssociatedServicesOnly = order.OrderType.AssociatedServicesOnly,
                SolutionName = order.OrderType.GetSolutionNameFromOrder(orderWrapper.RolledUp),
                SolutionId = order.GetSolutionId(),
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static async Task Get_SelectAssociatedServices_MergerSplit_WithSingleService_RedirectsToSelectPrice(
            OrderType orderType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItem service,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            order.OrderType = orderType;
            var orderWrapper = new OrderWrapper(order);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(orderWrapper);

            mockAssociatedServicesService.GetPublishedAssociatedServicesForSolution(order.GetSolutionId(), orderType.ToPracticeReorganisationType).Returns(new[] { service }.ToList());

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectAssociatedServices_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            result.Should().BeOfType<ViewResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectAssociatedServices_NoSelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            AssociatedServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectAssociatedServices_SelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            [Frozen] IOrderItemService mockOrderItemService,
            AssociatedServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var catalogueItemId = model.Services.First().CatalogueItemId;

            mockOrderItemService.AddOrderItems(internalOrgId, callOffId, new[] { catalogueItemId }).Returns(Task.CompletedTask);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            await mockOrderItemService.Received().AddOrderItems(internalOrgId, callOffId, Arg.Any<List<CatalogueItemId>>());

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }
    }
}
