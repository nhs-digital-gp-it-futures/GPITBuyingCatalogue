using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SupportedBrowsersModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SupportedBrowsersModel(null));
        }

        [Test]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                MobileResponsive = true
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new SupportedBrowsersModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123/section/browser-based", model.BackLink);
            Assert.AreEqual("Yes", model.MobileResponsive);
            Assert.True(model.Browsers.Single(x => x.BrowserName == "Microsoft Edge").Checked);
            Assert.AreEqual(7, model.Browsers.Count(x => x.BrowserName != "Microsoft Edge"));
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new SupportedBrowsersModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.MobileResponsive);
            Assert.Null(model.Browsers);
        }

        [Test]
        [TestCase(null, null, false)]
        [TestCase(true, null, false)]
        [TestCase(false, null, false)]
        [TestCase(null, true, false)]
        [TestCase(true, true, true)]
        [TestCase(false, true, true)]
        public static void IsCompleteIsCorrectlySet(bool? mobileResponsiveness, bool includeBrowser, bool expected)
        {
            var browsers = new HashSet<string>();

            if (includeBrowser)
                browsers.Add("Microsoft Edge");

            var clientApplication = new ClientApplication
            {
                BrowsersSupported = browsers,
                MobileResponsive = mobileResponsiveness
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new SupportedBrowsersModel(catalogueItem);

            Assert.AreEqual(expected, model.IsComplete);
        }
    }
}
