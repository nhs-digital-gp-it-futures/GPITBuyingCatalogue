using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutOrganisation
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AboutSupplierModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSupplierModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Supplier = new Supplier { SupplierUrl = "A link", Summary = "A description" }
            };

            var model = new AboutSupplierModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.AreEqual("A link", model.Link);
            Assert.AreEqual("A description", model.Description);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new AboutSupplierModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Description);
            Assert.Null(model.Link);
        }

        [Test]
        [TestCase(null, null, false)]
        [TestCase("", "", false)]
        [TestCase(" ", " ", false)]
        [TestCase("Some description", "Some link", true)]
        public static void IsCompleteIsCorrectlySet(string descripion, string link, bool? expected )
        {
            var catalogueItem = new CatalogueItem 
            { 
                CatalogueItemId = "123",
                Supplier = new Supplier {SupplierUrl = link, Summary = descripion }
            };

            var model = new AboutSupplierModel(catalogueItem);
            
            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
