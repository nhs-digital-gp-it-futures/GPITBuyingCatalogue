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
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
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
        public static async Task Get_Index_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<OrderItem> orderItems,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            CatalogueSolutionsController controller)
        {
            var expectedViewData = new CatalogueSolutionsModel(internalOrgId, order, orderItems);

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            orderItemServiceMock.Setup(s => s.GetOrderItems(order.CallOffId, internalOrgId, CatalogueItemType.Solution))
                .ReturnsAsync(orderItems);

            var actualResult = await controller.Index(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
            orderSessionServiceMock.Verify(v => v.ClearSession(order.CallOffId), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolution_AvailableSolutions_ReturnsExpectedResult(
            string internalOrgId,
            List<CatalogueItem> solutions,
            EntityFramework.Ordering.Models.Order order,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            CatalogueSolutionsController controller)
        {
            var expectedViewData = new SelectSolutionModel(internalOrgId, state.CallOffId, solutions, state.CatalogueItemId);

            orderServiceMock.Setup(s => s.GetOrderThin(state.CallOffId, internalOrgId)).ReturnsAsync(order);

            orderSessionServiceMock.Setup(s => s.InitialiseStateForCreate(order, CatalogueItemType.Solution, null, null)).Returns(state);

            solutionsServiceMock
                .Setup(s => s.GetSupplierSolutions(order.SupplierId))
                .ReturnsAsync(solutions);

            var actualResult = await controller.SelectSolution(internalOrgId, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSolutionPrice_ReturnsExpectedResult(
            string internalOrgId,
            CreateOrderItemModel state,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            CatalogueSolutionsController controller)
        {
            foreach (CataloguePrice catalogueItemCataloguePrice in catalogueItem.CataloguePrices)
            {
                catalogueItemCataloguePrice.CataloguePriceType = CataloguePriceType.Flat;
                catalogueItemCataloguePrice.CurrencyCode = "GBP";
                catalogueItemCataloguePrice.PublishedStatus = PublicationStatus.Published;
            }

            var prices = catalogueItem.CataloguePrices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();

            var expectedViewData = new SelectSolutionPriceModel(internalOrgId, state.CatalogueItemName, prices);

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            solutionsServiceMock.Setup(s => s.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault())).ReturnsAsync(catalogueItem);

            var actualResult = await controller.SelectSolutionPrice(internalOrgId, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_SelectFlatDeclarativeQuantity_ReturnsExpectedResult(
            string internalOrgId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionsController controller)
        {
            var expectedViewData = new SelectFlatDeclarativeQuantityModel(state.CallOffId, state.CatalogueItemName, state.Quantity);

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = controller.SelectFlatDeclarativeQuantity(internalOrgId, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_SelectFlatOnDemandQuantity_ReturnsExpectedResult(
            string internalOrgId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionsController controller)
        {
            var expectedViewData = new SelectFlatOnDemandQuantityModel(internalOrgId, state.CallOffId, state.CatalogueItemName, state.Quantity, state.EstimationPeriod);

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = controller.SelectFlatOnDemandQuantity(internalOrgId, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditSolution_ReturnsExpectedResult(
            string internalOrgId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            CatalogueSolutionsController controller)
        {
            var expectedViewData = new EditSolutionModel(internalOrgId, state);

            orderSessionServiceMock.Setup(s => s.InitialiseStateForEdit(internalOrgId, state.CallOffId, state.CatalogueItemId.GetValueOrDefault())).ReturnsAsync(state);

            var actualResult = await controller.EditSolution(internalOrgId, state.CallOffId, state.CatalogueItemId.GetValueOrDefault());

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }
    }
}
