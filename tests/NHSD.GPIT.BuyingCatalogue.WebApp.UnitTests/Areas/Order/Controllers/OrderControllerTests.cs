using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Orders.Controllers
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
            OrderProgress orderTaskList,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IOrderProgressService> orderProgressService,
            OrderController controller)
        {
            order.LastUpdatedByUser = aspNetUser;
            order.Completed = null;

            orderServiceMock
                .Setup(s => s.GetOrderForTaskListStatuses(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            orderProgressService
                .Setup(x => x.GetOrderProgress(internalOrgId, order.CallOffId))
                .ReturnsAsync(orderTaskList);

            var result = await controller.Order(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            orderProgressService.VerifyAll();

            var expected = new OrderModel(internalOrgId, order, orderTaskList)
            {
                DescriptionUrl = "testUrl",
            };

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.ViewData.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CompleteOrder_RedirectsCorrectly(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock
                .Setup(s => s.GetOrderForTaskListStatuses(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var actualResult = await controller.Order(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Summary));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Summary_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IImplementationPlanService> implementationPlanService,
            [Frozen] Mock<IOrderService> orderService,
            OrderController controller)
        {
            implementationPlanService
                .Setup(x => x.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            orderService
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.Summary(internalOrgId, order.CallOffId);

            var actualResult = result.Should().BeOfType<ViewResult>().Subject;
            var expected = new SummaryModel(new OrderWrapper(order), internalOrgId, defaultPlan);

            actualResult.Model.Should().BeEquivalentTo(
                expected,
                x => x.Excluding(m => m.BackLink)
                      .Excluding(m => m.Title)
                      .Excluding(m => m.AdviceText)
                      .Excluding(m => m.CanBeAmended));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Summary_CannotComplete_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            ImplementationPlan defaultPlan,
            [Frozen] Mock<IImplementationPlanService> implementationPlanService,
            [Frozen] Mock<IOrderService> orderService,
            OrderController controller)
        {
            order.Description = null;

            implementationPlanService
                .Setup(x => x.GetDefaultImplementationPlan())
                .ReturnsAsync(defaultPlan);

            orderService
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.SummaryComplete(
                internalOrgId,
                order.CallOffId);

            var modelState = result.Should().BeOfType<ViewResult>().Subject.ViewData.ModelState;

            modelState.ValidationState.Should().Be(ModelValidationState.Invalid);
            modelState.Keys.First().Should().Be(OrderController.ErrorKey);
            modelState.Values.First().Errors.First().ErrorMessage.Should().Be(OrderController.ErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Summary_CanComplete_ExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            OrderController controller)
        {
            order.Contract = new Contract() { ContractBilling = new ContractBilling(), ImplementationPlan = new ImplementationPlan(), };
            order.ContractFlags.UseDefaultDataProcessing = true;
            order.Completed = null;
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            orderService
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            orderService
                .Setup(x => x.CompleteOrder(order.CallOffId, internalOrgId, 1))
                .Verifiable();

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
        [CommonAutoData]
        public static async Task Get_NewOrder_ReturnsExpectedResult(
            string internalOrgId,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            Organisation organisation,
            OrderController controller)
        {
            organisationsService
                .Setup(s => s.GetOrganisationByInternalIdentifier(internalOrgId))
                .ReturnsAsync(organisation);

            var result = await controller.NewOrder(internalOrgId);

            organisationsService.VerifyAll();

            var expected = new OrderModel(internalOrgId, null, new OrderProgress(), organisation.Name)
            {
                DescriptionUrl = "testUrl",
            };

            var actual = result.Should().BeOfType<ViewResult>().Subject;

            actual.ViewData.Model.Should().BeEquivalentTo(expected, opt => opt.Excluding(m => m.BackLink));
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
        public static async Task Get_Completed_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderService,
            OrderController systemUnderTest)
        {
            orderService
                .Setup(x => x.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await systemUnderTest.Completed(internalOrgId, order.CallOffId);

            var actual = result.Should().BeOfType<ViewResult>().Subject;
            var model = actual.Model.Should().BeAssignableTo<CompletedModel>().Subject;

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(order.CallOffId);
            model.Order.Should().Be(order);
        }

        [Theory]
        [CommonAutoData]
        public static void Post_ReadyToStart_Redirects(
            string internalOrgId,
            ReadyToStartModel model,
            OrderTriageValue option,
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
            OrderTriageValue option,
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
            [Frozen] Mock<IOrderPdfService> pdfServiceMock,
            byte[] result,
            OrderController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            pdfServiceMock
                .Setup(s => s.CreateOrderSummaryPdf(It.IsAny<EntityFramework.Ordering.Models.Order>()))
                .ReturnsAsync(new MemoryStream(result));

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
            [Frozen] Mock<IOrderPdfService> pdfServiceMock,
            byte[] result,
            OrderController controller)
        {
            order.Completed = null;

            orderServiceMock
                .Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            pdfServiceMock
                .Setup(s => s.CreateOrderSummaryPdf(It.IsAny<EntityFramework.Ordering.Models.Order>()))
                .ReturnsAsync(new MemoryStream(result));

            SetControllerHttpContext(controller);

            var actualResult = await controller.Download(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            pdfServiceMock.VerifyAll();

            actualResult.Should().BeOfType<FileContentResult>();
            actualResult.As<FileContentResult>().ContentType.Should().Be("application/pdf");
            actualResult.As<FileContentResult>().FileDownloadName.Should().Be($"order-summary-in-progress-{order.CallOffId}.pdf");
            actualResult.As<FileContentResult>().FileContents.Should().BeEquivalentTo(result);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_AmendOrder_ReturnsExpectedResult(
            string internalOrgId,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            AmendOrderModel model,
            [Frozen] Mock<IOrderService> orderService,
            OrderController controller)
        {
            orderService
                .Setup(x => x.AmendOrder(internalOrgId, callOffId))
                .ReturnsAsync(order);

            var result = await controller.AmendOrder(internalOrgId, callOffId, model);

            orderService.VerifyAll();

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
        [CommonAutoData]
        public static void GetAdvice_CompletedAssociatedServicesOnlyOrder_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.AssociatedServicesOnly = true;
            order.Completed = DateTime.UtcNow;

            OrderController.GetAdvice(order, true)
                .Should()
                .Be("This order has been confirmed and can no longer be changed.");
        }

        [Theory]
        [CommonAutoData]
        public static void GetAdvice_CompletedOrderIsLatest_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.AssociatedServicesOnly = false;
            order.Completed = DateTime.UtcNow;

            OrderController.GetAdvice(order, true)
                .Should()
                .Be("This order has already been completed, but you can amend it if needed.");
        }

        [Theory]
        [CommonAutoData]
        public static void GetAdvice_CompletedOrderIsNotLatest_ReturnsExpectedAdvice(
            EntityFramework.Ordering.Models.Order order)
        {
            order.AssociatedServicesOnly = false;
            order.Completed = DateTime.UtcNow;

            OrderController.GetAdvice(order, false)
                .Should()
                .Be("This order can no longer be changed as there is already an amendment in progress.");
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
