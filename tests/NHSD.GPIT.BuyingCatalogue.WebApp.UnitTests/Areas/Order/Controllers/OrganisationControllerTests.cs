using System;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(OrganisationController).Should().BeDecoratedWith<AuthorizeAttribute>();
            typeof(OrganisationController).Should().BeDecoratedWith<AreaAttribute>(x=> x.RouteValue == "Order");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationController(null));
        }

        [Test]
        public static void Get_Index_NotBuyer_ReturnsNotBuyerView()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{    
                new Claim("organisationFunction", "Authority"),                
            }, "mock"));

            var controller = new OrganisationController(Mock.Of<ILogWrapper<OrganisationController>>());
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = controller.Index();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.AreEqual("NotBuyer", ((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_Index_Buyer_ReturnsDefaultView()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                new Claim("organisationFunction", "Buyer"),                
            }, "mock"));

            var controller = new OrganisationController(Mock.Of<ILogWrapper<OrganisationController>>());
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = controller.Index();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_NewOrder_ReturnsViewResult()
        {
            var controller = new OrganisationController(Mock.Of<ILogWrapper<OrganisationController>>());

            var result = controller.NewOrder();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
        }
    }
}
