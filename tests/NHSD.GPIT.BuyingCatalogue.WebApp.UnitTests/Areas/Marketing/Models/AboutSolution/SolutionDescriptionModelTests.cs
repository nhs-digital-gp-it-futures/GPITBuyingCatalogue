using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionDescriptionModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionDescriptionModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Summary = "A solution summary", FullDescription = "A full description", AboutUrl = "A Url" }
            };

            var model = new SolutionDescriptionModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.AreEqual("A solution summary", model.Summary);
            Assert.AreEqual("A full description", model.Description);
            Assert.AreEqual("A Url", model.Link);
        }

        [Test]
        public static void WithCatalogueItem_WithoutSummary_NotComplete()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Summary = "", FullDescription = "A full description", AboutUrl = "A Url" }
            };

            var model = new SolutionDescriptionModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.IsEmpty(model.Summary);
            Assert.AreEqual("A full description", model.Description);
            Assert.AreEqual("A Url", model.Link);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new SolutionDescriptionModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Summary);
            Assert.Null(model.Description);
            Assert.Null(model.Link);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("A summary", true)]
        public static void IsCompleteIsCorrectlySet(string summary, bool? expected)
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Summary = summary }
            };

            var model = new SolutionDescriptionModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
