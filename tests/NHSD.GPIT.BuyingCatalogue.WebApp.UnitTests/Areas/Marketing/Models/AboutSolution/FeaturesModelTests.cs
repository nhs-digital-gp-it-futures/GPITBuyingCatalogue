using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class FeaturesModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new FeaturesModel(null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void WithCatalogueItem_AndNoFeatures_PropertiesCorrectlySet_AndIncomplete(string features)
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Features = features }
            };

            var model = new FeaturesModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.IsEmpty(model.Listing1);
            Assert.IsEmpty(model.Listing2);
            Assert.IsEmpty(model.Listing3);
            Assert.IsEmpty(model.Listing4);
            Assert.IsEmpty(model.Listing5);
            Assert.IsEmpty(model.Listing6);
            Assert.IsEmpty(model.Listing7);
            Assert.IsEmpty(model.Listing8);
            Assert.IsEmpty(model.Listing9);
            Assert.IsEmpty(model.Listing10);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new FeaturesModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.IsEmpty(model.Listing1);
            Assert.IsEmpty(model.Listing2);
            Assert.IsEmpty(model.Listing3);
            Assert.IsEmpty(model.Listing4);
            Assert.IsEmpty(model.Listing5);
            Assert.IsEmpty(model.Listing6);
            Assert.IsEmpty(model.Listing7);
            Assert.IsEmpty(model.Listing8);
            Assert.IsEmpty(model.Listing9);
            Assert.IsEmpty(model.Listing10);
        }

        [Test]
        public static void WithCatalogueItem_AndOneFeature_PropertiesCorrectlySet_AndComplete()
        {
            var features = new string[1] { "Feature 1" };
            var json = JsonConvert.SerializeObject(features);

            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Features = json }
            };

            var model = new FeaturesModel(catalogueItem);
            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.AreEqual("Feature 1", model.Listing1);
            Assert.IsEmpty(model.Listing2);
            Assert.IsEmpty(model.Listing3);
            Assert.IsEmpty(model.Listing4);
            Assert.IsEmpty(model.Listing5);
            Assert.IsEmpty(model.Listing6);
            Assert.IsEmpty(model.Listing7);
            Assert.IsEmpty(model.Listing8);
            Assert.IsEmpty(model.Listing9);
            Assert.IsEmpty(model.Listing10);
        }

        [Test]
        public static void WithCatalogueItem_AndTenFeatures_PropertiesCorrectlySet_AndComplete()
        {
            var features = new string[10] { "Feature 1", "Feature 2", "Feature 3", "Feature 4", "Feature 5", "Feature 6", "Feature 7", "Feature 8", "Feature 9", "Feature 10", };
            var json = JsonConvert.SerializeObject(features);

            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { Features = json }
            };

            var model = new FeaturesModel(catalogueItem);
            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.AreEqual("Feature 1", model.Listing1);
            Assert.AreEqual("Feature 2", model.Listing2);
            Assert.AreEqual("Feature 3", model.Listing3);
            Assert.AreEqual("Feature 4", model.Listing4);
            Assert.AreEqual("Feature 5", model.Listing5);
            Assert.AreEqual("Feature 6", model.Listing6);
            Assert.AreEqual("Feature 7", model.Listing7);
            Assert.AreEqual("Feature 8", model.Listing8);
            Assert.AreEqual("Feature 9", model.Listing9);
            Assert.AreEqual("Feature 10", model.Listing10);
        }
    }
}
