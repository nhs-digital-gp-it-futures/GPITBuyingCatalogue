using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class FundingSourceControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(FundingSourceController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(FundingSourceController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(FundingSourceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_FundingSource_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] Mock<IOrderService> orderServiceMock,
            FundingSourceController controller)
        {
            var expectedViewData = new FundingSourceModel(odsCode, order.CallOffId, order.FundingSourceOnlyGms);

            orderServiceMock.Setup(s => s.GetOrderThin(order.CallOffId)).ReturnsAsync(order);

            var actualResult = await controller.FundingSource(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FundingSource_Deletes_CorrectlyRedirects(
            string odsCode,
            CallOffId callOffId,
            FundingSourceModel model,
            [Frozen] Mock<IFundingSourceService> fundingSourceServiceMock,
            FundingSourceController controller)
        {
            var actualResult = await controller.FundingSource(odsCode, callOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "odsCode", odsCode }, { "callOffId", callOffId } });
            fundingSourceServiceMock.Verify(o => o.SetFundingSource(callOffId, model.FundingSourceOnlyGms.EqualsIgnoreCase("Yes")), Times.Once);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_FundingSource_InvalidModelState_ReturnsView(
            string errorKey,
            string errorMessage,
            string odsCode,
            CallOffId callOffId,
            FundingSourceModel model,
            FundingSourceController controller)
        {
            controller.ModelState.AddModelError(errorKey, errorMessage);

            var actualResult = (await controller.FundingSource(odsCode, callOffId, model)).As<ViewResult>();

            actualResult.Should().NotBeNull();
            actualResult.ViewName.Should().BeNull();
            actualResult.ViewData.ModelState.IsValid.Should().BeFalse();
            actualResult.ViewData.ModelState.ErrorCount.Should().Be(1);
            actualResult.ViewData.ModelState.Keys.Single().Should().Be(errorKey);
            actualResult.ViewData.ModelState.Values.Single().Errors.Single().ErrorMessage.Should().Be(errorMessage);
        }
    }
}
