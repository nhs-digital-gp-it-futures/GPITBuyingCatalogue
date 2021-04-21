using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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

            Assert.NotNull(result);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void SolutionExtension_ReturnsDefaultFeaturesWhenNotSet(string features)
        {
            var solution = new Solution { Features = features };

            var result = solution.GetFeatures();

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void SolutionExtension_ReturnsDefaultHostingWhenNotSet(string hosting)
        {
            var solution = new Solution { Hosting = hosting };

            var result = solution.GetHosting();

            Assert.NotNull(result);
        }
    }
}
