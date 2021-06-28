using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutOrganisation
{
    public static class AboutSupplierModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new AboutSupplierModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Supplier = new Supplier { SupplierUrl = "A link", Summary = "A description" },
            };

            var model = new AboutSupplierModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123", model.BackLink);
            Assert.Equal("A link", model.Link);
            Assert.Equal("A description", model.Description);
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new AboutSupplierModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Description);
            Assert.Null(model.Link);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("", "", false)]
        [InlineData(" ", " ", false)]
        [InlineData("Some description", "Some link", true)]
        public static void IsCompleteIsCorrectlySet(string description, string link, bool? expected)
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Supplier = new Supplier { SupplierUrl = link, Summary = description }
            };

            var model = new AboutSupplierModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
