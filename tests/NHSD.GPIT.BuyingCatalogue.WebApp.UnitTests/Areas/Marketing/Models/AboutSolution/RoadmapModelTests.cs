using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class RoadmapModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new RoadmapModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { RoadMap = "A solution road map" },
            };

            var model = new RoadmapModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.AreEqual("A solution road map", model.Summary);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new RoadmapModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Summary);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("A solution road map", true)]
        public static void IsCompleteIsCorrectlySet(string roadMap, bool? expected)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { RoadMap = roadMap } };

            var model = new RoadmapModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
