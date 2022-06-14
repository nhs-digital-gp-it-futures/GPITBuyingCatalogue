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
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class AssociatedServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AssociatedServicesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AssociatedServicesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
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
        [CommonAutoData]
        public static void Get_AddAssociatedServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            AssociatedServicesController controller)
        {
            var result = controller.AddAssociatedServices(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new AddAssociatedServicesModel
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddAssociatedServices_WithModelErrors_ReturnsExpectedResult(
            AddAssociatedServicesModel model,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            var result = controller.AddAssociatedServices(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddAssociatedServices_YesSelected_ReturnsExpectedResult(
            AddAssociatedServicesModel model,
            AssociatedServicesController controller)
        {
            model.AdditionalServicesRequired = YesNoRadioButtonTagHelper.Yes;

            var result = controller.AddAssociatedServices(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_AddAssociatedServices_NoSelected_ReturnsExpectedResult(
            AddAssociatedServicesModel model,
            AssociatedServicesController controller)
        {
            model.AdditionalServicesRequired = YesNoRadioButtonTagHelper.No;

            var result = controller.AddAssociatedServices(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ReviewSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ReviewSolutionsController.ReviewSolutions));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", model.InternalOrgId },
                { "callOffId", model.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAssociatedServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSolution(order.SolutionId))
                .ReturnsAsync(services);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockAssociatedServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectServicesModel(order, services, CatalogueItemType.AssociatedService)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                AssociatedServicesOnly = true,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectAssociatedServices_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            SelectServicesModel model,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSolution(order.SolutionId))
                .ReturnsAsync(services);

            var result = await controller.SelectAssociatedServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockAssociatedServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectServicesModel(order, services, CatalogueItemType.AssociatedService)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                AssociatedServicesOnly = true,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
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

            actualResult.ControllerName.Should().Be(typeof(ReviewSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ReviewSolutionsController.ReviewSolutions));
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

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.AddServiceRecipients));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedServices_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(order);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSolution(order.SolutionId))
                .ReturnsAsync(services);

            var result = await controller.EditAssociatedServices(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockAssociatedServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectServicesModel(order, services, CatalogueItemType.AdditionalService)
            {
                InternalOrgId = internalOrgId,
                CallOffId = order.CallOffId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAssociatedServices_NoChangesMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            AssociatedServicesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            for (var i = 0; i < order.OrderItems.Count; i++)
            {
                model.Services[i].CatalogueItemId = order.OrderItems.ElementAt(i).CatalogueItemId;
                model.Services[i].IsSelected = true;
            }

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.EditAssociatedServices(internalOrgId, callOffId, model);

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
        public static async Task Post_EditAssociatedServices_ServicesAdded_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId newServiceId,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AssociatedServicesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

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
                .ReturnsAsync(order);

            IEnumerable<CatalogueItemId> newServiceIds = null;

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => newServiceIds = x);

            var result = await controller.EditAssociatedServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockOrderItemService.VerifyAll();

            newServiceIds.Should().BeEquivalentTo(new[] { newServiceId });

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.AddServiceRecipients));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", newServiceId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAssociatedServices_ServicesRemoved_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectServicesModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            AssociatedServicesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            model.Services.Clear();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.EditAssociatedServices(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.ConfirmAssociatedServiceChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "serviceIds", string.Empty },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmAssociatedServiceChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> services,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IAssociatedServicesService> mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(s => s.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            mockAssociatedServicesService
                .Setup(x => x.GetPublishedAssociatedServicesForSupplier(order.SupplierId))
                .ReturnsAsync(services);

            var serviceIds = string.Join(",", new[]
            {
                order.OrderItems.ElementAt(1).CatalogueItemId,
                services.First().Id,
            });

            var result = await controller.ConfirmAssociatedServiceChanges(internalOrgId, callOffId, serviceIds);

            mockOrderService.VerifyAll();
            mockAssociatedServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmServiceChangesModel(internalOrgId, callOffId, CatalogueItemType.AssociatedService)
            {
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
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
        public static async Task Post_ConfirmAssociatedServiceChanges_ChangesNotConfirmed_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            AssociatedServicesController controller)
        {
            model.ConfirmChanges = false;

            var result = await controller.ConfirmAssociatedServiceChanges(internalOrgId, callOffId, model);

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
        public static async Task Post_ConfirmAssociatedServiceChanges_RemovingServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AssociatedServicesController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = new List<ServiceModel>();

            IEnumerable<CatalogueItemId> itemIds = null;

            mockOrderItemService
                .Setup(x => x.DeleteOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => itemIds = x);

            var result = await controller.ConfirmAssociatedServiceChanges(internalOrgId, callOffId, model);

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
        public static async Task Post_ConfirmAssociatedServiceChanges_AddingServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toAdd,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            AssociatedServicesController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = new List<ServiceModel>();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            IEnumerable<CatalogueItemId> itemIds = null;

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => itemIds = x);

            var result = await controller.ConfirmAssociatedServiceChanges(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

            itemIds.Should().BeEquivalentTo(toAdd);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.AddServiceRecipients));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", model.ToAdd.First().CatalogueItemId },
            });
        }
    }
}
