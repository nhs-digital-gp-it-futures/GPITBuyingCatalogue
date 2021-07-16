using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderingParty;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class OrderingPartyControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrderingPartyController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrderingPartyController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderingPartyController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OrderingParty_ReturnsExpectedResult(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            Organisation organisation,
            [Frozen] Mock<IOrderService> orderServiceMock,
            [Frozen] Mock<IOrganisationsService> organisationServiceMock,
            OrderingPartyController controller)
        {
            var expectedViewData = new OrderingPartyModel(odsCode, order, organisation);

            orderServiceMock.Setup(s => s.GetOrder(order.CallOffId)).ReturnsAsync(order);
            organisationServiceMock.Setup(s => s.GetOrganisationByOdsCode(odsCode)).ReturnsAsync(organisation);

            var actualResult = await controller.OrderingParty(odsCode, order.CallOffId);

            actualResult.Should().BeOfType<ViewResult>();
            actualResult.As<ViewResult>().ViewData.Model.Should().BeEquivalentTo(expectedViewData);
        }
    }
}
