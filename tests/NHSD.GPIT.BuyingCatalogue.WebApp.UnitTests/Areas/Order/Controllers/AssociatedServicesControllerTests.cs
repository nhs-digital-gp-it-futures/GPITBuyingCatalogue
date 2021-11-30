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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
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
        public static async Task Get_Index_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            List<OrderItem> orderItems,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IOrderItemService> orderItemServiceMock,
            AssociatedServicesController controller)
        {
            var expectedViewData = new AssociatedServiceModel(odsCode, order, orderItems);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            orderItemServiceMock.Setup(s => s.GetOrderItems(order.CallOffId, CatalogueItemType.AssociatedService))
                .ReturnsAsync(orderItems);

            var actualResult = await controller.Index(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
            orderSessionServiceMock.Verify(v => v.ClearSession(order.CallOffId), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAssociatedService_NoServices_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            Organisation organisation,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IAssociatedServicesService> associatedServicesServiceMock,
            [Frozen] Mock<IOrganisationsService> organisationServiceMock,
            AssociatedServicesController controller)
        {
            var expectedViewData = new NoAssociatedServicesFoundModel();

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            organisationServiceMock.Setup(s => s.GetOrganisationByOdsCode(It.IsAny<string>()))
                .ReturnsAsync(organisation);

            associatedServicesServiceMock
                .Setup(s => s.GetAssociatedServicesForSupplier(It.IsAny<int>()))
                .ReturnsAsync(new List<CatalogueItem>());

            var actualResult = await controller.SelectAssociatedService(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAssociatedService_AvailableServices_ReturnsExpectedResult(
            string odsCode,
            List<CatalogueItem> associatedServices,
            EntityFramework.Ordering.Models.Order order,
            Organisation organisation,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IAssociatedServicesService> associatedServicesServiceMock,
            [Frozen] Mock<IOrganisationsService> organisationServiceMock,
            AssociatedServicesController controller)
        {
            var expectedViewData = new SelectAssociatedServiceModel(odsCode, state.CallOffId, associatedServices, state.CatalogueItemId);

            orderServiceMock.Setup(s => s.GetOrder(state.CallOffId)).ReturnsAsync(order);

            organisationServiceMock.Setup(s => s.GetOrganisationByOdsCode(It.IsAny<string>()))
                .ReturnsAsync(organisation);

            orderSessionServiceMock.Setup(s => s.InitialiseStateForCreate(order, CatalogueItemType.AssociatedService, null, It.IsAny<OrderItemRecipientModel>()))
                .Returns(state);

            associatedServicesServiceMock
                .Setup(s => s.GetAssociatedServicesForSupplier(It.IsAny<int>()))
                .ReturnsAsync(associatedServices);

            var actualResult = await controller.SelectAssociatedService(odsCode, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectAssociatedServicePrice_ReturnsExpectedResult(
            string odsCode,
            CreateOrderItemModel state,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            AssociatedServicesController controller)
        {
            foreach (CataloguePrice catalogueItemCataloguePrice in catalogueItem.CataloguePrices)
            {
                catalogueItemCataloguePrice.CataloguePriceType = CataloguePriceType.Flat;
                catalogueItemCataloguePrice.CurrencyCode = "GBP";
            }

            var prices = catalogueItem.CataloguePrices.Where(p =>
            p.CataloguePriceType == CataloguePriceType.Flat
            && p.PublishedStatus == PublicationStatus.Published).ToList();

            var expectedViewData = new SelectAssociatedServicePriceModel(odsCode, state.CatalogueItemName, prices);

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(state.CallOffId)).Returns(state);

            solutionsServiceMock.Setup(s => s.GetSolutionListPrices(state.CatalogueItemId.GetValueOrDefault())).ReturnsAsync(catalogueItem);

            var actualResult = await controller.SelectAssociatedServicePrice(odsCode, state.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAssociatedService_ReturnsExpectedResult(
            string odsCode,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            AssociatedServicesController controller)
        {
            var expectedViewData = new EditAssociatedServiceModel(odsCode, state);

            orderSessionServiceMock.Setup(s => s.InitialiseStateForEdit(odsCode, state.CallOffId, state.CatalogueItemId.GetValueOrDefault())).ReturnsAsync(state);

            var actualResult = await controller.EditAssociatedService(odsCode, state.CallOffId, state.CatalogueItemId.GetValueOrDefault());

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }
    }
}
