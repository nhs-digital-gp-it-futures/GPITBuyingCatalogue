using System;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
            string odsCode,
            CallOffId callOffId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            CommencementDateController controller)
        {
            var expectedViewData = new CommencementDateModel(odsCode, callOffId, order.CommencementDate);

            orderServiceMock.Setup(s => s.GetOrder(callOffId)).ReturnsAsync(order);

            var actualResult = await controller.CommencementDate(odsCode, callOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_CommencementDate_InvalidDate_ReturnsErrorResult(
            string odsCode,
            CallOffId callOffId,
            CommencementDateController controller)
        {
            var model = new CommencementDateModel { Day = "ABC", };

            var actualResult = await controller.CommencementDate(odsCode, callOffId, model);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.ModelState.ValidationState.Should().Be(ModelValidationState.Invalid);

            actualResult.As<ViewResult>()
                .ViewData.ModelState.Keys.Single()
                .Should()
                .Be("Day");

            actualResult.As<ViewResult>()
                .ViewData.ModelState.Values.Single()
                .Errors.Single()
                .ErrorMessage.Should()
                .Be("Commencement date must be a real date");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_CommencementDate_SetsDate_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            CreateOrderItemModel state,
            [Frozen] Mock<IOrderSessionService> orderSessionServiceMock,
            [Frozen] Mock<ICommencementDateService> commencementDateServiceMock,
            CommencementDateController controller)
        {
            var model = new CommencementDateModel
            {
                Day = DateTime.UtcNow.AddDays(1).Day.ToString(),
                Month = DateTime.UtcNow.AddDays(1).Month.ToString(),
                Year = DateTime.UtcNow.AddDays(1).Year.ToString(),
            };

            orderSessionServiceMock.Setup(s => s.GetOrderStateFromSession(callOffId)).Returns(state);

            var actualResult = await controller.CommencementDate(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", callOffId } });

            commencementDateServiceMock.Verify(c => c.SetCommencementDate(callOffId, DateTime.UtcNow.AddDays(1).Date), Times.Once);
        }
    }
}
