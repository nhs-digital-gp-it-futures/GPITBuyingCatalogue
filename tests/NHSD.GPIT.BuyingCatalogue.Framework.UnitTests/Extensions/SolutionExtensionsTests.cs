using System;
using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class SolutionExtensionsTests
    {
        [Fact]
        public static void GetClientApplication_NullSolution_ThrowsException()
        {
            Solution solution = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => solution.GetApplicationTypes());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void GetClientApplication_ReturnsDefaultClientApplicationWhenNotSet(string clientApplication)
        {
            var solution = new Solution { ApplicationType = clientApplication };

            var result = solution.GetApplicationTypes();

            result.Should().BeEquivalentTo(new ApplicationTypes());
        }

        [Fact]
        public static void GetClientApplication_ReturnsClientApplicationWhenSet()
        {
            var clientApplication = new ApplicationTypes { AdditionalInformation = "Additional Information" };
            var json = JsonSerializer.Serialize(clientApplication);
            var solution = new Solution { ApplicationType = json };

            var result = solution.GetApplicationTypes();

            result.Should().BeEquivalentTo(clientApplication);
        }

        [Fact]
        public static void GetFeatures_NullSolution_ThrowsException()
        {
            Solution solution = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => solution.GetFeatures());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void GetFeatures_ReturnsDefaultFeaturesWhenNotSet(string features)
        {
            var solution = new Solution { Features = features };

            var result = solution.GetFeatures();

            result.Should().BeEquivalentTo();
        }

        [Fact]
        public static void GetIntegrations_NullSolution_ThrowsException()
        {
            Solution solution = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => solution.GetIntegrations());
        }

        [Fact]
        public static void GetIntegrations_ReturnsFeaturesWhenSet()
        {
            var features = new[] { "Feature 1", "Feature 2", "Feature 3" };
            var json = JsonSerializer.Serialize(features);
            var solution = new Solution { Features = json };

            var result = solution.GetFeatures();

            result.Should().BeEquivalentTo(features);
        }
    }
}
