using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class HomeControllerTests
    {
        [Theory]
        [CommonAutoData]
        public static void Get_Index_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.Index().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_PrivacyPolicy_ReturnsDefaultView(
            HomeController controller)
        {
            var result = controller.PrivacyPolicy().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Error500_ReturnsDefaultErrorView(
            HomeController controller)
        {
            var result = controller.Error(500).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ErrorNullStatus_ReturnsDefaultErrorView(
            HomeController controller)
        {
            var result = controller.Error(null).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Error404_ReturnsPageNotFound(
            [Frozen] Mock<IFeatureCollection> features,
            HomeController controller)
        {
            features.Setup(c => c.Get<IStatusCodeReExecuteFeature>()).Returns(new StatusCodeReExecuteFeature { OriginalPath = "BAD" });

            var result = controller.Error(404).As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().Be("PageNotFound");
            result.ViewData.Should().Contain(d => string.Equals(d.Key, "BadUrl") && string.Equals(d.Value, "Incorrect url BAD"));
        }

        [Theory]
        [CommonAutoData]
        public static void Get_ErrorWithErrorValue_ReturnsErrorViewModel(
            string error,
            HomeController controller)
        {
            var expectedModel = new ErrorModel(error);

            var result = controller.Error(error: error).As<ViewResult>();

            result.Should().NotBeNull();
            result.Model.As<ErrorModel>().Should().BeEquivalentTo(expectedModel);
        }
    }
}
