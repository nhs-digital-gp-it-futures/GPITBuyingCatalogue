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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderDescriptionControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderDescriptionController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrderDescriptionController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderDescriptionController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OrderDescription_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderDescriptionController controller)
        {
            var expectedViewData = new OrderDescriptionModel(odsCode, order) { BackLink = "testUrl" };

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.OrderDescription(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OrderDescription_SetsDescription_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            OrderDescriptionModel model,
            [Frozen] Mock<IOrderDescriptionService> orderDescriptionServiceMock,
            OrderDescriptionController controller)
        {
            var actualResult = await controller.OrderDescription(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", callOffId } });
            orderDescriptionServiceMock.Verify(o => o.SetOrderDescription(callOffId, model.Description), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_NewOrderDescription_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderDescriptionController controller)
        {
            var expectedViewData = new OrderDescriptionModel(odsCode, null) { BackLink = "testUrl" };

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);

            var actualResult = controller.NewOrderDescription(odsCode);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_NewOrderDescription_CreatesOrder_CorrectlyRedirects(
            string odsCode,
            OrderDescriptionModel model,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderDescriptionController controller)
        {
            orderServiceMock.Setup(s => s.CreateOrder(model.Description, model.OdsCode)).ReturnsAsync(order);

            var actualResult = await controller.NewOrderDescription(odsCode, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", order.CallOffId } });
            orderServiceMock.Verify(o => o.CreateOrder(model.Description, model.OdsCode), Times.Once);
        }
    }
}
