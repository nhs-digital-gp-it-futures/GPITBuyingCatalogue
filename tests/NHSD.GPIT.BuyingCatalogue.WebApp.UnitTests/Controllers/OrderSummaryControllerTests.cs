using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
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
        [MockAutoData]
        public static async Task Get_Index_CompleteOrder_ReturnsExpectedResult(
            string internalOrgId,
            Order order,
            ImplementationPlan defaultPlan,
            [Frozen] IImplementationPlanService implementationPlanService,
            [Frozen] IOrderService orderServiceMock,
            OrderSummaryController controller)
        {
            order.Completed = DateTime.UtcNow;

            orderServiceMock.GetOrderForSummary(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            implementationPlanService.GetDefaultImplementationPlan().Returns(defaultPlan);

            var expectedViewData = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            var actualResult = await controller.Index(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_InProgress_InCompletable_Order_ReturnsExpectedResult(
            string internalOrgId,
            Order order,
            ImplementationPlan defaultPlan,
            [Frozen] IImplementationPlanService implementationPlanService,
            [Frozen] IOrderService orderServiceMock,
            OrderSummaryController controller)
        {
            order.Completed = null;

            orderServiceMock.GetOrderForSummary(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            implementationPlanService.GetDefaultImplementationPlan().Returns(defaultPlan);

            var expectedViewData = new OrderSummaryModel(new OrderWrapper(order), defaultPlan);

            var actualResult = await controller.Index(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }
    }
}
