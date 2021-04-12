using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HomeControllerTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HomeController(null));
        }

        [Test]
        public static void Get_Index_ReturnsViewResult()
        {
            var controller = new HomeController(Mock.Of<ILogger<HomeController>>());

            var result = controller.Index();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
        }
    }
}
