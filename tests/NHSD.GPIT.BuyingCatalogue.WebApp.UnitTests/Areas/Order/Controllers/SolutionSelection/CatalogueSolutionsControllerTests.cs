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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers.SolutionSelection
{
    public static class CatalogueSolutionsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CatalogueSolutionsController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(CatalogueSolutionsController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
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
                .ReturnsAsync(order);

            var result = await controller.SelectSolution(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(AssociatedServicesController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(AssociatedServicesController.SelectAssociatedServices));
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
                .ReturnsAsync(order);

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
                .ReturnsAsync(order);

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

            actualResult.ControllerName.Should().Be(typeof(ServiceRecipientsController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(ServiceRecipientsController.AddServiceRecipients));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "catalogueItemId", catalogueItemId },
            });
        }
    }
}
