using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class VaccinationsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(VaccinationsController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
        }

        [Fact]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new VaccinationsController();

            var result = controller.Index();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.Null(((ViewResult)result).ViewName);
        }
    }
}
