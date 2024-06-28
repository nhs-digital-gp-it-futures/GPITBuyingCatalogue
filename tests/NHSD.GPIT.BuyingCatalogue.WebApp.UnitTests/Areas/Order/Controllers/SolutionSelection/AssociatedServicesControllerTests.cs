using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
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
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        public static async Task Get_SelectAssociatedServices_ReturnsExpectedResult(
            OrderType orderType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            order.OrderType = orderType;
            var orderWrapper = new OrderWrapper(order);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(orderWrapper);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSolution(order.GetSolutionId(), orderType.ToPracticeReorganisationType))
                .ReturnsAsync(services);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockAssociatedServicesService.VerifyAll();

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
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static async Task Get_SelectAssociatedServices_MergerSplit_WithSingleService_RedirectsToSelectPrice(
            OrderType orderType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItem service,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            order.OrderType = orderType;
            var orderWrapper = new OrderWrapper(order);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(orderWrapper);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSolution(order.GetSolutionId(), orderType.ToPracticeReorganisationType))
                .ReturnsAsync(new[] { service }.ToList());

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId);
            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_SelectAssociatedServices_SelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AssociatedServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var catalogueItemId = model.Services.First().CatalogueItemId;

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, new[] { catalogueItemId }))
                .Returns(Task.CompletedTask);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

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
