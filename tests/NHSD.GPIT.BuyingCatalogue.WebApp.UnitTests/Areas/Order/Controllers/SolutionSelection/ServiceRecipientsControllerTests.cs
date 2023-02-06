﻿using System.Collections.Generic;
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
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using Xunit;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class ServiceRecipientsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(ServiceRecipientsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ServiceRecipientsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        [CommonInlineAutoData(SelectionMode.All)]
        public static async Task Get_AddServiceRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.AddServiceRecipients(internalOrgId, callOffId, solution.CatalogueItemId, selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                })
                .ToList();

            var expected = new SelectRecipientsModel(solution, null, recipients, selectionMode)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = solution.CatalogueItemId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddServiceRecipients_WithPreSelectedSolutionRecipients_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();
            var additionalService = order.OrderItems.ElementAt(1);

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            for (var i = 0; i < 3; i++)
            {
                solution.OrderItemRecipients.ElementAt(i).OdsCode = serviceRecipients[i].OrgId;
            }

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.AddServiceRecipients(internalOrgId, callOffId, additionalService.CatalogueItemId);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = actualResult.Model.Should().BeAssignableTo<SelectRecipientsModel>().Subject;

            model.PreSelected.Should().BeTrue();
            model.ServiceRecipients.ForEach(x => x.Selected.Should().BeTrue());
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.AddServiceRecipients(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddServiceRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            ServiceRecipientsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            var selectedOdsCodes = model.ServiceRecipients.Where(x => x.Selected).Select(x => x.OdsCode);
            var recipientIds = string.Join(ServiceRecipientsController.Separator, selectedOdsCodes);

            var result = controller.AddServiceRecipients(model.InternalOrgId, model.CallOffId, orderItem.Id, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
                { "catalogueItemId", orderItem.Id },
                { "recipientIds", recipientIds },
                { "journey", JourneyType.Add },
                { "source", model.Source },
            });
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData(SelectionMode.None)]
        [CommonInlineAutoData(SelectionMode.All)]
        public static async Task Get_EditServiceRecipients_ReturnsExpectedResult(
            SelectionMode? selectionMode,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOdsService> mockOdsService,
            ServiceRecipientsController controller)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOdsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            var result = await controller.EditServiceRecipients(internalOrgId, callOffId, solution.CatalogueItemId, selectionMode);

            mockOrderService.VerifyAll();
            mockOdsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var recipients = serviceRecipients
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                })
                .ToList();

            var expected = new SelectRecipientsModel(solution, null, recipients, selectionMode)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = solution.CatalogueItemId,
                IsAdding = false,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_EditServiceRecipients_WithModelErrors_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_EditServiceRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            EntityFramework.Ordering.Models.Order order,
            ServiceRecipientsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var orderItem = order.OrderItems.First().CatalogueItem;

            orderItem.CatalogueItemType = CatalogueItemType.Solution;

            var selectedOdsCodes = model.ServiceRecipients.Where(x => x.Selected).Select(x => x.OdsCode);
            var recipientIds = string.Join(ServiceRecipientsController.Separator, selectedOdsCodes);

            var result = controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, orderItem.Id, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.ConfirmChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
                { "catalogueItemId", orderItem.Id },
                { "recipientIds", recipientIds },
                { "journey", JourneyType.Edit },
                { "source", model.Source },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_EditServiceRecipients_NoSelectedRecipients_ReturnsExpectedResult(
            SelectRecipientsModel model,
            ServiceRecipientsController controller)
        {
            model.ServiceRecipients.ForEach(x => x.Selected = false);

            var result = controller.EditServiceRecipients(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }

        [Theory]
        [CommonInlineAutoData(JourneyType.Add)]
        [CommonInlineAutoData(JourneyType.Edit)]
        public static async Task Get_ConfirmChanges_ReturnsExpectedResult(
            JourneyType journeyType,
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<ServiceRecipient> serviceRecipients,
            RoutingResult routingResult,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IOdsService> odsService,
            [Frozen] Mock<IRoutingService> routingService,
            ServiceRecipientsController controller)
        {
            callOffId = new CallOffId(callOffId.OrderNumber, 1);
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            odsService
                .Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(internalOrgId))
                .ReturnsAsync(serviceRecipients);

            RouteValues routeValues = null;

            routingService
                .Setup(x => x.GetRoute(RoutingPoint.ConfirmServiceRecipientsBackLink, order, It.IsAny<RouteValues>()))
                .Callback<RoutingPoint, EntityFramework.Ordering.Models.Order, RouteValues>((_, _, x) => routeValues = x)
                .Returns(routingResult);

            var recipientIds = solution.OrderItemRecipients.First().OdsCode;

            var result = await controller.ConfirmChanges(
                internalOrgId,
                callOffId,
                solution.CatalogueItemId,
                recipientIds,
                journeyType,
                source: null);

            orderService.VerifyAll();
            odsService.VerifyAll();
            routingService.VerifyAll();

            routeValues.Should().BeEquivalentTo(new RouteValues(internalOrgId, callOffId, solution.CatalogueItemId)
            {
                Journey = journeyType,
                RecipientIds = recipientIds,
            });

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmChangesModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = solution.CatalogueItemId,
                Caption = solution.CatalogueItem.Name,
                Advice = string.Format(ConfirmChangesModel.AdviceText, solution.CatalogueItem.CatalogueItemType.Name()),
                Journey = journeyType,
                Selected = new List<ServiceRecipientModel>(),
                PreviouslySelected = new List<ServiceRecipientModel>(),
            };

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmChanges_ReturnsExpectedResult(
            ConfirmChangesModel model,
            EntityFramework.Ordering.Models.Order order,
            RoutingResult routingResult,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IOrderItemService> orderItemService,
            [Frozen] Mock<IOrderItemRecipientService> orderItemRecipientService,
            [Frozen] Mock<IRoutingService> routingService,
            ServiceRecipientsController controller)
        {
            orderService
                .Setup(x => x.GetOrderWithCatalogueItemAndPrices(model.CallOffId, model.InternalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            IEnumerable<CatalogueItemId> catalogueItemIds = null;

            orderItemService
                .Setup(x => x.CopyOrderItems(model.InternalOrgId, model.CallOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => catalogueItemIds = x);

            List<ServiceRecipientDto> serviceRecipientDtos = null;

            orderItemRecipientService
                .Setup(x => x.UpdateOrderItemRecipients(order.Id, model.CatalogueItemId, It.IsAny<List<ServiceRecipientDto>>()))
                .Callback<int, CatalogueItemId, List<ServiceRecipientDto>>((_, _, x) => serviceRecipientDtos = x);

            RouteValues routeValues = null;

            routingService
                .Setup(x => x.GetRoute(RoutingPoint.ConfirmServiceRecipients, order, It.IsAny<RouteValues>()))
                .Callback<RoutingPoint, EntityFramework.Ordering.Models.Order, RouteValues>((_, _, x) => routeValues = x)
                .Returns(routingResult);

            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var result = await controller.ConfirmChanges(model.InternalOrgId, model.CallOffId, model.CatalogueItemId, model);

            orderService.VerifyAll();
            orderItemService.VerifyAll();
            orderItemRecipientService.VerifyAll();
            routingService.VerifyAll();

            catalogueItemIds.Should().BeEquivalentTo(new[] { model.CatalogueItemId });
            serviceRecipientDtos.Should().BeEquivalentTo(model.Selected.Select(x => x.Dto).ToList());
            routeValues.Should().BeEquivalentTo(new RouteValues(model.InternalOrgId, model.CallOffId, model.CatalogueItemId) { Source = model.Source });

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ControllerName.Should().Be(routingResult.ControllerName);
            actual.ActionName.Should().Be(routingResult.ActionName);
        }
    }
}
