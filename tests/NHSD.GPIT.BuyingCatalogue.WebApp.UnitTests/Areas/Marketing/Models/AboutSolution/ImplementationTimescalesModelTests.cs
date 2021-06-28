using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    public static class ImplementationTimescalesModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new ImplementationTimescalesModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ImplementationDetail = "Some implementation detail" },
            };

            var model = new ImplementationTimescalesModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.Equal("Some implementation detail", model.Description);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ImplementationTimescalesModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Description);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("Some implementation detail", true)]
        public static void IsCompleteIsCorrectlySet(string implementationDetail, bool? expected)
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ImplementationDetail = implementationDetail } };

            var model = new ImplementationTimescalesModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
