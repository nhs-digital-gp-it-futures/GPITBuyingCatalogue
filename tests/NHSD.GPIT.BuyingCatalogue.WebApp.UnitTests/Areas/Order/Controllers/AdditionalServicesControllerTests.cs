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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class AdditionalServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AdditionalServicesController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(AdditionalServicesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
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
        public static async Task Get_Index_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            List<OrderItem> orderItems,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            AdditionalServicesController controller)
        {
            var expectedViewData = new AdditionalServiceModel(odsCode, order, orderItems);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            orderItemServiceMock.Setup(s => s.GetOrderItems(order.CallOffId, CatalogueItemType.AdditionalService))
                .ReturnsAsync(orderItems);

            var actualResult = await controller.Index(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
            orderSessionServiceMock.Verify(v => v.ClearSession(order.CallOffId), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalService_NoServices_ReturnsExpectedResult(
            string odsCode,
            CallOffId callOffId,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesServiceMock,
            AdditionalServicesController controller)
        {
            var expectedViewData = new NoAdditionalServicesFoundModel();

            additionalServicesServiceMock
                .Setup(s => s.GetAdditionalServicesBySolutionIds(It.IsAny<IEnumerable<CatalogueItemId>>()))
                .ReturnsAsync(new List<CatalogueItem>());

            var actualResult = await controller.SelectAdditionalService(odsCode, callOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalService_AvailableServices_ReturnsExpectedResult(
            string odsCode,
            List<CatalogueItem> additionalServices,
            EntityFramework.Ordering.Models.Order order,
            List<OrderItem> orderItems,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesServiceMock,
            AdditionalServicesController controller)
        {
            var expectedViewData = new SelectAdditionalServiceModel(odsCode, state.CallOffId, additionalServices, state.CatalogueItemId);

            orderServiceMock.Setup(s => s.GetOrder(state.CallOffId)).ReturnsAsync(order);

            orderItemServiceMock.Setup(s => s.GetOrderItems(state.CallOffId, CatalogueItemType.Solution))
                .ReturnsAsync(orderItems);

            var solutionIds = orderItems.Select(i => i.CatalogueItemId).ToList();

            orderSessionServiceMock.Setup(s => s.InitialiseStateForCreate(order, CatalogueItemType.AdditionalService, solutionIds, null)).Returns(state);

            additionalServicesServiceMock
                .Setup(s => s.GetAdditionalServicesBySolutionIds(It.IsAny<IEnumerable<CatalogueItemId>>()))
                .ReturnsAsync(additionalServices);

            var actualResult = await controller.SelectAdditionalService(odsCode, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAdditionalServicePrice_ReturnsExpectedResult(
            string odsCode,
            CreateOrderItemModel state,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            AdditionalServicesController controller)
        {
            foreach (CataloguePrice catalogueItemCataloguePrice in catalogueItem.CataloguePrices)
            {
                catalogueItemCataloguePrice.CataloguePriceType = CataloguePriceType.Flat;
                catalogueItemCataloguePrice.CurrencyCode = "GBP";
            }

            var prices = catalogueItem.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            var expectedViewData = new SelectAdditionalServicePriceModel(odsCode, state.CallOffId, state.CatalogueItemName, prices);

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            solutionsServiceMock.Setup(s => s.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault())).ReturnsAsync(catalogueItem);

            var actualResult = await controller.SelectAdditionalServicePrice(odsCode, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_SelectFlatDeclarativeQuantity_ReturnsExpectedResult(
          string odsCode,
          CreateOrderItemModel state,
          [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
          AdditionalServicesController controller)
        {
            var expectedViewData = new SelectFlatDeclarativeQuantityModel(state.CallOffId, state.CatalogueItemName, state.Quantity);

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = controller.SelectFlatDeclarativeQuantity(odsCode, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_SelectFlatOnDemandQuantity_ReturnsExpectedResult(
          string odsCode,
          CreateOrderItemModel state,
          [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
          AdditionalServicesController controller)
        {
            var expectedViewData = new SelectFlatOnDemandQuantityModel(odsCode, state.CallOffId, state.CatalogueItemName, state.Quantity, state.EstimationPeriod);

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            var actualResult = controller.SelectFlatOnDemandQuantity(odsCode, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAdditionalService_ReturnsExpectedResult(
          string odsCode,
          CreateOrderItemModel state,
          [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
          AdditionalServicesController controller)
        {
            var expectedViewData = new EditAdditionalServiceModel(odsCode, state);

            orderSessionServiceMock.Setup(s => s.InitialiseStateForEdit(odsCode, state.CallOffId, state.CatalogueItemId.GetValueOrDefault())).ReturnsAsync(state);

            var actualResult = await controller.EditAdditionalService(odsCode, state.CallOffId, state.CatalogueItemId.GetValueOrDefault());

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }
    }
}
