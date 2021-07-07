using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    public static class SolutionDescriptionModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new SolutionDescriptionModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { Summary = "A solution summary", FullDescription = "A full description", AboutUrl = "A Url" },
            };

            var model = new SolutionDescriptionModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.Equal("A solution summary", model.Summary);
            Assert.Equal("A full description", model.Description);
            Assert.Equal("A Url", model.Link);
        }

        [Fact]
        public static void WithCatalogueItem_WithoutSummary_NotComplete()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { Summary = "", FullDescription = "A full description", AboutUrl = "A Url" },
            };

            var model = new SolutionDescriptionModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Empty(model.Summary);
            Assert.Equal("A full description", model.Description);
            Assert.Equal("A Url", model.Link);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new SolutionDescriptionModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Summary);
            Assert.Null(model.Description);
            Assert.Null(model.Link);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("A summary", true)]
        public static void IsCompleteIsCorrectlySet(string summary, bool? expected)
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { Summary = summary },
            };

            var model = new SolutionDescriptionModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
