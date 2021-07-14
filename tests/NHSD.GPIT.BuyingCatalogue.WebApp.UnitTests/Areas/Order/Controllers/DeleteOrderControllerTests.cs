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
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class DeleteOrderControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DeleteOrderController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DeleteOrderController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
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
        public static async Task Get_DeleteOrder_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            var expectedViewData = new DeleteOrderModel(odsCode, order);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.DeleteOrder(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_DeleteOrder_Deletes_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            DeleteOrderModel model,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            var actualResult = await controller.DeleteOrder(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(DashboardController.Organisation));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(DashboardController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode } });
            orderServiceMock.Verify(o => o.DeleteOrder(callOffId), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_DeleteOrderConfirmation_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            DeleteOrderController controller)
        {
            var expectedViewData = new DeleteConfirmationModel(odsCode, order);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.DeleteOrderConfirmation(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }
    }
}
