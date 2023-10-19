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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class SupplierControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SupplierController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(SupplierController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
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

            orderServiceMock
                .Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var actualResult = await controller.Supplier(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierController.SelectSupplier));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
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
            var model = new SupplierModel(internalOrgId, order.CallOffId, order)
            {
                Contacts = supplier.SupplierContacts.ToList(),
                SelectedContactId = SupplierContact.TemporaryContactId,
            };

            orderServiceMock
                .Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            supplierServiceMock
                .Setup(s => s.GetSupplierFromBuyingCatalogue(order.Supplier.Id))
                .ReturnsAsync(supplier);

            var result = await controller.Supplier(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            supplierServiceMock.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewData.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Supplier_WithTemporaryContact_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISupplierService> mockSupplierService,
            [Frozen] Mock<ISupplierContactSessionService> mockSessionService,
            SupplierController controller)
        {
            var model = new SupplierModel(internalOrgId, order.CallOffId, order)
            {
                Contacts = supplier.SupplierContacts.ToList(),
                SelectedContactId = SupplierContact.TemporaryContactId,
            };

            mockOrderService
                .Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSupplierService
                .Setup(s => s.GetSupplierFromBuyingCatalogue(order.Supplier.Id))
                .ReturnsAsync(supplier);

            SupplierContact actual = null;

            mockSessionService
                .Setup(x => x.SetSupplierContact(order.CallOffId, supplier.Id, It.IsAny<SupplierContact>()))
                .Callback<CallOffId, int, SupplierContact>((_, _, x) => actual = x)
                .Verifiable();

            var result = await controller.Supplier(internalOrgId, order.CallOffId);

            mockOrderService.VerifyAll();
            mockSupplierService.VerifyAll();
            mockSessionService.VerifyAll();

            actual.Id.Should().Be(SupplierContact.TemporaryContactId);
            actual.SupplierId.Should().Be(supplier.Id);
            actual.FirstName.Should().Be(order.SupplierContact.FirstName);
            actual.LastName.Should().Be(order.SupplierContact.LastName);
            actual.Department.Should().Be(order.SupplierContact.Department);
            actual.PhoneNumber.Should().Be(order.SupplierContact.Phone);
            actual.Email.Should().Be(order.SupplierContact.Email);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            actualResult.ViewData.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Supplier_UpdatesContact_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            SupplierModel model,
            Supplier supplier,
            [Frozen] Mock<ISupplierService> mockSupplierService,
            SupplierController controller)
        {
            var supplierContact = supplier.SupplierContacts.First();

            model.SupplierId = supplier.Id;
            model.SelectedContactId = supplierContact.Id;

            mockSupplierService
                .Setup(x => x.GetSupplierFromBuyingCatalogue(supplier.Id))
                .ReturnsAsync(supplier);

            mockSupplierService
                .Setup(x => x.AddOrUpdateOrderSupplierContact(callOffId, internalOrgId, supplierContact))
                .Returns(Task.CompletedTask);

            var result = await controller.Supplier(internalOrgId, callOffId, model);

            mockSupplierService.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonInlineAutoData(true)]
        [CommonInlineAutoData(false)]
        public static async Task Get_SelectSupplier_WithSupplier_RedirectsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            SupplierController controller)
        {
            orderServiceMock
                .Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SelectSupplier(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SupplierController.Supplier));
            actualResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SelectSupplier_NoSupplier_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<Supplier> suppliers,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<ISupplierService> mockSupplierService,
            SupplierController controller)
        {
            order.Supplier = null;

            orderServiceMock
                .Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSupplierService
                .Setup(x => x.GetAllSuppliersFromBuyingCatalogue())
                .ReturnsAsync(suppliers);

            var result = await controller.SelectSupplier(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            mockSupplierService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = actualResult.ViewData.Model.Should().BeAssignableTo<SelectSupplierModel>().Subject;

            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.OrderType.Should().Be(order.OrderTypeValue);

            foreach (var supplier in suppliers)
            {
                model.Suppliers.Should().Contain(x => x.Text == supplier.Name && x.Value == $"{supplier.Id}");
            }
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther, PracticeReorganisationTypeEnum.None)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, PracticeReorganisationTypeEnum.Split)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, PracticeReorganisationTypeEnum.Merger)]
        public static async Task Get_SelectSupplier_Other_Split_Or_Merger_NoSupplier_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            PracticeReorganisationTypeEnum practiceReorganisationType,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<Supplier> suppliers,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<ISupplierService> supplierService,
            SupplierController controller)
        {
            order.Supplier = null;
            order.OrderType = orderType;

            orderService
                .Setup(s => s.GetOrderWithSupplier(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            supplierService
                .Setup(x => x.GetAllSuppliersWithAssociatedServices(practiceReorganisationType))
                .ReturnsAsync(suppliers);

            var result = await controller.SelectSupplier(internalOrgId, order.CallOffId);

            orderService.VerifyAll();
            supplierService.VerifyAll();

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = actualResult.ViewData.Model.Should().BeAssignableTo<SelectSupplierModel>().Subject;

            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.OrderType.Should().Be(order.OrderTypeValue);

            foreach (var supplier in suppliers)
            {
                model.Suppliers.Should().Contain(x => x.Text == supplier.Name && x.Value == $"{supplier.Id}");
            }
        }

        [Theory]
        [CommonInlineAutoData(OrderTypeEnum.Solution)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [CommonInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static void Post_SelectSupplier_WithModelErrors_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            SelectSupplierModel model,
            SupplierController controller)
        {
            controller.ModelState.AddModelError("key", "errorMessage");

            model.OrderType = new OrderType(orderType);

            var result = controller.SelectSupplier(model.InternalOrgId, model.CallOffId, model);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var actualModel = actualResult.ViewData.Model.Should().BeAssignableTo<SelectSupplierModel>().Subject;

            actualModel.CallOffId.Should().Be(model.CallOffId);
            actualModel.InternalOrgId.Should().Be(model.InternalOrgId);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_SelectSupplier_RedirectsCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            SelectSupplierModel model,
            SupplierController controller)
        {
            model.SelectedSupplierId = $"{1}";

            var result = controller.SelectSupplier(internalOrgId, callOffId, model);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SupplierController.ConfirmSupplier));
            actualResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "supplierId", int.Parse(model.SelectedSupplierId) },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSupplier_ExistingSupplierOnOrder_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            Supplier supplier,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            SupplierController controller)
        {
            order.Supplier = supplier;

            mockOrderService
                .Setup(_ => _.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, order.Supplier.Id);

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
            int supplierId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<ISupplierService> mockSupplierService,
            SupplierController controller)
        {
            order.Supplier = null;

            orderService
                .Setup(x => x.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSupplierService
                .Setup(x => x.GetSupplierFromBuyingCatalogue(supplierId))
                .ReturnsAsync((Supplier)null);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, supplierId);

            orderService.VerifyAll();
            mockSupplierService.VerifyAll();

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.SelectSupplier));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConfirmSupplier_EverythingOk_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int supplierId,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] Mock<IOrderService> orderService,
            [Frozen] Mock<ISupplierService> mockSupplierService,
            SupplierController controller)
        {
            order.Supplier = null;

            orderService
                .Setup(x => x.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSupplierService
                .Setup(x => x.GetSupplierFromBuyingCatalogue(supplierId))
                .ReturnsAsync(supplier);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, supplierId);

            orderService.VerifyAll();
            mockSupplierService.VerifyAll();

            var expected = new ConfirmSupplierModel(internalOrgId, callOffId, supplier);

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
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

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SupplierController.Supplier));
            actualResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NewContact_NoSupplierOnOrder_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            SupplierController systemUnderTest)
        {
            order.SupplierId = null;

            mockOrderService
                .Setup(_ => _.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await systemUnderTest.NewContact(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.SelectSupplier));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NewContact_NoSupplierContactInSession_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int supplierId,
            string supplierName,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> mockOrderService,
            SupplierController systemUnderTest)
        {
            var model = new NewContactModel(callOffId, supplierId, supplierName)
            {
                Title = "Add a contact",
            };

            order.SupplierId = supplierId;
            order.Supplier.Name = supplierName;

            mockOrderService
                .Setup(x => x.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await systemUnderTest.NewContact(internalOrgId, callOffId);

            mockOrderService.VerifyAll();

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NewContact_SupplierContactInSession_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int supplierId,
            string supplierName,
            EntityFramework.Ordering.Models.Order order,
            SupplierContact supplierContact,
            [Frozen] Mock<IOrderService> mockOrderService,
            [Frozen] Mock<ISupplierContactSessionService> mockSessionService,
            SupplierController systemUnderTest)
        {
            var model = new NewContactModel(callOffId, supplierId, supplierName)
            {
                Title = $"{supplierContact.FirstName} {supplierContact.LastName} details",
                FirstName = supplierContact.FirstName,
                LastName = supplierContact.LastName,
                Department = supplierContact.Department,
                PhoneNumber = supplierContact.PhoneNumber,
                Email = supplierContact.Email,
            };

            order.SupplierId = supplierId;
            order.Supplier.Name = supplierName;

            mockOrderService
                .Setup(x => x.GetOrderWithSupplier(callOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            mockSessionService
                .Setup(x => x.GetSupplierContact(callOffId, supplierId))
                .Returns(supplierContact);

            var result = await systemUnderTest.NewContact(internalOrgId, callOffId);

            mockOrderService.VerifyAll();
            mockSessionService.VerifyAll();

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static void Post_NewContact_AddsContactToSession_RedirectsCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            NewContactModel model,
            [Frozen] Mock<ISupplierContactSessionService> mockSessionService,
            SupplierController systemUnderTest)
        {
            SupplierContact supplierContact = null;

            mockSessionService
                .Setup(x => x.SetSupplierContact(callOffId, model.SupplierId, It.IsAny<SupplierContact>()))
                .Callback<CallOffId, int, SupplierContact>((_, _, x) => supplierContact = x)
                .Verifiable();

            var result = systemUnderTest.NewContact(internalOrgId, callOffId, model);

            mockSessionService.VerifyAll();

            supplierContact.Id.Should().Be(SupplierContact.TemporaryContactId);
            supplierContact.SupplierId.Should().Be(model.SupplierId);
            supplierContact.FirstName.Should().Be(model.FirstName);
            supplierContact.LastName.Should().Be(model.LastName);
            supplierContact.Department.Should().Be(model.Department);
            supplierContact.PhoneNumber.Should().Be(model.PhoneNumber);
            supplierContact.Email.Should().Be(model.Email);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SupplierController.Supplier));
            actualResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
                { "selected", SupplierContact.TemporaryContactId },
            });
        }
    }
}
