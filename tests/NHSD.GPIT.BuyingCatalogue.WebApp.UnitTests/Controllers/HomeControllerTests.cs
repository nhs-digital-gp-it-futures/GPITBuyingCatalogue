using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class HomeControllerTests
    {
        [Fact]
        public static void Get_PrivacyPolicy_ReturnsDefaultView()
        {
            var controller = new HomeController();

            var result = controller.PrivacyPolicy() as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Fact]
        public static void Get_Error500_ReturnsDefaultErrorView()
        {
            var controller = new HomeController();

            var result = controller.Error(500);

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Fact]
        public static void Get_ErrorNullStatus_ReturnsDefaultErrorView()
        {
            var controller = new HomeController();

            var result = controller.Error(null);

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }

        [Fact]
        public static void Get_Error404_ReturnsPageNotFound()
        {
            var controller = new HomeController();

            IFeatureCollection features = new FeatureCollection();
            features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature { OriginalPath = "BAD" });

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(features),
            };

            var result = controller.Error(404);

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Equal("PageNotFound", ((ViewResult)result).ViewName);
            Assert.Equal("Incorrect url BAD", ((ViewResult)result).ViewData["BadUrl"]);
        }
    }
}
