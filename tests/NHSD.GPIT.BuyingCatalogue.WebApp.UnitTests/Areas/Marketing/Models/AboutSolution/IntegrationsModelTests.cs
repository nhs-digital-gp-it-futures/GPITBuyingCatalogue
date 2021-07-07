using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    public static class IntegrationsModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new IntegrationsModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { IntegrationsUrl = "Some integrations url" },
            };

            var model = new IntegrationsModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.Equal("Some integrations url", model.Link);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new IntegrationsModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Link);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("Some integrations url", true)]
        public static void IsCompleteIsCorrectlySet(string integrationsUrl, bool? expected)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { IntegrationsUrl = integrationsUrl } };

            var model = new IntegrationsModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
