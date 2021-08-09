using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    public static class FeaturesModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new FeaturesModel(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public static void WithCatalogueItem_AndNoFeatures_PropertiesCorrectlySet_AndIncomplete(string features)
        {
            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution { Features = features },
            };

            var model = new FeaturesModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Empty(model.Listing1);
            Assert.Empty(model.Listing2);
            Assert.Empty(model.Listing3);
            Assert.Empty(model.Listing4);
            Assert.Empty(model.Listing5);
            Assert.Empty(model.Listing6);
            Assert.Empty(model.Listing7);
            Assert.Empty(model.Listing8);
            Assert.Empty(model.Listing9);
            Assert.Empty(model.Listing10);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new FeaturesModel();

            Assert.False(model.IsComplete);
            Assert.Empty(model.Listing1);
            Assert.Empty(model.Listing2);
            Assert.Empty(model.Listing3);
            Assert.Empty(model.Listing4);
            Assert.Empty(model.Listing5);
            Assert.Empty(model.Listing6);
            Assert.Empty(model.Listing7);
            Assert.Empty(model.Listing8);
            Assert.Empty(model.Listing9);
            Assert.Empty(model.Listing10);
        }

        [Fact]
        public static void WithCatalogueItem_AndOneFeature_PropertiesCorrectlySet_AndComplete()
        {
            var features = new string[1] { "Feature 1" };
            var json = JsonConvert.SerializeObject(features);

            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution { Features = json },
            };

            var model = new FeaturesModel(catalogueItem);
            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.Equal("Feature 1", model.Listing1);
            Assert.Empty(model.Listing2);
            Assert.Empty(model.Listing3);
            Assert.Empty(model.Listing4);
            Assert.Empty(model.Listing5);
            Assert.Empty(model.Listing6);
            Assert.Empty(model.Listing7);
            Assert.Empty(model.Listing8);
            Assert.Empty(model.Listing9);
            Assert.Empty(model.Listing10);
        }

        [Fact]
        public static void WithCatalogueItem_AndTenFeatures_PropertiesCorrectlySet_AndComplete()
        {
            var features = new string[10] { "Feature 1", "Feature 2", "Feature 3", "Feature 4", "Feature 5", "Feature 6", "Feature 7", "Feature 8", "Feature 9", "Feature 10", };
            var json = JsonConvert.SerializeObject(features);

            var catalogueItem = new CatalogueItem
            {
                Id = new CatalogueItemId(1, "123"),
                Solution = new Solution { Features = json },
            };

            var model = new FeaturesModel(catalogueItem);
            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.Equal("Feature 1", model.Listing1);
            Assert.Equal("Feature 2", model.Listing2);
            Assert.Equal("Feature 3", model.Listing3);
            Assert.Equal("Feature 4", model.Listing4);
            Assert.Equal("Feature 5", model.Listing5);
            Assert.Equal("Feature 6", model.Listing6);
            Assert.Equal("Feature 7", model.Listing7);
            Assert.Equal("Feature 8", model.Listing8);
            Assert.Equal("Feature 9", model.Listing9);
            Assert.Equal("Feature 10", model.Listing10);
        }
    }
}
