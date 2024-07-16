using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderControllerTests
    {
        private const int UserId = 1;

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrderController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_InProgressOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            AspNetUser aspNetUser,
            OrderProgress orderTaskList,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] IOrderProgressService orderProgressService,
            OrderController controller)
        {
            order.LastUpdatedByUser = aspNetUser;
            order.Completed = null;

            orderServiceMock.GetOrderForTaskListStatuses(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            orderProgressService.GetOrderProgress(internalOrgId, order.CallOffId).Returns(Task.FromResult(orderTaskList));

            var result = await controller.Order(internalOrgId, order.CallOffId);

            await orderServiceMock.Received().GetOrderForTaskListStatuses(order.CallOffId, internalOrgId);
            await orderProgressService.Received().GetOrderProgress(internalOrgId, order.CallOffId);

            var expected = new OrderModel(internalOrgId, orderTaskList, order)
            {
                DescriptionUrl = "testUrl",
            };

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.ViewData.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_CompleteOrder_RedirectsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            OrderController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock.GetOrderForTaskListStatuses(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            var actualResult = await controller.Order(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Summary));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NullOrder_RedirectsToOrderDashboard(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            OrderController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            orderWrapper.Order = null;

            orderServiceMock.GetOrderForTaskListStatuses(order.CallOffId, internalOrgId).Returns(Task.FromResult(orderWrapper));

            var result = await controller.Order(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actualResult.ActionName.Should()
                .Be(nameof(DashboardController.Organisation));
            actualResult.RouteValues.Should()
                .BeEquivalentTo(
                    new RouteValueDictionary { { "internalOrgId", internalOrgId }, });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Summary_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlan defaultPlan,
            bool hasSubsequentRevisions,
            [Frozen] IImplementationPlanService implementationPlanService,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            implementationPlanService.GetDefaultImplementationPlan().Returns(Task.FromResult(defaultPlan));

            orderService.GetOrderForSummary(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            orderService.HasSubsequentRevisions(order.CallOffId).Returns(Task.FromResult(hasSubsequentRevisions));

            var result = await controller.Summary(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new SummaryModel(new OrderWrapper(order), internalOrgId, hasSubsequentRevisions, defaultPlan);

            actualResult.Model.Should().BeEquivalentTo(
                expected,
                x => x.Excluding(m => m.BackLink)
                      .Excluding(m => m.Title)
                      .Excluding(m => m.AdviceText)
                      .Excluding(m => m.CanBeAmended));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Summary_CannotComplete_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlan defaultPlan,
            [Frozen] IImplementationPlanService implementationPlanService,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            order.Description = null;

            implementationPlanService.GetDefaultImplementationPlan().Returns(Task.FromResult(defaultPlan));

            orderService.GetOrderForSummary(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            var result = (await controller.SummaryComplete(
                internalOrgId,
                order.CallOffId)).As<RedirectToActionResult>();

            result.ActionName.Should().Be(nameof(controller.Summary));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_Summary_CanComplete_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            order.Contract = new Contract() { ContractBilling = new ContractBilling(), ImplementationPlan = new ImplementationPlan(), };
            order.ContractFlags.UseDefaultDataProcessing = true;
            order.Completed = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService.GetOrderForSummary(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            var result = await controller.SummaryComplete(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.ActionName.Should().Be(nameof(OrderController.Completed));
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_NewOrder_ReturnsExpectedResult(
            string internalOrgId,
            OrderTypeEnum orderType,
            string frameworkId,
            [Frozen] IOrganisationsService organisationsService,
            Organisation organisation,
            OrderController controller)
        {
            organisationsService.GetOrganisationByInternalIdentifier(internalOrgId).Returns(Task.FromResult(organisation));

            var result = await controller.NewOrder(internalOrgId, orderType, frameworkId);

            await organisationsService.Received().GetOrganisationByInternalIdentifier(internalOrgId);

            var expected = new OrderModel(internalOrgId, orderType, new OrderProgress(), organisation.Name)
            {
                DescriptionUrl = "testUrl",
            };

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.ViewData.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_ReadyToStart_ReturnsView(
            Organisation organisation,
            [Frozen] IOrganisationsService service,
            OrderController controller)
        {
            service.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier).Returns(Task.FromResult(organisation));

            var result = await controller.ReadyToStart(organisation.InternalIdentifier);

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            actual.Model.Should().BeAssignableTo<ReadyToStartModel>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Completed_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            OrderController systemUnderTest)
        {
            orderService.GetOrderForSummary(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            var result = await systemUnderTest.Completed(internalOrgId, order.CallOffId);

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeAssignableTo<CompletedModel>().Subject;

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(order.CallOffId);
            model.Order.Should().Be(order);
        }

        [Theory]
        [MockAutoData]
        public static void Post_ReadyToStart_Redirects(
            string internalOrgId,
            ReadyToStartModel model,
            OrderController controller)
        {
            var result = controller.ReadyToStart(internalOrgId, model).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderTriageController.SelectOrganisation));
            result.RouteValues.Should().BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Download_CompleteOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] IOrderPdfService pdfServiceMock,
            byte[] result,
            OrderController controller)
        {
            order.Completed = DateTime.UtcNow;

            await DownloadReturnsExpectedResult(internalOrgId, order, orderServiceMock, pdfServiceMock, result, controller, "order-summary-completed");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Download_InProgressOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] IOrderPdfService pdfServiceMock,
            byte[] result,
            OrderController controller)
        {
            order.Completed = null;

            await DownloadReturnsExpectedResult(internalOrgId, order, orderServiceMock, pdfServiceMock, result, controller, "order-summary-in-progress");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Download_TerminatedOrder_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] IOrderPdfService pdfServiceMock,
            byte[] result,
            OrderController controller)
        {
            order.IsTerminated = true;

            await DownloadReturnsExpectedResult(internalOrgId, order, orderServiceMock, pdfServiceMock, result, controller, "order-summary-terminated");
        }

        [Theory]
        [MockAutoData]
        public static void Get_AmendOrder_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderController controller)
        {
            var result = controller.AmendOrder(internalOrgId, callOffId);

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new AmendOrderModel(internalOrgId, callOffId);

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AmendOrder_HasSubsequentRevisions_Redirects(
            string internalOrgId,
            CallOffId callOffId,
            AmendOrderModel model,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            orderService.GetOrderThin(callOffId, internalOrgId).Returns(new OrderWrapper());
            orderService.HasSubsequentRevisions(callOffId).Returns(Task.FromResult(true));

            var result = (await controller.AmendOrder(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

            await orderService.Received().HasSubsequentRevisions(callOffId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(DashboardController.Organisation));
            result.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AmendOrder_ContractExpired_Redirects(
            string internalOrgId,
            CallOffId callOffId,
            AmendOrderModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            order.CommencementDate = DateTime.Now.AddMonths(-6);
            order.MaximumTerm = 1;

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            orderService.HasSubsequentRevisions(callOffId).Returns(Task.FromResult(false));

            var result = (await controller.AmendOrder(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

            await orderService.Received().GetOrderThin(callOffId, internalOrgId);
            await orderService.Received().HasSubsequentRevisions(callOffId);

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(DashboardController.Organisation));
            result.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AmendOrder_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            AmendOrderModel model,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            order.CommencementDate = DateTime.Now;
            order.MaximumTerm = 6;

            orderService.GetOrderThin(callOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            orderService.AmendOrder(internalOrgId, callOffId).Returns(Task.FromResult(order));

            var result = await controller.AmendOrder(internalOrgId, callOffId, model);

            await orderService.Received().GetOrderThin(callOffId, internalOrgId);
            await orderService.Received().AmendOrder(internalOrgId, callOffId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(OrderController.Order));
            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static void Get_TerminateOrder_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            OrderController controller)
        {
            var result = controller.TerminateOrder(internalOrgId, callOffId);

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            var expected = new TerminateOrderModel(internalOrgId, callOffId);

            actual.Model.Should().BeEquivalentTo(expected, x => x.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TerminateOrder_HasSubsequentRevisions_Redirects(
            string internalOrgId,
            CallOffId callOffId,
            TerminateOrderModel model,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            orderService.HasSubsequentRevisions(callOffId).Returns(Task.FromResult(true));
            orderService.TerminateOrder(callOffId, internalOrgId, Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<string>()).Returns(Task.CompletedTask);

            var result = (await controller.TerminateOrder(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

            await orderService.Received().HasSubsequentRevisions(callOffId);
            await orderService.Received(0).TerminateOrder(callOffId, internalOrgId, Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<string>());

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(DashboardController.Organisation));
            result.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_TerminateOrder_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            TerminateOrderModel model,
            [Frozen] IOrderService orderService,
            OrderController controller)
        {
            orderService.HasSubsequentRevisions(callOffId).Returns(Task.FromResult(false));

            var result = (await controller.TerminateOrder(internalOrgId, callOffId, model)).As<RedirectToActionResult>();

            await orderService.Received(1).HasSubsequentRevisions(callOffId);
            await orderService.Received(1).TerminateOrder(callOffId, internalOrgId, Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<string>());

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(OrderController.Summary));
            result.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            result.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static void GetAdvice_TerminatedOrder_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.IsTerminated = true;

            OrderController.GetAdvice(new OrderWrapper(order), true)
                .Should()
                .Be("This contract has been terminated, but you can still view the details.");
        }

        [Theory]
        [MockAutoData]
        public static void GetAdvice_ExpiredOrder_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.Completed = DateTime.UtcNow.AddMonths(-5);
            order.CommencementDate = DateTime.Now.AddMonths(-6);
            order.MaximumTerm = 2;

            OrderController.GetAdvice(new OrderWrapper(order), true)
                .Should()
                .Be($"This order expired on {order.EndDate.DisplayValue}, but you can still view the details.");
        }

        [Theory]
        [MockAutoData]
        public static void GetAdvice_CompletedAssociatedServicesOnlyOrder_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.AssociatedServiceOther;
            order.Completed = DateTime.UtcNow;

            OrderController.GetAdvice(new OrderWrapper(order), true)
                .Should()
                .Be("This order has already been completed, but you can terminate the contract if needed.");
        }

        [Theory]
        [MockAutoData]
        public static void GetAdvice_CompletedOrderIsLatest_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.Completed = DateTime.UtcNow;

            OrderController.GetAdvice(new OrderWrapper(order), true)
                .Should()
                .Be("This order has already been completed, but you can amend or terminate the contract if needed.");
        }

        [Theory]
        [MockAutoData]
        public static void GetAdvice_CompletedOrderIsNotLatest_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderType = OrderTypeEnum.Solution;
            order.Completed = DateTime.UtcNow;

            OrderController.GetAdvice(new OrderWrapper(order), false)
                .Should()
                .Be("There is an amendment currently in progress for this contract.");
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

        private static async Task DownloadReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] IOrderPdfService pdfServiceMock,
            byte[] result,
            OrderController controller,
            string fileName)
        {
            orderServiceMock.GetOrderForSummary(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            pdfServiceMock.CreateOrderSummaryPdf(Arg.Any<EntityFramework.Ordering.Models.Order>()).Returns(Task.FromResult(new MemoryStream(result)));

            SetControllerHttpContext(controller);

            var actualResult = await controller.Download(internalOrgId, order.CallOffId);

            await orderServiceMock.Received().GetOrderForSummary(order.CallOffId, internalOrgId);
            await pdfServiceMock.Received().CreateOrderSummaryPdf(Arg.Any<EntityFramework.Ordering.Models.Order>());

            actualResult.Should().BeOfType<FileContentResult>();
            actualResult.As<FileContentResult>().ContentType.Should().Be("application/pdf");
            actualResult.As<FileContentResult>().FileDownloadName.Should().Be($"{fileName}-{order.CallOffId}.pdf");
            actualResult.As<FileContentResult>().FileContents.Should().BeEquivalentTo(result);
        }
    }
}
