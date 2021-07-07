using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    public static class SupportedBrowsersModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SupportedBrowsersModel(null));
        }

        [Fact]
        public static void WithCatalogueItem_PropertiesCorrectlySet()
        {
            var clientApplication = new ClientApplication
            {
                BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                MobileResponsive = true,
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new SupportedBrowsersModel(catalogueItem);

            Assert.Equal("/marketing/supplier/solution/1-123/section/browser-based", model.BackLink);
            Assert.Equal("Yes", model.MobileResponsive);
            Assert.True(model.Browsers.Single(m => m.BrowserName == "Microsoft Edge").Checked);
            Assert.Equal(7, model.Browsers.Count(m => m.BrowserName != "Microsoft Edge"));
        }

        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new SupportedBrowsersModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.Null(model.MobileResponsive);
            Assert.Null(model.Browsers);
        }

        [Theory]
        [InlineData(null, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        [InlineData(null, true, false)]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        public static void IsCompleteIsCorrectlySet(bool? mobileResponsiveness, bool includeBrowser, bool expected)
        {
            var browsers = new HashSet<string>();

            if (includeBrowser)
                browsers.Add("Microsoft Edge");

            var clientApplication = new ClientApplication
            {
                BrowsersSupported = browsers,
                MobileResponsive = mobileResponsiveness,
            };

            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = new CatalogueItemId(1, "123"),
                Solution = new Solution { ClientApplication = json },
            };

            var model = new SupportedBrowsersModel(catalogueItem);

            Assert.Equal(expected, model.IsComplete);
        }
    }
}
