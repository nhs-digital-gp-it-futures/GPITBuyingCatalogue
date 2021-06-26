using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HomeControllerTests
    {
        [Test]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new HomeController();

            var result = controller.Index();

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_Error500_ReturnsDefaultErrorView()
        {
            var controller = new HomeController();

            var result = controller.Error(500);

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_ErrorNullStatus_ReturnsDefaultErrorView()
        {
            var controller = new HomeController();

            var result = controller.Error(null);

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }

        [Test]
        public static void Get_Error404_ReturnsPageNotFound()
        {
            var controller = new HomeController();

            IFeatureCollection features = new FeatureCollection();
            features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature { OriginalPath = "BAD" });

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(features)
            };

            var result = controller.Error(404);

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.AreEqual("PageNotFound", ((ViewResult)result).ViewName);
            Assert.AreEqual("Incorrect url BAD", ((ViewResult)result).ViewData["BadUrl"]);
        }
    }
}
