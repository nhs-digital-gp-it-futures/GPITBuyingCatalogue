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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CommencementDate;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class CommencementDateControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CommencementDateController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(CommencementDateController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CommencementDateController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CommencementDate_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            CommencementDateController controller)
        {
            var expectedViewData = new CommencementDateModel(internalOrgId, order.CallOffId, order.CommencementDate, order.InitialPeriod, order.MaximumTerm);

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId, internalOrgId)).ReturnsAsync(order);

            var actualResult = await controller.CommencementDate(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_CommencementDate_SetsDate_CorrectlyRedirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<ICommencementDateService> commencementDateServiceMock,
            CommencementDateController controller)
        {
            var date = DateTime.UtcNow.AddDays(1).Date;
            const int initialPeriod = 3;
            const int maximumTerm = 12;

            var model = new CommencementDateModel
            {
                Day = date.Day.ToString(),
                Month = date.Month.ToString(),
                Year = date.Year.ToString(),
                InitialPeriod = $"{initialPeriod}",
                MaximumTerm = $"{maximumTerm}",
            };

            commencementDateServiceMock
                .Setup(x => x.SetCommencementDate(order.CallOffId, internalOrgId, date, initialPeriod, maximumTerm))
                .Returns(Task.CompletedTask);

            var result = await controller.CommencementDate(internalOrgId, order.CallOffId, model);

            commencementDateServiceMock.VerifyAll();

            var actualResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
            actualResult.ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.RouteValues.Should().BeEquivalentTo(new RouteValueDictionary
            {
                { "internalOrgId", internalOrgId },
                { "callOffId", order.CallOffId },
            });
        }
    }
}
