using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class MobileFirstApproachModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new MobileFirstApproachModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication { MobileFirstDesign = true };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new MobileFirstApproachModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/browser-based", model.BackLink);
            Assert.AreEqual("Yes", model.MobileFirstApproach);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new MobileFirstApproachModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.Null(model.MobileFirstApproach);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public static void IsCompleteIsCorrectlySet(bool? mobileFirstDesign, bool? expected)
        {
            var clientApplication = new ClientApplication { MobileFirstDesign = mobileFirstDesign };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new MobileFirstApproachModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
