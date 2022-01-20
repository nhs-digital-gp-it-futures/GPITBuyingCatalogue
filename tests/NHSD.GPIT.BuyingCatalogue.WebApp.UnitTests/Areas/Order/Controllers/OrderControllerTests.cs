using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrderController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
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
        public static async Task Get_IncompleteOrder_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            OrderTaskList orderTaskList,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<ITaskListService> taskListServiceMock,
            OrderController controller)
        {
            order.OrderStatus = OrderStatus.Incomplete;

            var expectedViewData = new OrderModel(odsCode, order, orderTaskList) { DescriptionUrl = "testUrl" };

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId, odsCode)).ReturnsAsync(order);

            taskListServiceMock.Setup(s => s.GetTaskListStatusModelForOrder(order)).Returns(orderTaskList);

            var actualResult = await controller.Order(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.BackLinkText));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CompleteOrder_RedirectsCorrectly(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderController controller)
        {
            order.OrderStatus = OrderStatus.Complete;

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId, odsCode)).ReturnsAsync(order);

            var actualResult = await controller.Order(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Summary));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", order.CallOffId } });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_NewOrder_ReturnsExpectedResult(
            string odsCode,
            [Frozen] Mock<IOrganisationsService> organisationsService,
            Organisation organisation,
            OrderController controller)
        {
            organisationsService.Setup(s => s.GetOrganisationByOdsCode(odsCode))
                .ReturnsAsync(organisation);

            var expectedViewData = new OrderModel(odsCode, null, new OrderTaskList(), organisation.Name) { DescriptionUrl = "testUrl" };

            var actualResult = await controller.NewOrder(odsCode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Summary_CannotComplete_ReturnsErrorResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderController controller)
        {
            order.Description = null;

            orderServiceMock.Setup(s => s.GetOrderForSummary(order.CallOffId, odsCode)).ReturnsAsync(order);

            var actualResult = await controller.Summary(odsCode, order.CallOffId);

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
        public static void Get_ReadyToStart_ReturnsView(
            string odsCode,
            OrderController controller)
        {
            var result = controller.ReadyToStart(odsCode);

            result.As<ViewResult>().Should().NotBeNull();
        }
    }
}
