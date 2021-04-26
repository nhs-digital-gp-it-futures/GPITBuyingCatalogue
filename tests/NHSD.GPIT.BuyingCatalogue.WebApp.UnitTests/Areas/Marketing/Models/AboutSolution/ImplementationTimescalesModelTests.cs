using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.AboutSolution
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ImplementationTimescalesModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ImplementationTimescalesModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var catalogueItem = new CatalogueItem 
                { 
                    CatalogueItemId = "123",
                    Solution = new Solution { ImplementationDetail = "Some implementation detail" } 
                };
            
            var model = new ImplementationTimescalesModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.AreEqual("Some implementation detail", model.Description);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ImplementationTimescalesModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.Description);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("Some implementation detail", true)]
        public static void IsCompleteIsCorrectlySet(string implementationDetail, bool? expected )
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution { ImplementationDetail = implementationDetail } };

            var model = new ImplementationTimescalesModel(catalogueItem);
            
            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
