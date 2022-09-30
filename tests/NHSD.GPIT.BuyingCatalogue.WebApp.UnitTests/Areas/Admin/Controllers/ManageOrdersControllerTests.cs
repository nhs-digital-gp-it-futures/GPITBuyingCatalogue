using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;
using DeleteOrderModel = NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders.DeleteOrderModel;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class ManageOrdersControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ManageOrdersController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ReturnsViewWithModel(
            PagedList<AdminManageOrder> orders,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            ManageOrdersController controller)
        {
            var expectedModel = new ManageOrdersDashboardModel(orders.Items, orders.Options);
            orderAdminService.Setup(s => s.GetPagedOrders(It.IsAny<PageOptions>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(orders);

            var result = (await controller.Index()).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ViewOrder_ReturnsViewWithModel(
            AspNetUser user,
            Organisation organisation,
            Supplier supplier,
            OrderItem orderItem,
            Solution solution,
            FrameworkSolution[] frameworkSolutions,
            EntityFramework.Ordering.Models.Order order,
            EntityFramework.Catalogue.Models.Framework framework,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            [Frozen] Mock<IFrameworkService> mockFrameworkService,
            ManageOrdersController controller)
        {
            solution.FrameworkSolutions = frameworkSolutions;

            orderItem.CatalogueItem = solution.CatalogueItem;

            order.OrderItems.Add(orderItem);

            order.OrderingParty = organisation;
            order.LastUpdatedByUser = user;
            order.Supplier = supplier;

            orderAdminService.Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            mockFrameworkService
                .Setup(x => x.GetFramework(order.Id))
                .ReturnsAsync(framework);

            var result = (await controller.ViewOrder(order.CallOffId)).As<ViewResult>();

            var expected = new ViewOrderModel(order, framework);

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Download_CompleteOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderAdminService> orderServiceMock,
            [Frozen] Mock<IPdfService> pdfServiceMock,
            byte[] fileContents,
            ManageOrdersController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock
                .Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            pdfServiceMock
                .Setup(s => s.Convert(It.IsAny<System.Uri>()))
                .Returns(fileContents);

            SetControllerHttpContext(controller);

            var result = (await controller.Download(order.CallOffId, internalOrgId)).As<FileContentResult>();

            orderServiceMock.VerifyAll();
            pdfServiceMock.VerifyAll();

            result.Should().NotBeNull();
            result.ContentType.Should().Be("application/pdf");
            result.FileDownloadName.Should().Be($"order-summary-completed-{order.CallOffId}.pdf");
            result.FileContents.Should().BeEquivalentTo(fileContents);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Download_InProgressOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderAdminService> orderServiceMock,
            [Frozen] Mock<IPdfService> pdfServiceMock,
            byte[] fileContents,
            ManageOrdersController controller)
        {
            order.Completed = null;

            orderServiceMock
                .Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            pdfServiceMock
                .Setup(s => s.Convert(It.IsAny<System.Uri>()))
                .Returns(fileContents);

            SetControllerHttpContext(controller);

            var result = (await controller.Download(order.CallOffId, internalOrgId)).As<FileContentResult>();

            orderServiceMock.VerifyAll();
            pdfServiceMock.VerifyAll();

            result.Should().NotBeNull();
            result.ContentType.Should().Be("application/pdf");
            result.FileDownloadName.Should().Be($"order-summary-in-progress-{order.CallOffId}.pdf");
            result.FileContents.Should().BeEquivalentTo(fileContents);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DownloadFullOrderCsv_ReturnsExpectedResult(
            CallOffId callOffId,
            string externalOrgId,
            [Frozen] Mock<ICsvService> csvService,
            ManageOrdersController controller)
        {
            var result = (await controller.DownloadFullOrderCsv(callOffId, externalOrgId)).As<FileContentResult>();

            csvService.VerifyAll();

            result.Should().NotBeNull();
            result.ContentType.Should().Be("application/octet-stream");
            result.FileDownloadName.Should().Be($"{callOffId}_{externalOrgId}_full.csv");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DownloadPatientNumberCsv_ReturnsExpectedResult(
            CallOffId callOffId,
            string externalOrgId,
            [Frozen] Mock<ICsvService> csvService,
            ManageOrdersController controller)
        {
            var result = (await controller.DownloadPatientNumberCsv(callOffId, externalOrgId)).As<FileContentResult>();

            csvService.VerifyAll();

            result.Should().NotBeNull();
            result.ContentType.Should().Be("application/octet-stream");
            result.FileDownloadName.Should().Be($"{callOffId}_{externalOrgId}_patient.csv");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteOrder_ReturnsViewWithModel(
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            ManageOrdersController controller)
        {
            var expectedModel = new DeleteOrderModel(order);

            orderAdminService.Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            var result = (await controller.DeleteOrder(order.CallOffId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteOrder_InvalidModelState(
            CallOffId callOffId,
            DeleteOrderModel model,
            ManageOrdersController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.DeleteOrder(callOffId, model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteOrder_ConfirmedDelete(
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            ManageOrdersController controller)
        {
            var result = (await controller.DeleteOrder(callOffId, model)).As<RedirectToActionResult>();

            orderAdminService.Verify(s => s.DeleteOrder(callOffId, model.NameOfRequester, model.NameOfApprover, model.ApprovalDate!.Value), Times.Once());

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.Index));
        }

        private static void SetControllerHttpContext(ControllerBase controller)
        {
            const int UserId = 1;
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Scheme = "https", Host = new HostString("localhost") },
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(CatalogueClaims.UserId, $"{UserId}"),
                    })),
                },
            };
        }
    }
}
