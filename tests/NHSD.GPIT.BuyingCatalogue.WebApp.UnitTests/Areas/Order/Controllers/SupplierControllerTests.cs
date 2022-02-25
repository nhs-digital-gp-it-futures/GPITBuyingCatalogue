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
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            order.Supplier = null;

            orderServiceMock.Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var actualResult = await controller.Supplier(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.SupplierSearch));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Supplier_WithSupplier_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var expectedViewData = new SupplierModel(internalOrgId, order, supplier.SupplierContacts);

            orderServiceMock.Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId)).ReturnsAsync(order);
            supplierServiceMock.Setup(s => s.GetSupplierFromBuyingCatalogue(order.Supplier.Id)).ReturnsAsync(supplier);

            var actualResult = await controller.Supplier(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Supplier_UpdatesContact_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            SupplierModel model,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var actualResult = await controller.Supplier(internalOrgId, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId } });
            supplierServiceMock.Verify(s => s.AddOrUpdateOrderSupplierContact(callOffId, internalOrgId, model.PrimaryContact), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearch_WithSupplier_RedirectsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            orderServiceMock.Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var actualResult = await controller.SupplierSearch(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.Supplier));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearch_NoSupplier_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            order.Supplier = null;

            var expectedViewData = new SupplierSearchModel(internalOrgId, order);

            orderServiceMock.Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var actualResult = await controller.SupplierSearch(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SupplierSearch_RedirectsCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            SupplierSearchModel model,
            SupplierController controller)
        {
            var actualResult = controller.SupplierSearch(internalOrgId, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.SupplierSearchSelect));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", callOffId }, { "search", model.SearchString } });
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static async Task Get_SupplierSearchSelect_NoSearch_ReturnsExpectedResult(
            string search,
            string internalOrgId,
            CallOffId callOffId,
            SupplierController controller)
        {
            var expectedViewData = new NoSupplierFoundModel();

            var actualResult = await controller.SupplierSearchSelect(internalOrgId, callOffId, search);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
            actualResult.As<ViewResult>().ViewName.Should().Be("NoSupplierFound");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearchSelect_NoSuppliersFound_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var expectedViewData = new NoSupplierFoundModel();

            supplierServiceMock.Setup(s => s.GetListFromBuyingCatalogue(It.IsAny<string>(), null, null))
                .ReturnsAsync(new List<Supplier>());

            var actualResult = await controller.SupplierSearchSelect(internalOrgId, callOffId, "searchTerm");

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
            actualResult.As<ViewResult>().ViewName.Should().Be("NoSupplierFound");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearchSelect_SuppliersFound_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            List<Supplier> suppliers,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var expectedViewData = new SupplierSearchSelectModel(internalOrgId, callOffId, suppliers);

            supplierServiceMock.Setup(s => s.GetListFromBuyingCatalogue(It.IsAny<string>(), null, null))
                .ReturnsAsync(suppliers);

            var actualResult = await controller.SupplierSearchSelect(internalOrgId, callOffId, "searchTerm");

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierSearchSelect_ExistingSupplierOnOrder_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            string search,
            Supplier supplier,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            order.Supplier = supplier;
            orderServiceMock.Setup(_ => _.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.SupplierSearchSelect(internalOrgId, callOffId, search);

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.Supplier));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupplierSearchSelect_ValidModelState_AddsSupplier_RedirectsCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            string search,
            int supplierId,
            SupplierSearchSelectModel model,
            SupplierController controller)
        {
            model.SelectedSupplierId = supplierId;

            var actualResult = await controller.SupplierSearchSelect(internalOrgId, callOffId, model, search);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.ConfirmSupplier));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "search", search },
                { "supplierId", supplierId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSupplier_ExistingSupplierOnOrder_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            string search,
            Supplier supplier,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            SupplierController controller)
        {
            order.Supplier = supplier;

            mockOrderService
                .Setup(_ => _.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(order);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, search, order.Supplier.Id);

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.Supplier));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSupplier_SupplierIdNotFound_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            string search,
            int supplierId,
            [Frozen] Mock<ISupplierService> mockSupplierService,
            SupplierController controller)
        {
            mockSupplierService
                .Setup(x => x.GetSupplierFromBuyingCatalogue(supplierId))
                .ReturnsAsync((Supplier)null);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, search, supplierId);

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.SupplierSearch));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSupplier_EverythingOk_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            List<Supplier> suppliers,
            [Frozen] Mock<ISupplierService> supplierServiceMock,
            SupplierController controller)
        {
            var model = new SupplierSearchSelectModel(internalOrgId, callOffId, suppliers);

            supplierServiceMock
                .Setup(s => s.GetListFromBuyingCatalogue(It.IsAny<string>(), null, null))
                .ReturnsAsync(suppliers);

            var result = await controller.SupplierSearchSelect(internalOrgId, callOffId, "search");

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConfirmSupplier_ValidModelState_AddsSupplier_RedirectsCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmSupplierModel model,
            [Frozen] Mock<ISupplierService> mockSupplierService,
            SupplierController controller)
        {
            mockSupplierService
                .Setup(x => x.AddOrderSupplier(callOffId, internalOrgId, model.SupplierId))
                .Returns(Task.CompletedTask);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, model);

            mockSupplierService.VerifyAll();

            result.Should().BeOfType<RedirectToActionResult>();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.Supplier));
            result.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            result.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }
    }
}
