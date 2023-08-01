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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class AdditionalServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AdditionalServicesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AdditionalServicesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServices_NoSolution_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            AdditionalServicesController controller)
        {
            order.OrderItems.Clear();

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SelectAdditionalServices(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServices_WithSolution_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            AdditionalServicesController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var solutionId = order.OrderItems.First().CatalogueItemId;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(orderWrapper);

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionId(solutionId, true))
                .ReturnsAsync(services);

            var result = await controller.SelectAdditionalServices(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockAdditionalServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var previousItems = orderWrapper.Previous?.GetAssociatedServices().Select(x => x.CatalogueItem)
                ?? Enumerable.Empty<CatalogueItem>();
            var currentItems = orderWrapper.Order?.GetAssociatedServices().Select(x => x.CatalogueItem)
                ?? Enumerable.Empty<CatalogueItem>();

            var expected = new SelectServicesModel(previousItems,  currentItems, services)
            {
                InternalOrgId = internalOrgId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                SolutionName = order.AssociatedServicesOnly
                    ? orderWrapper.RolledUp.Solution.Name
                    : orderWrapper.RolledUp.GetSolution()?.CatalogueItem.Name,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAdditionalServices_NoSelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            AdditionalServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectAdditionalServices, order, It.IsAny<RouteValues>()))
                .Returns(new RoutingResult
                {
                    ActionName = Constants.Actions.TaskList,
                    ControllerName = Constants.Controllers.TaskList,
                    RouteValues = new { internalOrgId, callOffId },
                });

            var result = await controller.SelectAdditionalServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockRoutingService.VerifyAll();

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
        public static async Task Post_SelectAdditionalServices_SelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IRoutingService> mockRoutingService,
            AdditionalServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var catalogueItemId = model.Services.First().CatalogueItemId;

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, new[] { catalogueItemId }))
                .Returns(Task.CompletedTask);

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockRoutingService
                .Setup(x => x.GetRoute(RoutingPoint.SelectAdditionalServices, order, It.IsAny<RouteValues>()))
                .Returns(new RoutingResult
                {
                    ActionName = Constants.Actions.TaskList,
                    ControllerName = Constants.Controllers.TaskList,
                    RouteValues = new { internalOrgId, callOffId },
                });

            var result = await controller.SelectAdditionalServices(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();
            mockOrderService.VerifyAll();
            mockRoutingService.VerifyAll();

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
        public static async Task Get_EditAdditionalServices_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            AdditionalServicesController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var solutionId = order.OrderItems.First().CatalogueItemId;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(orderWrapper);

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionId(solutionId, true))
                .ReturnsAsync(services);

            var result = await controller.EditAdditionalServices(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockAdditionalServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var previousItems = orderWrapper.Previous?.GetAssociatedServices().Select(x => x.CatalogueItem)
                ?? Enumerable.Empty<CatalogueItem>();
            var currentItems = orderWrapper.Order?.GetAssociatedServices().Select(x => x.CatalogueItem)
                ?? Enumerable.Empty<CatalogueItem>();

            var expected = new SelectServicesModel(previousItems, currentItems, services)
            {
                InternalOrgId = internalOrgId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                SolutionName = order.AssociatedServicesOnly
                    ? orderWrapper.RolledUp.Solution.Name
                    : orderWrapper.RolledUp.GetSolution()?.CatalogueItem.Name,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAdditionalServices_NoChangesMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            AdditionalServicesController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            for (var i = 0; i < order.OrderItems.Count; i++)
            {
                model.Services[i].CatalogueItemId = order.OrderItems.ElementAt(i).CatalogueItemId;
                model.Services[i].IsSelected = true;
            }

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditAdditionalServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

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
        public static async Task Post_EditAdditionalServices_ServicesAdded_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId newServiceId,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AdditionalServicesController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            for (var i = 0; i < order.OrderItems.Count; i++)
            {
                model.Services[i].CatalogueItemId = order.OrderItems.ElementAt(i).CatalogueItemId;
                model.Services[i].IsSelected = true;
            }

            model.Services.Add(new ServiceModel
            {
                CatalogueItemId = newServiceId,
                IsSelected = true,
            });

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            IEnumerable<CatalogueItemId> newServiceIds = null;

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => newServiceIds = x);

            var result = await controller.EditAdditionalServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockOrderItemService.VerifyAll();

            newServiceIds.Should().BeEquivalentTo(new[] { newServiceId });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.SelectServiceRecipients));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", newServiceId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAdditionalServices_ServicesRemoved_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            AdditionalServicesController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            model.Services.Clear();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditAdditionalServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AdditionalServicesController.ConfirmAdditionalServiceChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "serviceIds", string.Empty },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmAdditionalServiceChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            AdditionalServicesController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            var solutionId = order.OrderItems.First().CatalogueItemId;

            mockOrderService
                .Setup(s => s.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionId(solutionId, true))
                .ReturnsAsync(services);

            var serviceIds = string.Join(",", new[]
            {
                order.OrderItems.ElementAt(1).CatalogueItemId,
                services.First().Id,
            });

            var result = await controller.ConfirmAdditionalServiceChanges(internalOrgId, callOffId, serviceIds);

            mockOrderService.VerifyAll();
            mockAdditionalServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.AdditionalService)
            {
                InternalOrgId = internalOrgId,
                ToAdd = new List<ServiceModel>
                {
                    new() { CatalogueItemId = services.First().Id, Description = services.First().Name },
                },
                ToRemove = new List<ServiceModel>
                {
                    new() { CatalogueItemId = order.OrderItems.ElementAt(2).CatalogueItemId },
                },
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmAdditionalServiceChanges_ChangesNotConfirmed_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            AdditionalServicesController controller)
        {
            model.ConfirmChanges = false;

            var result = await controller.ConfirmAdditionalServiceChanges(internalOrgId, callOffId, model);

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
        public static async Task Post_ConfirmAdditionalServiceChanges_RemovingServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AdditionalServicesController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = new List<ServiceModel>();

            IEnumerable<CatalogueItemId> itemIds = null;

            mockOrderItemService
                .Setup(x => x.DeleteOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => itemIds = x);

            var result = await controller.ConfirmAdditionalServiceChanges(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

            itemIds.Should().BeEquivalentTo(toRemove);

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
        public static async Task Post_ConfirmAdditionalServiceChanges_AddingServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toAdd,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AdditionalServicesController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = new List<ServiceModel>();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            IEnumerable<CatalogueItemId> itemIds = null;

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => itemIds = x);

            var result = await controller.ConfirmAdditionalServiceChanges(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

            itemIds.Should().BeEquivalentTo(toAdd);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.SelectServiceRecipients));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", model.ToAdd.First().CatalogueItemId },
            });
        }
    }
}
