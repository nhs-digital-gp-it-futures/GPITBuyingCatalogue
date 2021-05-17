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
    internal static class PlugInsOrExtensionsModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new PlugInsOrExtensionsModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                Plugins = new Plugins { Required = true, AdditionalInformation = "Some more information" }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new PlugInsOrExtensionsModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/browser-based", model.BackLink);
            Assert.AreEqual("Yes", model.PlugInsRequired);
            Assert.AreEqual("Some more information", model.AdditionalInformation);
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new PlugInsOrExtensionsModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.Null(model.IsComplete);
            Assert.Null(model.PlugInsRequired);
            Assert.Null(model.AdditionalInformation);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public static void IsCompleteIsCorrectlySet(bool? required, bool? expected)
        {
            var clientApplication = new ClientApplication
            {
                Plugins = new Plugins { Required = required, AdditionalInformation = "Some more information" }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem { Solution = new Solution { ClientApplication = json } };

            var model = new PlugInsOrExtensionsModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
