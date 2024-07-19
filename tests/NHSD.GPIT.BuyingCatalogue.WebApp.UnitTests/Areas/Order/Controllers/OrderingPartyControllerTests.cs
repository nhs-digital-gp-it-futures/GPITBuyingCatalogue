using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderingParty;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderingPartyControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderingPartyController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrderingPartyController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Orders");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderingPartyController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_OrderingParty_ReturnsExpectedResult(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            OrderingPartyController controller)
        {
            var expectedViewData = new OrderingPartyModel(internalOrgId, order);

            orderServiceMock.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            var actualResult = await controller.OrderingParty(internalOrgId, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_OrderingParty_SetsParty_CorrectlyRedirects(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order,
            [Frozen] IOrderService orderServiceMock,
            [Frozen] IOrderingPartyService orderingPartyServiceMock,
            OrderingPartyController controller,
            OrderingPartyModel model,
            Contact contact)
        {
            order.OrderingPartyContact = contact;

            orderServiceMock.GetOrderThin(order.CallOffId, internalOrgId).Returns(new OrderWrapper(order));

            var actualResult = await controller.OrderingParty(internalOrgId, order.CallOffId, model);

            actualResult.Should().BeOfType<RedirectToActionResult>();
            actualResult.As<RedirectToActionResult>().ActionName.Should().Be(nameof(OrderController.Order));
            actualResult.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(OrderController).ControllerName());
            actualResult.As<RedirectToActionResult>().RouteValues.Should().BeEquivalentTo(new RouteValueDictionary { { "internalOrgId", internalOrgId }, { "callOffId", order.CallOffId } });
            await orderingPartyServiceMock
                .Received()
                .SetOrderingPartyContact(order.CallOffId, contact);
        }
    }
}
