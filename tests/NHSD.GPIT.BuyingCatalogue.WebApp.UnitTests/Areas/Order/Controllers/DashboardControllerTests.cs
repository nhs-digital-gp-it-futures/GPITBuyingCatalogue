using System;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    public static class DashboardControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DashboardController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(DashboardController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Order");
        }

        [Fact]
        public static void Constructor_NullOrganisationService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new DashboardController( null, Mock.Of<IOrderService>()));
        }

        [Fact]
        public static void Constructor_NullOrderService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new DashboardController( Mock.Of<IOrganisationsService>(), null));
        }

        [Fact]
        public static void Get_Order_NotBuyer_ReturnsNotBuyerView()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new("organisationFunction", "Authority") },
                "mock"));

            var controller = new DashboardController( Mock.Of<IOrganisationsService>(), Mock.Of<IOrderService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user },
                }
            };

            var result = controller.Index();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Equal("NotBuyer", ((ViewResult)result).ViewName);
        }
    }
}
