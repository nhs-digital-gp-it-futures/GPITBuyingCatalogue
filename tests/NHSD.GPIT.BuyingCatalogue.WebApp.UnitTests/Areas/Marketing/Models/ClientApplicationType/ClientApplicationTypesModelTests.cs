using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType

{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ClientApplicationTypesModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ClientApplicationTypesModel(null));
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new ClientApplicationTypesModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);
            Assert.False(model.BrowserBased);
            Assert.False(model.NativeMobile);
            Assert.False(model.NativeDesktop);
        }

        [Test]
        public static void WithAllBrowserTypesChecked_PropertiesAreTrue_AndIsComplete()
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new HashSet<string> { "browser-based", "native-mobile", "native-desktop" }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new ClientApplicationTypesModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.True(model.BrowserBased);
            Assert.True(model.NativeMobile);
            Assert.True(model.NativeDesktop);
        }

        [Test]
        public static void WithBrowserBasedOnlyChecked_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new HashSet<string> { "browser-based" }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new ClientApplicationTypesModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.True(model.BrowserBased);
            Assert.False(model.NativeMobile);
            Assert.False(model.NativeDesktop);
        }

        [Test]
        public static void WithNativeMobileOnlyChecked_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new HashSet<string> { "native-mobile" }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new ClientApplicationTypesModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.False(model.BrowserBased);
            Assert.True(model.NativeMobile);
            Assert.False(model.NativeDesktop);
        }

        [Test]
        public static void WithNativeDesktopOnlyChecked_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                ClientApplicationTypes = new HashSet<string> { "native-desktop" }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new ClientApplicationTypesModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);
            Assert.False(model.BrowserBased);
            Assert.False(model.NativeMobile);
            Assert.True(model.NativeDesktop);
        }
    }
}
