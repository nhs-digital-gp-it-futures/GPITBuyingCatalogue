using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class IntegrationsModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new IntegrationsModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { IntegrationsUrl = "Some integrations url" }
            };

            var model = new IntegrationsModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.AreEqual("Some integrations url", model.Link);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new IntegrationsModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Link);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("Some integrations url", true)]
        public static void IsCompleteIsCorrectlySet(string integrationsUrl, bool? expected)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { IntegrationsUrl = integrationsUrl } };

            var model = new IntegrationsModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
