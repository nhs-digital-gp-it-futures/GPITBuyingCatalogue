using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class Covid19ControllerTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new Covid19Controller(null));
        }

        [Test]
        public static void Get_Index_ReturnsViewResult()
        {
            var controller = new Covid19Controller(Mock.Of<ILogger<Covid19Controller>>());

            var result = controller.Index();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
        }
    }
}
