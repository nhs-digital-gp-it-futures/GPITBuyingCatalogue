using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class HomeControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(HomeController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void Get_Index_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.Index().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }
    }
}
