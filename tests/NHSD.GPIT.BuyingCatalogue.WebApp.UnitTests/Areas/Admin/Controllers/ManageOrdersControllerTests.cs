using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;
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
            IEnumerable<FrameworkFilterInfo> frameworks,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            ManageOrdersController controller)
        {
            var expectedModel = new ManageOrdersDashboardModel(orders.Items, frameworks, orders.Options, string.Empty, string.Empty);
            orderAdminService.Setup(s => s.GetPagedOrders(It.IsAny<PageOptions>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(orders);

            var result = (await controller.Index()).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FilterSearchSuggestions_ReturnsResults(
            string searchTerm,
            Uri uri,
            List<SearchFilterModel> searchResults,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            ManageOrdersController controller)
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                Request =
                {
                    Headers = { Referer = uri.ToString(), },
                },
            };

            var requestUri = new UriBuilder(uri.ToString());

            var expected = searchResults.Select(
                r => new HtmlEncodedSuggestionSearchResult(
                    r.Title,
                    r.Category,
                    requestUri.AppendQueryParameterToUrl("search", r.Title).AppendQueryParameterToUrl("searchTermType", r.Category).Uri.PathAndQuery));

            orderAdminService.Setup(s => s.GetOrdersBySearchTerm(searchTerm))
                .ReturnsAsync(searchResults);

            var result = (await controller.FilterSearchSuggestions(searchTerm)).As<JsonResult>();

            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expected);
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
            ManageOrdersController controller)
        {
            solution.FrameworkSolutions = frameworkSolutions;

            orderItem.CatalogueItem = solution.CatalogueItem;

            order.OrderItems.Add(orderItem);

            order.OrderingParty = organisation;
            order.LastUpdatedByUser = user;
            order.Supplier = supplier;
            order.SelectedFramework = framework;

            orderAdminService.Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            var result = (await controller.ViewOrder(order.CallOffId)).As<ViewResult>();

            var expected = new ViewOrderModel(order);

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Download_CompleteOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderAdminService> orderServiceMock,
            [Frozen] Mock<IOrderPdfService> pdfServiceMock,
            byte[] fileContents,
            ManageOrdersController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock
                .Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            pdfServiceMock
                .Setup(s => s.CreateOrderSummaryPdf(It.IsAny<EntityFramework.Ordering.Models.Order>()))
                .ReturnsAsync(new MemoryStream(fileContents));

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
            [Frozen] Mock<IOrderPdfService> pdfServiceMock,
            byte[] fileContents,
            ManageOrdersController controller)
        {
            order.Completed = null;

            orderServiceMock
                .Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            pdfServiceMock
                .Setup(s => s.CreateOrderSummaryPdf(It.IsAny<EntityFramework.Ordering.Models.Order>()))
                .ReturnsAsync(new MemoryStream(fileContents));

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
        [CommonInlineAutoData(1, 1, "")]
        [CommonInlineAutoData(1, 2, "Amendment_")]
        public static async Task Get_DownloadFullOrderCsv_ReturnsExpectedResult(
            int orderNumber,
            int revision,
            string prefix,
            string externalOrgId,
            [Frozen] Mock<ICsvService> csvService,
            ManageOrdersController controller)
        {
            var callOffId = new CallOffId(orderNumber, revision);
            var result = (await controller.DownloadFullOrderCsv(callOffId, externalOrgId)).As<FileContentResult>();

            csvService.VerifyAll();

            result.Should().NotBeNull();
            result.ContentType.Should().Be("application/octet-stream");
            result.FileDownloadName.Should().Be($"{prefix}{callOffId}_{externalOrgId}_full.csv");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteNotLatest_ReturnsViewWithModel(
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            ManageOrdersController controller)
        {
            var expectedModel = new DeleteNotLatestModel(order.CallOffId);

            orderAdminService.Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            var result = (await controller.DeleteNotLatest(order.CallOffId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteOrder_ReturnsViewWithModel_When_Latest(
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            [Frozen] Mock<IOrderService> orderService,
            ManageOrdersController controller)
        {
            var expectedModel = new DeleteOrderModel(order.CallOffId);

            orderAdminService.Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            orderService.Setup(s => s.HasSubsequentRevisions(order.CallOffId))
                .ReturnsAsync(false);

            var result = (await controller.DeleteOrder(order.CallOffId)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.OrderCreationDate));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteOrder_Redirects_To_DeleteNotLatest_When_Not_Latest(
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            [Frozen] Mock<IOrderService> orderService,
            ManageOrdersController controller)
        {
            var expectedModel = new DeleteOrderModel(order.CallOffId);

            orderAdminService.Setup(s => s.GetOrder(order.CallOffId))
                .ReturnsAsync(order);

            orderService.Setup(s => s.HasSubsequentRevisions(order.CallOffId))
                .ReturnsAsync(true);

            var result = (await controller.DeleteOrder(order.CallOffId)).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(ManageOrdersController.DeleteNotLatest));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteOrder_InvalidModelState(
            DeleteOrderModel model,
            ManageOrdersController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = (await controller.DeleteOrder(model)).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteOrder_ConfirmedDelete(
            DeleteOrderModel model,
            [Frozen] Mock<IOrderAdminService> orderAdminService,
            ManageOrdersController controller)
        {
            var result = (await controller.DeleteOrder(model)).As<RedirectToActionResult>();

            orderAdminService.Verify(s => s.DeleteOrder(model.CallOffId, model.NameOfRequester, model.NameOfApprover, model.ApprovalDate ?? null), Times.Once());

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
