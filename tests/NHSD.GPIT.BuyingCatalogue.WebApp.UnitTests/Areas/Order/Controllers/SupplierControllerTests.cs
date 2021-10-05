using System.Collections.Generic;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class SupplierControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SupplierController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(SupplierController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Supplier_NoSupplier_RedirectsCorrectly(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            order.Supplier = null;

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.Supplier(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.SupplierSearch));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Supplier_WithSupplier_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var expectedViewData = new SupplierModel(odsCode, order, supplier.SupplierContacts);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);
            supplierServiceMock.Setup(s => s.GetSupplierFromBuyingCatalogue(order.Supplier.Id)).ReturnsAsync(supplier);

            var actualResult = await controller.Supplier(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Supplier_UpdatesContact_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            SupplierModel model,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var actualResult = await controller.Supplier(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", callOffId } });
            supplierServiceMock.Verify(s => s.AddOrUpdateOrderSupplierContact(callOffId, model.PrimaryContact), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearch_WithSupplier_RedirectsCorrectly(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.SupplierSearch(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.Supplier));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearch_NoSupplier_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            order.Supplier = null;

            var expectedViewData = new SupplierSearchModel(odsCode, order);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.SupplierSearch(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SupplierSearch_RedirectsCorrectly(
            string odsCode,
            CallOffId callOffId,
            SupplierSearchModel model,
            SupplierController controller)
        {
            var actualResult = controller.SupplierSearch(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.SupplierSearchSelect));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", callOffId }, { "search", model.SearchString } });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static async Task Get_SupplierSearchSelect_NoSearch_ReturnsExpectedResult(string search)
        {
            var fixture = new Fixture().Customize(new CompositeCustomization(
                new AutoMoqCustomization(),
                new OrderCustomization(),
                new CallOffIdCustomization(),
                new CatalogueItemIdCustomization(),
                new ControllerCustomization(),
                new IgnoreCircularReferenceCustomisation()));

            var odsCode = fixture.Create<string>();
            var callOffId = fixture.Create<CallOffId>();
            var controller = fixture.Create<SupplierController>();

            var expectedViewData = new NoSupplierFoundModel(odsCode, callOffId);

            var actualResult = await controller.SupplierSearchSelect(odsCode, callOffId, search);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
            actualResult.As<ViewResult>().ViewName.Should().Be("NoSupplierFound");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearchSelect_NoSuppliersFound_ReturnsExpectedResult(
            string odsCode,
            CallOffId callOffId,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var expectedViewData = new NoSupplierFoundModel(odsCode, callOffId);

            supplierServiceMock.Setup(s => s.GetListFromBuyingCatalogue(It.IsAny<string>(), null, null))
                .ReturnsAsync(new List<Supplier>());

            var actualResult = await controller.SupplierSearchSelect(odsCode, callOffId, "searchTerm");

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
            actualResult.As<ViewResult>().ViewName.Should().Be("NoSupplierFound");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearchSelect_SuppliersFound_ReturnsExpectedResult(
            string odsCode,
            CallOffId callOffId,
            List<Supplier> suppliers,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var expectedViewData = new SupplierSearchSelectModel(odsCode, callOffId, suppliers);

            supplierServiceMock.Setup(s => s.GetListFromBuyingCatalogue(It.IsAny<string>(), null, null))
                .ReturnsAsync(suppliers);

            var actualResult = await controller.SupplierSearchSelect(odsCode, callOffId, "searchTerm");

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupplierSearchSelect_ValidModelState_AddsSupplier_RedirectsCorrectly(
            string odsCode,
            CallOffId callOffId,
            SupplierSearchSelectModel model,
            string search,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var actualResult = await controller.SupplierSearchSelect(odsCode, callOffId, model, search);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.Supplier));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", callOffId } });
            supplierServiceMock.Verify(s => s.AddOrderSupplier(callOffId, model.SelectedSupplierId.Value), Times.Once);
        }
    }
}
