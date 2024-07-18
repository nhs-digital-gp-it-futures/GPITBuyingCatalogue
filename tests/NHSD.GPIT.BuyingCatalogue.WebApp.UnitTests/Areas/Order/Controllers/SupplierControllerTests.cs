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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Supplier_NoSupplier_RedirectsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            SupplierController controller)
        {
            order.Supplier = null;

            orderServiceMock
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

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
        [MockAutoData]
        public static async Task Get_Supplier_WithSupplier_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] ISupplierService supplierServiceMock,
            SupplierController controller)
        {
            var model = new SupplierModel(internalOrgId, order.CallOffId, order)
            {
                Contacts = supplier.SupplierContacts.ToList(),
                SelectedContactId = SupplierContact.TemporaryContactId,
            };

            orderServiceMock
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            supplierServiceMock
                .GetSupplierFromBuyingCatalogue(order.Supplier.Id)
                .Returns(supplier);

            var result = await controller.Supplier(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            actualResult.ViewData.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Supplier_WithTemporaryContact_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISupplierService mockSupplierService,
            [Frozen] ISupplierContactSessionService mockSessionService,
            SupplierController controller)
        {
            var model = new SupplierModel(internalOrgId, order.CallOffId, order)
            {
                Contacts = supplier.SupplierContacts.ToList(),
                SelectedContactId = SupplierContact.TemporaryContactId,
            };

            mockOrderService
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            mockSupplierService
                .GetSupplierFromBuyingCatalogue(order.Supplier.Id)
                .Returns(supplier);

            SupplierContact actual = null;

            mockSessionService
                .When(x => x.SetSupplierContact(order.CallOffId, supplier.Id, Arg.Any<SupplierContact>()))
                .Do(x => actual = x.ArgAt<SupplierContact>(2));

            var result = await controller.Supplier(internalOrgId, order.CallOffId);

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
        [MockAutoData]
        public static async Task Post_Supplier_UpdatesContact_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            SupplierModel model,
            Supplier supplier,
            [Frozen] ISupplierService mockSupplierService,
            SupplierController controller)
        {
            var supplierContact = supplier.SupplierContacts.First();

            model.SupplierId = supplier.Id;
            model.SelectedContactId = supplierContact.Id;

            mockSupplierService
                .GetSupplierFromBuyingCatalogue(supplier.Id)
                .Returns(supplier);

            mockSupplierService
                .AddOrUpdateOrderSupplierContact(callOffId, internalOrgId, supplierContact)
                .Returns(Task.CompletedTask);

            var result = await controller.Supplier(internalOrgId, callOffId, model);

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
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static async Task Get_SelectSupplier_WithSupplier_RedirectsCorrectly(
            OrderTypeEnum orderType,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            SupplierController controller)
        {
            order.OrderType = orderType;

            orderServiceMock
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

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
        [MockAutoData]
        public static async Task Get_SelectSupplier_NoSupplier_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<Supplier> suppliers,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] ISupplierService mockSupplierService,
            SupplierController controller)
        {
            order.Supplier = null;

            orderServiceMock
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            mockSupplierService
                .GetActiveSuppliers(OrderTypeEnum.Solution, Arg.Any<string>())
                .Returns(suppliers);

            var result = await controller.SelectSupplier(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = actualResult.ViewData.Model.Should().BeAssignableTo<SelectSupplierModel>().Subject;

            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.OrderType.Should().Be(order.OrderType);

            foreach (var supplier in suppliers)
            {
                model.Suppliers.Should().Contain(x => x.Text == supplier.Name && x.Value == $"{supplier.Id}");
            }
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static async Task Get_SelectSupplier_Split_Or_Merger_NoSupplier_SingleSearchResult_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] IOrderService orderService,
            [Frozen] ISupplierService supplierService,
            SupplierController controller)
        {
            order.Supplier = null;
            order.OrderType = orderType;

            orderService
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            supplierService
                .GetActiveSuppliers(order.OrderType, Arg.Any<string>())
                .Returns(new List<Supplier>() { supplier });

            var result = await controller.SelectSupplier(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SupplierController.ConfirmSupplier));
            actualResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static async Task Get_SelectSupplier_Split_Or_Merger_NoSupplier_NoSearchResult_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            [Frozen] ISupplierService supplierService,
            SupplierController controller)
        {
            order.Supplier = null;
            order.OrderType = orderType;

            orderService
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            supplierService
                .GetActiveSuppliers(order.OrderType, Arg.Any<string>())
                .Returns(new List<Supplier>());

            var result = await controller.SelectSupplier(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ActionName.Should().Be(nameof(SupplierController.NoAvailableSuppliers));
            actualResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
        public static async Task Get_SelectSupplier_Other_Split_Or_Merger_NoSupplier_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            List<Supplier> suppliers,
            [Frozen] IOrderService orderService,
            [Frozen] ISupplierService supplierService,
            SupplierController controller)
        {
            order.Supplier = null;
            order.OrderType = orderType;

            orderService
                .GetOrderWithSupplier(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            supplierService
                .GetActiveSuppliers(orderType, Arg.Any<string>())
                .Returns(suppliers);

            var result = await controller.SelectSupplier(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = actualResult.ViewData.Model.Should().BeAssignableTo<SelectSupplierModel>().Subject;

            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.OrderType.Should().Be(order.OrderType);

            foreach (var supplier in suppliers)
            {
                model.Suppliers.Should().Contain(x => x.Text == supplier.Name && x.Value == $"{supplier.Id}");
            }
        }

        [Theory]
        [MockInlineAutoData(OrderTypeEnum.Solution)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceOther)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit)]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger)]
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
        [MockAutoData]
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
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceSplit, "Split")]
        [MockInlineAutoData(OrderTypeEnum.AssociatedServiceMerger, "Merger")]
        public static async Task Get_NoAvailableSuppliers_ReturnsExpectedResult(
            OrderTypeEnum orderType,
            string expectedOrderTypeText,
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            SupplierController controller)
        {
            order.OrderType = orderType;

            mockOrderService
                .GetOrderThin(order.CallOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            var result = await controller.NoAvailableSuppliers(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = actualResult.ViewData.Model.Should().BeAssignableTo<NoAvailableSuppliersModel>().Subject;

            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(internalOrgId);
            model.OrderTypeText.Should().Be(expectedOrderTypeText);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmSupplier_ExistingSupplierOnOrder_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            Supplier supplier,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            SupplierController controller)
        {
            order.Supplier = supplier;

            mockOrderService
                .GetOrderWithSupplier(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, order.Supplier.Id);

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.Supplier));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmSupplier_SupplierIdNotFound_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            int supplierId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            [Frozen] ISupplierService mockSupplierService,
            SupplierController controller)
        {
            order.Supplier = null;

            orderService
                .GetOrderWithSupplier(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            mockSupplierService
                .GetSupplierFromBuyingCatalogue(supplierId)
                .Returns((Supplier)null);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, supplierId);

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.SelectSupplier));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ConfirmSupplier_EverythingOk_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int supplierId,
            EntityFramework.Ordering.Models.Order order,
            Supplier supplier,
            [Frozen] IOrderService orderService,
            [Frozen] ISupplierService mockSupplierService,
            SupplierController controller)
        {
            order.Supplier = null;

            orderService
                .GetOrderWithSupplier(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            mockSupplierService
                .GetSupplierFromBuyingCatalogue(supplierId)
                .Returns(supplier);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, supplierId);

            var expected = new ConfirmSupplierModel(internalOrgId, callOffId, supplier);

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_ConfirmSupplier_ValidModelState_AddsSupplier_RedirectsCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmSupplierModel model,
            [Frozen] ISupplierService mockSupplierService,
            SupplierController controller)
        {
            mockSupplierService
                .AddOrderSupplier(callOffId, internalOrgId, model.SupplierId)
                .Returns(Task.CompletedTask);

            var result = await controller.ConfirmSupplier(internalOrgId, callOffId, model);

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
        [MockAutoData]
        public static async Task Get_NewContact_NoSupplierOnOrder_ReturnsRedirectResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            SupplierController systemUnderTest)
        {
            order.SupplierId = null;

            mockOrderService
                .GetOrderWithSupplier(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            var result = await systemUnderTest.NewContact(internalOrgId, callOffId);

            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.Should().NotBeNull();
            redirectResult.ActionName.Should().Be(nameof(SupplierController.SelectSupplier));
            redirectResult.ControllerName.Should().Be(typeof(SupplierController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NewContact_NoSupplierContactInSession_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int supplierId,
            string supplierName,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService mockOrderService,
            SupplierController systemUnderTest)
        {
            var model = new NewContactModel(callOffId, supplierId, supplierName)
            {
                Title = "Add a contact",
            };

            order.SupplierId = supplierId;
            order.Supplier.Name = supplierName;

            mockOrderService
                .GetOrderWithSupplier(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            var result = await systemUnderTest.NewContact(internalOrgId, callOffId);

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NewContact_SupplierContactInSession_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            int supplierId,
            string supplierName,
            EntityFramework.Ordering.Models.Order order,
            SupplierContact supplierContact,
            [Frozen] IOrderService mockOrderService,
            [Frozen] ISupplierContactSessionService mockSessionService,
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
                .GetOrderWithSupplier(callOffId, internalOrgId)
                .Returns(new OrderWrapper(order));

            mockSessionService
                .GetSupplierContact(callOffId, supplierId)
                .Returns(supplierContact);

            var result = await systemUnderTest.NewContact(internalOrgId, callOffId);

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(model, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static void Post_NewContact_AddsContactToSession_RedirectsCorrectly(
            string internalOrgId,
            CallOffId callOffId,
            NewContactModel model,
            [Frozen] ISupplierContactSessionService mockSessionService,
            SupplierController systemUnderTest)
        {
            SupplierContact supplierContact = null;

            mockSessionService
                .When(x => x.SetSupplierContact(callOffId, model.SupplierId, Arg.Any<SupplierContact>()))
                .Do(x => supplierContact = x.ArgAt<SupplierContact>(2));

            var result = systemUnderTest.NewContact(internalOrgId, callOffId, model);

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
