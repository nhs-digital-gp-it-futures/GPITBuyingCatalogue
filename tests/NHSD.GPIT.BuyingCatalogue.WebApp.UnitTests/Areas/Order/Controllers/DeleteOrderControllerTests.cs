using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class DeleteOrderControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DeleteOrderController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DeleteOrderController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DeleteOrderController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteOrder_InProgress_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            DeleteOrderController controller)
        {
            order.Completed = null;

            orderServiceMock.GetOrderThin(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            var actualResult = await controller.DeleteOrder(internalOrgId, order.CallOffId);

            await orderServiceMock.Received().GetOrderThin(order.CallOffId, internalOrgId);

            actualResult.Should().BeOfType<ViewResult>();
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteOrder_Completed_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            DeleteOrderController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock.GetOrderThin(order.CallOffId, internalOrgId).Returns(Task.FromResult(new OrderWrapper(order)));

            var result = await controller.DeleteOrder(internalOrgId, order.CallOffId);

            await orderServiceMock.Received().GetOrderThin(order.CallOffId, internalOrgId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(OrderController.Summary));
            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_DeleteOrder_NullOrder_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            DeleteOrderController controller)
        {
            var orderWrapper = new OrderWrapper(order);
            orderWrapper.Order = null;

            orderServiceMock.GetOrderThin(order.CallOffId, internalOrgId).Returns(Task.FromResult(orderWrapper));

            var result = await controller.DeleteOrder(internalOrgId, order.CallOffId);

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
        public static async Task Post_DeleteOrder_Amendment_YesSelected_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] IOrderService orderServiceMock,
            DeleteOrderController controller)
        {
            model.IsAmendment = true;
            model.SelectedOption = true;

            orderServiceMock.HardDeleteOrder(callOffId, internalOrgId).Returns(Task.CompletedTask);

            var result = await controller.DeleteOrder(internalOrgId, callOffId, model);

            await orderServiceMock.Received().HardDeleteOrder(callOffId, internalOrgId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DashboardController.Organisation));
            actual.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteOrder_Order_YesSelected_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] IOrderService orderServiceMock,
            DeleteOrderController controller)
        {
            model.IsAmendment = false;
            model.SelectedOption = true;

            orderServiceMock.SoftDeleteOrder(callOffId, internalOrgId).Returns(Task.CompletedTask);

            var result = await controller.DeleteOrder(internalOrgId, callOffId, model);

            await orderServiceMock.Received().SoftDeleteOrder(callOffId, internalOrgId);

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DashboardController.Organisation));
            actual.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
            });
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_DeleteOrder_NoSelected_DoesNotDelete_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] IOrderService orderServiceMock,
            DeleteOrderController controller)
        {
            model.SelectedOption = false;

            var result = await controller.DeleteOrder(internalOrgId, callOffId, model);

            orderServiceMock.ReceivedCalls().Should().BeEmpty();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(OrderController.Order));
            actual.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", callOffId },
            });
        }
    }
}
