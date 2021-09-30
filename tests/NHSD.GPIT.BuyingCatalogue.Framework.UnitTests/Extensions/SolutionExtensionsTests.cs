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
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void SolutionExtension_ReturnsDefaultClientApplicationWhenNotSet(string clientApplication)
        {
            var solution = new Solution { ClientApplication = clientApplication };

            var result = solution.GetClientApplication();

            result.Should().BeEquivalentTo(new ClientApplication());
        }

        [Fact]
        public static void SolutionExtension_ReturnsClientApplicationWhenSet()
        {
            var clientApplication = new ClientApplication { AdditionalInformation = "Additional Information" };
            var json = JsonSerializer.Serialize(clientApplication);
            var solution = new Solution { ClientApplication = json };

            var result = solution.GetClientApplication();

            result.Should().BeEquivalentTo(clientApplication);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void SolutionExtension_ReturnsDefaultFeaturesWhenNotSet(string features)
        {
            var solution = new Solution { Features = features };

            var result = solution.GetFeatures();

            result.Should().BeEquivalentTo();
        }

        [Fact]
        public static void SolutionExtension_ReturnsFeaturesWhenSet()
        {
            var features = new[] { "Feature 1", "Feature 2", "Feature 3" };
            var json = JsonSerializer.Serialize(features);
            var solution = new Solution { Features = json };

            var result = solution.GetFeatures();

            result.Should().BeEquivalentTo(features);
        }
    }
}
