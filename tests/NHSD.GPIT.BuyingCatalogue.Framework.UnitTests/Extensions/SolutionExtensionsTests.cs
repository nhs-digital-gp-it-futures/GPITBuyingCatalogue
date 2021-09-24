using System.Collections.Generic;
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
            var solution = new Solution { ClientApplication = JsonSerializer.Serialize(clientApplication) };

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
            var features = new string[3] { "Feature 1", "Feature 2", "Feature 3" };
            var solution = new Solution { Features = JsonSerializer.Serialize(features) };

            var result = solution.GetFeatures();

            result.Should().BeEquivalentTo(features);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void SolutionExtension_ReturnsDefaultHostingWhenNotSet(string hosting)
        {
            var solution = new Solution { Hosting = hosting };

            var result = solution.GetHosting();

            result.Should().BeEquivalentTo(new Hosting());
        }

        [Fact]
        public static void SolutionExtension_ReturnsHostingWhenSet()
        {
            var hosting = new Hosting { HybridHostingType = new HybridHostingType { Summary = "Hybrid Summary" } };
            var solution = new Solution { Hosting = JsonSerializer.Serialize(hosting) };

            var result = solution.GetHosting();

            result.Should().BeEquivalentTo(hosting);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void SolutionExtension_ReturnsDefaultIntegrationsWhenNotSet(string integrations)
        {
            var solution = new Solution { Integrations = integrations };

            var result = solution.GetIntegrations();

            result.Should().BeEquivalentTo(new List<Integration>());
        }

        [Fact]
        public static void SolutionExtension_ReturnsIntegrationsWhenSet()
        {
            var expected = new List<Integration>
            {
                new Integration { Description = "Description 1" },
                new Integration { Description = "Description 2" },
            };

            var solution = new Solution { Integrations = JsonSerializer.Serialize(expected) };

            var result = solution.GetIntegrations();

            result.Should().BeEquivalentTo(expected);
        }
    }
}
