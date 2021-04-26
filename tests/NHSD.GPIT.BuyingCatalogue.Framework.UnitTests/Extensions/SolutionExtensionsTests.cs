using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionExtensionsTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void SolutionExtension_ReturnsDefaultClientApplicationWhenNotSet(string clientApplication)
        {
            var solution = new Solution { ClientApplication = clientApplication };

            var result = solution.GetClientApplication();
            
            result.Should().BeEquivalentTo(new ClientApplication());
        }

        [Test]
        public static void SolutionExtension_ReturnsClientApplicationWhenSet()
        {
            var clientApplication = new ClientApplication { AdditionalInformation = "Additional Information" };
            var json = JsonConvert.SerializeObject(clientApplication);
            var solution = new Solution { ClientApplication = json };

            var result = solution.GetClientApplication();

            result.Should().BeEquivalentTo(clientApplication);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void SolutionExtension_ReturnsDefaultFeaturesWhenNotSet(string features)
        {
            var solution = new Solution { Features = features };

            var result = solution.GetFeatures();

            result.Should().BeEquivalentTo(new string[0]);
        }

        [Test]
        public static void SolutionExtension_ReturnsFeaturesWhenSet()
        {
            var features = new string[3] { "Feature 1", "Feature 2", "Feature 3" };
            var json = JsonConvert.SerializeObject(features);
            var solution = new Solution { Features = json };

            var result = solution.GetFeatures();

            result.Should().BeEquivalentTo(features);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void SolutionExtension_ReturnsDefaultHostingWhenNotSet(string hosting)
        {
            var solution = new Solution { Hosting = hosting };

            var result = solution.GetHosting();
            
            result.Should().BeEquivalentTo(new Hosting());
        }

        [Test]
        public static void SolutionExtension_ReturnsHostingWhenSet()
        {
            var hosting = new Hosting { HybridHostingType = new HybridHostingType { Summary = "Hybrid Summary" } };
            var json = JsonConvert.SerializeObject(hosting);
            var solution = new Solution { Hosting = json };

            var result = solution.GetHosting();

            result.Should().BeEquivalentTo(hosting);
        }
    }
}
