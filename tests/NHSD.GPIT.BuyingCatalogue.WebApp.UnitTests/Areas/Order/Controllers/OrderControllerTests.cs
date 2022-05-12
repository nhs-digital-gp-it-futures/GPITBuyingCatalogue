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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderControllerTests
    {
        private const int UserId = 1;

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_InProgressOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            AspNetUser aspNetUser,
            OrderTaskList orderTaskList,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<ITaskListService> taskListServiceMock,
            OrderController controller)
        {
            order.LastUpdatedByUser = aspNetUser;
            order.OrderStatus = OrderStatus.InProgress;

            var expectedViewData = new OrderModel(internalOrgId, order, orderTaskList) { DescriptionUrl = "testUrl" };

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            taskListServiceMock.Setup(s => s.GetTaskListStatusModelForOrder(order.Id)).ReturnsAsync(orderTaskList);

            var actualResult = await controller.Order(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CompleteOrder_RedirectsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderController controller)
        {
            order.OrderStatus = OrderStatus.Completed;

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var actualResult = await controller.Order(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Summary));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NewOrder_ReturnsExpectedResult(
            string internalOrgId,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            Organisation organisation,
            OrderController controller)
        {
            organisationsService.Setup(s => s.GetOrganisationByInternalIdentifier(internalOrgId))
                .ReturnsAsync(organisation);

            var expectedViewData = new OrderModel(internalOrgId, null, new OrderTaskList(), organisation.Name) { DescriptionUrl = "testUrl" };

            var actualResult = await controller.NewOrder(internalOrgId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Summary_CannotComplete_ReturnsErrorResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderController controller)
        {
            order.Description = null;

            orderServiceMock
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(order);

            var actualResult = await controller.Summary(internalOrgId, order.CallOffId, new SummaryModel());

            orderServiceMock.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);

            actualResult.As<ViewResult>()
                .ViewData.ModelState.Keys.Single()
                .Should()
                .Be("Order");

            actualResult.As<ViewResult>()
                .ViewData.ModelState.Values.Single()
                .Errors.Single()
                .ErrorMessage.Should()
                .Be("Your order is incomplete. Please go back to the order and check again");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ReadyToStart_ReturnsView(
            Organisation organisation,
            [Frozen] Mock<IOrganisationsService> service,
            OrderController controller)
        {
            service.Setup(s => s.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
                .ReturnsAsync(organisation);

            var result = await controller.ReadyToStart(organisation.InternalIdentifier);

            result.As<ViewResult>().Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Completed_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderController systemUnderTest)
        {
            var result = systemUnderTest.Completed(internalOrgId, callOffId);

            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.Should().BeAssignableTo<CompletedModel>().Subject;

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_ReadyToStart_Redirects(
            string internalOrgId,
            ReadyToStartModel model,
            TriageOption option,
            CatalogueItemType orderType,
            OrderController controller)
        {
            var result = controller.ReadyToStart(internalOrgId, model, option, orderType).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.NewOrder));
            result.RouteValues.Should().BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(option), option },
                    { nameof(orderType), orderType },
                });
        }

        [Theory]
        [CommonAutoData]
        public static void Post_ReadyToStart_WithFundingSource_Redirects(
            string internalOrgId,
            ReadyToStartModel model,
            TriageOption option,
            CatalogueItemType orderType,
            OrderController controller)
        {
            var result = controller.ReadyToStart(internalOrgId, model, option, orderType).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(controller.NewOrder));
            result.RouteValues.Should().BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(option), option },
                    { nameof(orderType), orderType },
                });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Download_CompleteOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IPdfService> pdfServiceMock,
            byte[] result,
            OrderController controller)
        {
            order.OrderStatus = OrderStatus.Completed;

            orderServiceMock
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(order);

            pdfServiceMock
                .Setup(s => s.Convert(It.IsAny<System.Uri>()))
                .Returns(result);

            SetControllerHttpContext(controller);

            var actualResult = await controller.Download(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            pdfServiceMock.VerifyAll();

            actualResult.Should().BeOfType<FileContentResult>();
            actualResult.As<FileContentResult>().ContentType.Should().Be("application/pdf");
            actualResult.As<FileContentResult>().FileDownloadName.Should().Be($"order-summary-completed-{order.CallOffId}.pdf");
            actualResult.As<FileContentResult>().FileContents.Should().BeEquivalentTo(result);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Download_InProgressOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IPdfService> pdfServiceMock,
            byte[] result,
            OrderController controller)
        {
            order.OrderStatus = OrderStatus.InProgress;

            orderServiceMock
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(order);

            pdfServiceMock
                .Setup(s => s.Convert(It.IsAny<System.Uri>()))
                .Returns(result);

            SetControllerHttpContext(controller);

            var actualResult = await controller.Download(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            pdfServiceMock.VerifyAll();

            actualResult.Should().BeOfType<FileContentResult>();
            actualResult.As<FileContentResult>().ContentType.Should().Be("application/pdf");
            actualResult.As<FileContentResult>().FileDownloadName.Should().Be($"order-summary-in-progress-{order.CallOffId}.pdf");
            actualResult.As<FileContentResult>().FileContents.Should().BeEquivalentTo(result);
        }

        private static void SetControllerHttpContext(ControllerBase controller)
        {
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
