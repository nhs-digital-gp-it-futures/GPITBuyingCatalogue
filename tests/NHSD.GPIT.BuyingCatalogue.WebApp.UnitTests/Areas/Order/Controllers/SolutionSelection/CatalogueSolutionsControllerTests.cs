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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using NSubstitute.ReturnsExtensions;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CatalogueSolutionsController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectSolution_AssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectSolution(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectSolution_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            List<CatalogueItem> additionalServices,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            [Frozen] IAdditionalServicesService mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;

            mockOrderService.GetOrderThin(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            mockSolutionsService.GetSupplierSolutions(order.SupplierId, Arg.Any<string>())
                .Returns(supplierSolutions);

            mockAdditionalServicesService.GetAdditionalServicesBySolutionIds(Arg.Any<IEnumerable<CatalogueItemId>>())
                .Returns(additionalServices);

            var result = await controller.SelectSolution(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, additionalServices)
            {
                SelectedCatalogueSolutionId = string.Empty,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectSolution_WithModelErrors_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            List<CatalogueItem> additionalServices,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            [Frozen] IAdditionalServicesService mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;

            mockOrderService.GetOrderThin(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            mockSolutionsService.GetSupplierSolutions(order.SupplierId, Arg.Any<string>())
                .Returns(supplierSolutions);

            mockAdditionalServicesService.GetAdditionalServicesBySolutionIds(Arg.Any<IEnumerable<CatalogueItemId>>())
                .Returns(additionalServices);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectSolution(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, additionalServices)
            {
                SelectedCatalogueSolutionId = model.SelectedCatalogueSolutionId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectSolution_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            [Frozen] IOrderItemService mockOrderItemService,
            CatalogueSolutionsController controller)
        {
            var orderItem = order.OrderItems.First();
            var catalogueItemId = new CatalogueItemId(1, "abc");
            orderItem.CatalogueItemId = catalogueItemId;
            orderItem.CatalogueItem.Id = catalogueItemId;

            model.SelectedCatalogueSolutionId = $"{catalogueItemId}";

            for (var i = 0; i < model.AdditionalServices.Count; i++)
            {
                model.AdditionalServices[i].CatalogueItemId = CatalogueItemId.ParseExact($"{catalogueItemId}{i:000}");
            }

            mockOrderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            var ids = model.AdditionalServices
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .Union(new[] { catalogueItemId });

            mockOrderItemService.AddOrderItems(internalOrgId, callOffId, ids).Returns(Task.CompletedTask);

            var result = await controller.SelectSolution(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectSolutionAssociatedServicesOnly_NonNullSolutionId_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId solutionId,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = solutionId;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task
            Get_SelectSolutionAssociatedServicesOnly_NonNullSolutionId_FromSelectAssociatedServices_ReturnsExpectedResult(
                string internalOrgId,
                CallOffId callOffId,
                EntityFramework.Ordering.Models.Order order,
                CatalogueItemId solutionId,
                List<CatalogueItem> supplierSolutions,
                [Frozen] IOrderService mockOrderService,
                [Frozen] ISolutionsService mockSolutionsService,
                CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = solutionId;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService
                .GetSupplierSolutionsWithAssociatedServices(
                    order.SupplierId,
                    order.OrderType.ToPracticeReorganisationType,
                    Arg.Any<string>())
                .Returns(supplierSolutions);

            var result = await controller.SelectSolutionAssociatedServicesOnly(
                internalOrgId,
                callOffId,
                RoutingSource.SelectAssociatedServices);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = $"{solutionId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_SelectSolutionAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = null;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService
                .GetSupplierSolutionsWithAssociatedServices(
                    order.SupplierId,
                    order.OrderType.ToPracticeReorganisationType,
                    Arg.Any<string>())
                .Returns(supplierSolutions);

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = string.Empty,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectSolutionAssociatedServicesOnly_WithModelError_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> supplierSolutions,
            SelectSolutionModel model,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.SolutionId = null;

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService
                .GetSupplierSolutionsWithAssociatedServices(
                    order.SupplierId,
                    order.OrderType.ToPracticeReorganisationType,
                    Arg.Any<string>())
                .Returns(supplierSolutions);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, supplierSolutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = model.SelectedCatalogueSolutionId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SelectSolutionAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            CatalogueItemId catalogueItemId,
            CatalogueSolutionsController controller)
        {
            model.SelectedCatalogueSolutionId = $"{catalogueItemId}";

            var result = await controller.SelectSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "internalOrgId", internalOrgId },
                        { "callOffId", callOffId },
                        { "source", RoutingSource.SelectSolution },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSolution_AssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            mockOrderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditSolution(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSolution_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService.GetSupplierSolutions(order.SupplierId, Arg.Any<string>()).Returns(solutions);

            var result = await controller.EditSolution(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, solutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = $"{order.OrderItems.First().CatalogueItemId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSolution_NoChangesMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);

            var solution = order.OrderItems.First();

            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            model.SelectedCatalogueSolutionId = $"{solution.CatalogueItemId}";

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditSolution(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSolution_SolutionChanged_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId newSolutionId,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            model.SelectedCatalogueSolutionId = $"{newSolutionId}";

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditSolution(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(CatalogueSolutionsController.ConfirmSolutionChanges));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "internalOrgId", internalOrgId },
                        { "callOffId", callOffId },
                        { "catalogueItemId", newSolutionId },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSolutionAssociatedServicesOnly_NoSolutionSelected_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.AssociatedServicesOnlyDetails.Solution = null;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, callOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditSolutionAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService.GetSupplierSolutionsWithAssociatedServices(
                    order.SupplierId,
                    order.OrderType.ToPracticeReorganisationType,
                    Arg.Any<string>())
                .Returns(solutions);

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, solutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = $"{order.AssociatedServicesOnlyDetails.SolutionId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSolutionAssociatedServicesOnly_WithModelError_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<CatalogueItem> solutions,
            SelectSolutionModel model,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            mockOrderService.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService.GetSupplierSolutions(order.SupplierId, Arg.Any<string>()).Returns(solutions);

            controller.ModelState.AddModelError("key", "errorMessage");

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, order.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new SelectSolutionModel(order, solutions, Enumerable.Empty<CatalogueItem>())
            {
                SelectedCatalogueSolutionId = model.SelectedCatalogueSolutionId,
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSolutionAssociatedServicesOnly_NoChangesMade_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            model.SelectedCatalogueSolutionId = $"{order.AssociatedServicesOnlyDetails.Solution.Id}";

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditSolutionAssociatedServicesOnly_SolutionChanged_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            SelectSolutionModel model,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId newSolutionId,
            [Frozen] IOrderService mockOrderService,
            CatalogueSolutionsController controller)
        {
            model.SelectedCatalogueSolutionId = $"{newSolutionId}";

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            var result = await controller.EditSolutionAssociatedServicesOnly(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(CatalogueSolutionsController.ConfirmSolutionChangesAssociatedServicesOnly));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "internalOrgId", internalOrgId },
                        { "callOffId", callOffId },
                        { "catalogueItemId", newSolutionId },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmSolutionChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId catalogueItemId,
            CatalogueItem newSolution,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.ElementAt(1).CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.OrderItems.ElementAt(2).CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService.GetSolutionThin(catalogueItemId).Returns(newSolution);

            var result = await controller.ConfirmSolutionChanges(internalOrgId, callOffId, catalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.Solution)
            {
                InternalOrgId = internalOrgId,
                ToAdd = new List<ServiceModel>
                {
                    new() { CatalogueItemId = newSolution.Id, Description = newSolution.Name },
                },
                ToRemove = order.OrderItems.Select(
                        x => new ServiceModel
                            {
                                CatalogueItemId = x.CatalogueItemId, Description = x.CatalogueItem.Name,
                            })
                    .ToList(),
                Caption = $"Order {callOffId}",
            };

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
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
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmSolutionChanges_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            List<CatalogueItemId> toAdd,
            [Frozen] IOrderItemService mockOrderItemService,
            [Frozen] IAdditionalServicesService mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            var newSolutionId = model.ToAdd.First().CatalogueItemId;

            IEnumerable<CatalogueItemId> removedItemIds = null;
            IEnumerable<CatalogueItemId> addedItemIds = null;

            mockOrderItemService.DeleteOrderItems(
                    internalOrgId,
                    callOffId,
                    Arg.Do<IEnumerable<CatalogueItemId>>(x => removedItemIds = x))
                .Returns(Task.CompletedTask);

            mockOrderItemService.AddOrderItems(
                    internalOrgId,
                    callOffId,
                    Arg.Do<IEnumerable<CatalogueItemId>>(x => addedItemIds = x))
                .Returns(Task.CompletedTask);

            mockAdditionalServicesService.GetAdditionalServicesBySolutionId(newSolutionId, true)
                .Returns(new List<CatalogueItem>());

            var result = await controller.ConfirmSolutionChanges(internalOrgId, callOffId, model);

            removedItemIds.Should().BeEquivalentTo(toRemove);
            addedItemIds.Should().BeEquivalentTo(toAdd);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmSolutionChanges_WithAdditionalServices_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            List<CatalogueItemId> toAdd,
            List<CatalogueItem> additionalServices,
            [Frozen] IOrderItemService mockOrderItemService,
            [Frozen] IAdditionalServicesService mockAdditionalServicesService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            IEnumerable<CatalogueItemId> removedItemIds = null;
            IEnumerable<CatalogueItemId> addedItemIds = null;

            mockOrderItemService.DeleteOrderItems(
                    internalOrgId,
                    callOffId,
                    Arg.Do<IEnumerable<CatalogueItemId>>(x => removedItemIds = x))
                .Returns(Task.CompletedTask);

            mockOrderItemService.AddOrderItems(
                    internalOrgId,
                    callOffId,
                    Arg.Do<IEnumerable<CatalogueItemId>>(x => addedItemIds = x))
                .Returns(Task.CompletedTask);

            mockAdditionalServicesService.GetAdditionalServicesBySolutionId(model.ToAdd.First().CatalogueItemId, true)
                .Returns(additionalServices);

            var result = await controller.ConfirmSolutionChanges(internalOrgId, callOffId, model);

            removedItemIds.Should().BeEquivalentTo(toRemove);
            addedItemIds.Should().BeEquivalentTo(toAdd);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmSolutionChanges_ClearsContract(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ConfirmServiceChangesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IContractsService mockContractsService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;

            orderService.GetOrderId(internalOrgId, callOffId).Returns(orderId);

            await controller.ConfirmSolutionChanges(internalOrgId, callOffId, model);

            await mockContractsService.Received().RemoveContract(orderId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmSolutionChangesAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            CatalogueItemId catalogueItemId,
            CatalogueItem newSolution,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISolutionsService mockSolutionsService,
            CatalogueSolutionsController controller)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService);

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            mockSolutionsService.GetSolutionThin(catalogueItemId).Returns(newSolution);

            var result = await controller.ConfirmSolutionChangesAssociatedServicesOnly(
                internalOrgId,
                callOffId,
                catalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.Solution)
            {
                InternalOrgId = internalOrgId,
                ToAdd = new List<ServiceModel>
                {
                    new() { CatalogueItemId = newSolution.Id, Description = newSolution.Name },
                },
                ToRemove = order.OrderItems.Select(
                        x => new ServiceModel
                        {
                            CatalogueItemId = x.CatalogueItemId, Description = x.CatalogueItem.Name,
                        })
                    .ToList(),
            };

            expected.ToRemove.Add(
                new ServiceModel
                {
                    CatalogueItemId = order.AssociatedServicesOnlyDetails.Solution.Id,
                    Description = order.AssociatedServicesOnlyDetails.Solution.Name,
                });

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task
            Post_ConfirmSolutionChangesAssociatedServicesOnly_ChangesNotConfirmed_ReturnsExpectedResult(
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
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmSolutionChangesAssociatedServicesOnly_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmServiceChangesModel model,
            List<CatalogueItemId> toRemove,
            List<CatalogueItemId> toAdd,
            [Frozen] IOrderItemService mockOrderItemService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;
            model.ToRemove = toRemove.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();
            model.ToAdd = toAdd.Select(x => new ServiceModel { CatalogueItemId = x, IsSelected = true }).ToList();

            IEnumerable<CatalogueItemId> removedItemIds = null;

            mockOrderItemService.DeleteOrderItems(
                    internalOrgId,
                    callOffId,
                    Arg.Do<IEnumerable<CatalogueItemId>>(x => removedItemIds = x))
                .Returns(Task.CompletedTask);

            var result = await controller.ConfirmSolutionChangesAssociatedServicesOnly(internalOrgId, callOffId, model);

            removedItemIds.Should().BeEquivalentTo(toRemove);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary
                    {
                        { "internalOrgId", internalOrgId },
                        { "callOffId", callOffId },
                        { "source", RoutingSource.EditSolution },
                    });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmSolutionChangesAssociatedServicesOnly_ClearsContract(
            string internalOrgId,
            CallOffId callOffId,
            int orderId,
            ConfirmServiceChangesModel model,
            [Frozen] IOrderService orderService,
            [Frozen] IContractsService mockContractsService,
            CatalogueSolutionsController controller)
        {
            model.ConfirmChanges = true;

            orderService.GetOrderId(internalOrgId, callOffId).Returns(orderId);

            await controller.ConfirmSolutionChangesAssociatedServicesOnly(internalOrgId, callOffId, model);

            await mockContractsService.Received().RemoveContract(orderId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RemoveService_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderItemService orderItemService,
            CatalogueSolutionsController controller,
            OrderItem orderItem)
        {
            orderItemService.GetOrderItem(order.CallOffId, internalOrgId, orderItem.CatalogueItemId).Returns(orderItem);

            var result = await controller.RemoveService(internalOrgId, order.CallOffId, orderItem.CatalogueItemId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new RemoveServiceModel(orderItem.CatalogueItem);

            actualResult.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_RemoveService_IncorrectCatalogueItemId_ReturnsRedirect(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderItemService orderItemService,
            CatalogueSolutionsController controller,
            OrderItem orderItem)
        {
            orderItemService.GetOrderItem(order.CallOffId, internalOrgId, orderItem.CatalogueItemId).ReturnsNull();

            var result = await controller.RemoveService(internalOrgId, order.CallOffId, orderItem.CatalogueItemId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_RemoveService_RemovesService(
            string internalOrgId,
            CallOffId callOffId,
            RemoveServiceModel model,
            CatalogueItem catalogueItem,
            [Frozen] IOrderItemService mockOrderItemService,
            CatalogueSolutionsController controller)
        {
            var result = await controller.RemoveService(internalOrgId, callOffId, catalogueItem.Id, model);

            await mockOrderItemService.Received()
                .DeleteOrderItems(internalOrgId, callOffId, Arg.Any<List<CatalogueItemId>>());

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(TaskListController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(TaskListController.TaskList));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, });
        }
    }
}
