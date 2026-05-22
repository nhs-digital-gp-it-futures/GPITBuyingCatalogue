using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
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

            mockAssociatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(order.GetSolutionId(), orderType.ToPracticeReorganisationType).Returns(services);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId);

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

            mockAssociatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(order.GetSolutionId(), orderType.ToPracticeReorganisationType).Returns(new[] { service }.ToList());

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
            EntityFramework.Ordering.Models.Order order,
            OrderItem orderItem,
            [Frozen] IOrderService mockOrderService,
            AssociatedServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Add(orderItem);

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

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
            EntityFramework.Ordering.Models.Order order,
            OrderItem orderItem,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrderItemService mockOrderItemService,
            AssociatedServicesController controller)
        {
            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var catalogueItemId = model.Services.First().CatalogueItemId;

            orderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.Add(orderItem);

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));


            mockOrderItemService.AddOrderItems(internalOrgId, callOffId, new[] { catalogueItemId }).Returns(Task.CompletedTask);

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
        public static async Task Get_SelectAssociatedServices_ForAdditionalService_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EntityFramework.Ordering.Models.Order order,
            OrderItem additionalService,
            List<OrderItem> existingAssociatedServices,
            List<CatalogueItem> associatedServices,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            SetupAdditionalService(additionalService, catalogueItemId);
            AddAssociatedServices(additionalService, existingAssociatedServices, associatedServices.First());
            order.OrderItems.Add(additionalService);

            var orderWrapper = new OrderWrapper(order);
            var existingAssociatedServiceItems = existingAssociatedServices.Select(x => x.CatalogueItem).ToList();

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(orderWrapper);
            mockAssociatedServicesService
                .GetPublishedAssociatedServicesForCatalogueItem(catalogueItemId, PracticeReorganisationTypeEnum.None)
                .Returns(associatedServices);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, catalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectServicesModel(existingAssociatedServiceItems, associatedServices)
            {
                SolutionId = catalogueItemId,
                InternalOrgId = internalOrgId,
                AssociatedServicesOnly = false,
                SolutionName = additionalService.CatalogueItem.Name,
                ParentItem = CatalogueItemType.AdditionalService,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectAssociatedServices_ForAdditionalService_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectServicesModel model,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, catalogueItemId, model);

            result.Should().BeOfType<ViewResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectAssociatedServices_ForAdditionalService_SelectionMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            OrderItem additionalService,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrderItemService mockOrderItemService,
            AssociatedServicesController controller)
        {
            SetupAdditionalService(additionalService, catalogueItemId);
            order.OrderItems.Add(additionalService);

            model.Services.ForEach(x => x.IsSelected = false);
            model.Services.First().IsSelected = true;

            var selectedServiceId = model.Services.First().CatalogueItemId;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, catalogueItemId, model);

            await mockOrderItemService.Received(1).AddOrderItems(
                internalOrgId,
                callOffId,
                Arg.Is<IEnumerable<CatalogueItemId>>(x => x.SequenceEqual(new[] { selectedServiceId })),
                additionalService.Id);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.ManageAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ManageAssociatedServices_NoAssociatedServices_RedirectsToTaskList(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EntityFramework.Ordering.Models.Order order,
            OrderItem additionalService,
            List<CatalogueItem> associatedServices,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            SetupAdditionalService(additionalService, catalogueItemId);
            additionalService.Services.Clear();
            order.OrderItems.Add(additionalService);

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));
            mockAssociatedServicesService
                .GetPublishedAssociatedServicesForCatalogueItem(catalogueItemId, order.OrderType.ToPracticeReorganisationType)
                .Returns(associatedServices);

            var result = await controller.ManageAssociatedServices(internalOrgId, callOffId, catalogueItemId);

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
        public static async Task Get_ManageAssociatedServices_WithAssociatedServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            EntityFramework.Ordering.Models.Order order,
            OrderItem additionalService,
            List<OrderItem> associatedServices,
            List<CatalogueItem> allAvailableAssociatedServices,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            SetupAdditionalService(additionalService, catalogueItemId);
            AddAssociatedServices(additionalService, associatedServices, allAvailableAssociatedServices.First());
            AddOrderRecipients(order);
            order.OrderItems.Add(additionalService);

            var orderWrapper = new OrderWrapper(order);
            var services = associatedServices.ToList();

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(orderWrapper);
            mockAssociatedServicesService
                .GetPublishedAssociatedServicesForCatalogueItem(catalogueItemId, order.OrderType.ToPracticeReorganisationType)
                .Returns(allAvailableAssociatedServices);

            var result = await controller.ManageAssociatedServices(internalOrgId, callOffId, catalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ManageAssociatedServicesModel(
                services,
                additionalService.CatalogueItem.Name,
                orderWrapper.DetermineOrderRecipients(catalogueItemId),
                callOffId,
                internalOrgId,
                catalogueItemId)
            {
                UnselectedAssociatedServicesAvailable = allAvailableAssociatedServices.Count != services.Count,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        private static void SetupAdditionalService(
            OrderItem additionalService,
            CatalogueItemId catalogueItemId)
        {
            additionalService.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            additionalService.CatalogueItem.Id = catalogueItemId;
            additionalService.CatalogueItemId = catalogueItemId;
        }

        private static void AddAssociatedServices(OrderItem additionalService, IEnumerable<OrderItem> existingAssociatedServices, CatalogueItem associatedService)
        {
            existingAssociatedServices.ForEach(associatedServiceOrderItem =>
            {
                associatedServiceOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;
                associatedServiceOrderItem.CatalogueItem = associatedService;
                additionalService.Services.Add(associatedServiceOrderItem);
            });
        }

        private static void AddOrderRecipients(EntityFramework.Ordering.Models.Order order)
        {
            order.OrderSublocations = new List<OrderSublocation>
            {
                new()
                {
                    OrderId = order.Id,
                    SublocationOdsCode = "ABC",
                    SublocationRecipients = new List<OrderSublocationRecipient>
                    {
                        new()
                        {
                            OrderId = order.Id,
                            ParentSublocationOdsCode = "ABC",
                            RecipientOdsCode = "DEF",
                        },
                    },
                },
            };
        }
    }
}
