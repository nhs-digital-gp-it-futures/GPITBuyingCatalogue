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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class CatalogueSolutionsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CatalogueSolutionsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(CatalogueSolutionsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolution_AssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SelectSolution(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolution_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            List<CatalogueItem> additionalServices,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = false;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutions(order.SupplierId))
                .ReturnsAsync(supplierSolutions);

            var solutionIds = supplierSolutions.Select(x => x.Id);

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionIds(solutionIds))
                .ReturnsAsync(additionalServices);

            var result = await controller.SelectSolution(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();
            mockAdditionalServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, additionalServices)
            {
                SelectedCatalogueSolutionId = string.Empty,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectSolution_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            List<CatalogueItem> additionalServices,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = false;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutions(order.SupplierId))
                .ReturnsAsync(supplierSolutions);

            var solutionIds = supplierSolutions.Select(x => x.Id);

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionIds(solutionIds))
                .ReturnsAsync(additionalServices);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectSolution(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();
            mockAdditionalServicesService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, additionalServices)
            {
                SelectedCatalogueSolutionId = model.SelectedCatalogueSolutionId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectSolution_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            CatalogueSolutionsController controller)
        {
            var catalogueItemId = new CatalogueItemId(1, "abc");

            model.SelectedCatalogueSolutionId = $"{catalogueItemId}";

            for (var i = 0; i < model.AdditionalServices.Count; i++)
            {
                model.AdditionalServices[i].CatalogueItemId = CatalogueItemId.ParseExact($"{catalogueItemId}{i:000}");
            }

            var ids = model.AdditionalServices
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .Union(new[] { catalogueItemId });

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, ids))
                .Returns(Task.CompletedTask);

            var result = await controller.SelectSolution(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.SelectPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolutionAssociatedServicesOnly_NonNullSolutionId_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId solutionId,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;
            order.SolutionId = solutionId;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolutionAssociatedServicesOnly_NonNullSolutionId_FromSelectAssociatedServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId solutionId,
            List<CatalogueItem> supplierSolutions,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;
            order.SolutionId = solutionId;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutionsWithAssociatedServices(order.SupplierId))
                .ReturnsAsync(supplierSolutions);

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId, RoutingSource.SelectAssociatedServices);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = $"{solutionId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolutionAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;
            order.SolutionId = null;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutionsWithAssociatedServices(order.SupplierId))
                .ReturnsAsync(supplierSolutions);

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = string.Empty,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectSolutionAssociatedServicesOnly_WithModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            SelectSolutionModel model,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;
            order.SolutionId = null;

            mockOrderService
                .Setup(x => x.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutionsWithAssociatedServices(order.SupplierId))
                .ReturnsAsync(supplierSolutions);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = model.SelectedCatalogueSolutionId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SelectSolutionAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            model.SelectedCatalogueSolutionId = $"{catalogueItemId}";

            mockOrderService
                .Setup(x => x.SetSolutionId(internalOrgId, callOffId, catalogueItemId))
                .Verifiable();

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "source", RoutingSource.SelectSolution },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSolution_AssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            mockOrderService
                .Setup(s => s.GetOrderThin(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditSolution(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSolution_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = false;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutions(order.SupplierId))
                .ReturnsAsync(solutions);

            var result = await controller.EditSolution(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, solutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = $"{order.OrderItems.First().CatalogueItemId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSolution_NoChangesMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            model.SelectedCatalogueSolutionId = $"{solution.CatalogueItemId}";

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditSolution(internalOrgId, callOffId, model);

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
        public static async Task Post_EditSolution_SolutionChanged_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId newSolutionId,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            model.SelectedCatalogueSolutionId = $"{newSolutionId}";

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditSolution(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(CatalogueSolutionsController.ConfirmSolutionChanges));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", newSolutionId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSolutionAssociatedServicesOnly_NoSolutionSelected_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;
            order.Solution = null;

            mockOrderService
                .Setup(s => s.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSolutionAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.AssociatedServicesOnly = true;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            mockOrderService
                .Setup(s => s.GetOrderWithOrderItems(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutionsWithAssociatedServices(order.SupplierId))
                .ReturnsAsync(solutions);

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, solutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = $"{order.SolutionId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSolutionAssociatedServicesOnly_WithModelError_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            SelectSolutionModel model,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            mockOrderService
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSupplierSolutions(order.SupplierId))
                .ReturnsAsync(solutions);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, order.CallOffId, model);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, solutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = model.SelectedCatalogueSolutionId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditSolutionAssociatedServicesOnly_NoChangesMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            model.SelectedCatalogueSolutionId = $"{order.Solution.Id}";

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

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
        public static async Task Post_EditSolutionAssociatedServicesOnly_SolutionChanged_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId newSolutionId,
            [Frozen] Mock<IOrderService> mockOrderService,
            CatalogueSolutionsController controller)
        {
            model.SelectedCatalogueSolutionId = $"{newSolutionId}";

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(CatalogueSolutionsController.ConfirmSolutionChangesAssociatedServicesOnly));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", newSolutionId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSolutionChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId catalogueItemId,
            CatalogueItem newSolution,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.ElementAt(1).CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.OrderItems.ElementAt(2).CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            mockOrderService
                .Setup(s => s.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(newSolution);

            var result = await controller.ConfirmSolutionChanges(internalOrgId, callOffId, catalogueItemId);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.Solution)
            {
                InternalOrgId = internalOrgId,
                ToAdd = new List<ServiceModel>
                {
                    new() { CatalogueItemId = newSolution.Id, Description = newSolution.Name },
                },
                ToRemove = order.OrderItems.Select(x => new ServiceModel
                {
                    CatalogueItemId = x.CatalogueItemId,
                    Description = x.CatalogueItem.Name,
                }).ToList(),
                Caption = $"Order {callOffId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSolutionChanges_ChangesNotConfirmed_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = false;

            var result = await controller.ConfirmSolutionChanges(internalOrgId, callOffId, model);

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
        public static async Task Post_ConfirmSolutionChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            List<CatalogueItemId> toAdd,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            var newSolutionId = model.ToAdd.First().CatalogueItemId;

            IEnumerable<CatalogueItemId> removedItemIds = null;
            IEnumerable<CatalogueItemId> addedItemIds = null;

            mockOrderItemService
                .Setup(x => x.DeleteOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => removedItemIds = x);

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => addedItemIds = x);

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionId(newSolutionId, true))
                .ReturnsAsync(new List<CatalogueItem>());

            var result = await controller.ConfirmSolutionChanges(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

            removedItemIds.Should().BeEquivalentTo(toRemove);
            addedItemIds.Should().BeEquivalentTo(toAdd);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(PricesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(PricesController.SelectPrice));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", newSolutionId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSolutionChanges_WithAdditionalServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            List<CatalogueItemId> toAdd,
            List<CatalogueItem> additionalServices,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            IEnumerable<CatalogueItemId> removedItemIds = null;
            IEnumerable<CatalogueItemId> addedItemIds = null;

            mockOrderItemService
                .Setup(x => x.DeleteOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => removedItemIds = x);

            mockOrderItemService
                .Setup(x => x.AddOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => addedItemIds = x);

            mockAdditionalServicesService
                .Setup(x => x.GetAdditionalServicesBySolutionId(model.ToAdd.First().CatalogueItemId, true))
                .ReturnsAsync(additionalServices);

            var result = await controller.ConfirmSolutionChanges(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();

            removedItemIds.Should().BeEquivalentTo(toRemove);
            addedItemIds.Should().BeEquivalentTo(toAdd);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AdditionalServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AdditionalServicesController.SelectAdditionalServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSolutionChanges_ClearsContract(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ConfirmServiceChangesModel model,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            await controller.ConfirmSolutionChanges(internalOrgId, callOffId, model);

            orderService.VerifyAll();
            mockContractsService.Verify(s => s.RemoveContract(orderId), Times.Once());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSolutionChangesAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId catalogueItemId,
            CatalogueItem newSolution,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            mockOrderService
                .Setup(s => s.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSolutionsService
                .Setup(x => x.GetSolutionThin(catalogueItemId))
                .ReturnsAsync(newSolution);

            var result = await controller.ConfirmSolutionChangesAssociatedServicesOnly(internalOrgId, callOffId, catalogueItemId);

            mockOrderService.VerifyAll();
            mockSolutionsService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.Solution)
            {
                InternalOrgId = internalOrgId,
                ToAdd = new List<ServiceModel>
                {
                    new() { CatalogueItemId = newSolution.Id, Description = newSolution.Name },
                },
                ToRemove = order.OrderItems.Select(x => new ServiceModel
                {
                    CatalogueItemId = x.CatalogueItemId,
                    Description = x.CatalogueItem.Name,
                }).ToList(),
            };

            expected.ToRemove.Add(new ServiceModel { CatalogueItemId = order.Solution.Id, Description = order.Solution.Name });

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSolutionChangesAssociatedServicesOnly_ChangesNotConfirmed_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = false;

            var result = await controller.ConfirmSolutionChangesAssociatedServicesOnly(internalOrgId, callOffId, model);

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
        public static async Task Post_ConfirmSolutionChangesAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            List<CatalogueItemId> toAdd,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<IOrderItemService> mockOrderItemService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            var newSolutionId = model.ToAdd.First().CatalogueItemId;

            IEnumerable<CatalogueItemId> removedItemIds = null;

            mockOrderItemService
                .Setup(x => x.DeleteOrderItems(internalOrgId, callOffId, It.IsAny<IEnumerable<CatalogueItemId>>()))
                .Callback<string, CallOffId, IEnumerable<CatalogueItemId>>((_, _, x) => removedItemIds = x);

            mockOrderService
                .Setup(x => x.SetSolutionId(internalOrgId, callOffId, newSolutionId))
                .Verifiable();

            var result = await controller.ConfirmSolutionChangesAssociatedServicesOnly(internalOrgId, callOffId, model);

            mockOrderItemService.VerifyAll();
            mockOrderService.VerifyAll();

            removedItemIds.Should().BeEquivalentTo(toRemove);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "source", RoutingSource.EditSolution },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSolutionChangesAssociatedServicesOnly_ClearsContract(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ConfirmServiceChangesModel model,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<IContractsService> mockContractsService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;

            orderService
                .Setup(x => x.GetOrderId(internalOrgId, callOffId))
                .ReturnsAsync(orderId);

            await controller.ConfirmSolutionChangesAssociatedServicesOnly(internalOrgId, callOffId, model);

            orderService.VerifyAll();
            mockContractsService.Verify(s => s.RemoveContract(orderId), Times.Once());
        }
    }
}
