using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class OrderSummaryControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderSummaryController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderSummaryController).Should().BeDecoratedWith<RestrictToLocalhostActionFilter>();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_CompleteOrder_ReturnsExpectedResult(
            string internalOrgId,
            Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderSummaryController controller)
        {
            order.OrderStatus = OrderStatus.Completed;

            orderServiceMock.Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var expectedViewData = new OrderSummaryModel(order)
            {
                AdviceText = "This order has been completed and can no longer be changed.",
                Title = $"Order completed for {order.CallOffId}",
            };

            var actualResult = await controller.Index(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_InProgress_InCompletable_Order_ReturnsExpectedResult(
            string internalOrgId,
            Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            OrderSummaryController controller)
        {
            order.OrderStatus = OrderStatus.InProgress;

            orderServiceMock.Setup(s => s.GetOrderForSummary(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var expectedViewData = new OrderSummaryModel(order)
            {
                AdviceText = "This is what's been added to your order so far. You must complete all mandatory steps before you can confirm your order.",
                Title = $"Order summary for {order.CallOffId}",
            };

            var actualResult = await controller.Index(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }
    }
}
