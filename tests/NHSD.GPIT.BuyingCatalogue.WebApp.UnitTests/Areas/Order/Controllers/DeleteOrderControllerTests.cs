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
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;
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
        [CommonAutoData]
        public static async Task Get_DeleteOrder_InProgress_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            order.Completed = null;

            orderServiceMock
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var actualResult = await controller.DeleteOrder(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();

            actualResult.Should().BeOfType<ViewResult>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteOrder_Completed_Redirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock
                .Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId))
                .ReturnsAsync(new OrderWrapper(order));

            var result = await controller.DeleteOrder(internalOrgId, order.CallOffId);

            orderServiceMock.VerifyAll();
            orderServiceMock.VerifyNoOtherCalls();

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
        [CommonAutoData]
        public static async Task Post_DeleteOrder_Amendment_YesSelected_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            model.IsAmendment = true;
            model.SelectedOption = true;

            orderServiceMock
                .Setup(x => x.HardDeleteOrder(callOffId, internalOrgId))
                .Verifiable();

            var result = await controller.DeleteOrder(internalOrgId, callOffId, model);

            orderServiceMock.VerifyAll();
            orderServiceMock.VerifyNoOtherCalls();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DashboardController.Organisation));
            actual.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteOrder_Order_YesSelected_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            model.IsAmendment = false;
            model.SelectedOption = true;

            orderServiceMock
                .Setup(x => x.SoftDeleteOrder(callOffId, internalOrgId))
                .Verifiable();

            var result = await controller.DeleteOrder(internalOrgId, callOffId, model);

            orderServiceMock.VerifyAll();
            orderServiceMock.VerifyNoOtherCalls();

            var actual = result.Should().BeOfType<RedirectToActionResult>().Subject;

            actual.ActionName.Should().Be(nameof(DashboardController.Organisation));
            actual.ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actual.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
            });
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteOrder_NoSelected_DoesNotDelete_CorrectlyRedirects(
            string internalOrgId,
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            model.SelectedOption = false;

            var result = await controller.DeleteOrder(internalOrgId, callOffId, model);

            orderServiceMock.VerifyNoOtherCalls();

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
