using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HomeControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(HomeController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HomeController(null));
        }

        [Test]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new HomeController(Mock.Of<ILogWrapper<HomeController>>());

            var result = controller.Index();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }
    }
}
