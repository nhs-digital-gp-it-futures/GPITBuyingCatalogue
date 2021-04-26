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
    internal static class BrowserBasedModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedModel(null));
        }

        [Test]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new BrowserBasedModel();

            Assert.AreEqual("./", model.BackLink);
            Assert.False(model.IsComplete);            
        }

        [Test]
        public static void WithEmptyCatalogueItem_AllStatusesIncomplete()
        {
            var catalogueItem = new CatalogueItem { Solution = new Solution(), CatalogueItemId = "123" };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("INCOMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.PlugInsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithSupportedBrowsersStatusComplete_SupportedBrowsersStatus_IsComplete()
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

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("COMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.PlugInsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithMobileFirstApproachComplete_MobileFirstApproachStatus_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                MobileFirstDesign = true
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };
            
            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("INCOMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.PlugInsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithPlugInsStatusComplete_PlugInsStatusStatus_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                Plugins = new Plugins { Required = true }
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("INCOMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.PlugInsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithConnectivityStatusComplete_ConnectivityStatusStatus_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                MinimumConnectionSpeed = "15Mbs"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("INCOMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.PlugInsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithHardwareRequirementsComplete_HardwareRequirementsStatus_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                HardwareRequirements = "Some hardware requirements"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("INCOMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.PlugInsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithAdditionalInformationStatusComplete_AdditionalInformationStatus_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                AdditionalInformation = "Some additional information"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("INCOMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.PlugInsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("COMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithMandatorySectionsComplete_IsComplete()
        {
            var clientApplication = new ClientApplication
            {
                BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                MobileResponsive = true,
                MobileFirstDesign = true,
                Plugins = new Plugins { Required = true },
                MinimumConnectionSpeed = "15Mbs"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.True(model.IsComplete);

            Assert.AreEqual("COMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.PlugInsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WhenSupportedBrowsersIncomplete_IsntComplete()
        {
            var clientApplication = new ClientApplication
            {
                MobileFirstDesign = true,
                Plugins = new Plugins { Required = true },
                MinimumConnectionSpeed = "15Mbs"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("INCOMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.PlugInsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WhenMobileFirstApproachIncomplete_IsntComplete()
        {
            var clientApplication = new ClientApplication
            {
                BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                MobileResponsive = true,                
                Plugins = new Plugins { Required = true },
                MinimumConnectionSpeed = "15Mbs"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("COMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.PlugInsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WhenPluginsIncomplete_IsntComplete()
        {
            var clientApplication = new ClientApplication
            {
                BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                MobileResponsive = true,
                MobileFirstDesign = true,
                //Plugins = new Plugins { Required = true },
                MinimumConnectionSpeed = "15Mbs"
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("COMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.PlugInsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WhenConnectivityIncomplete_IsntComplete()
        {
            var clientApplication = new ClientApplication
            {
                BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                MobileResponsive = true,
                MobileFirstDesign = true,
                Plugins = new Plugins { Required = true },                
            };
            var json = JsonConvert.SerializeObject(clientApplication);
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = json }
            };

            var model = new BrowserBasedModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
            Assert.False(model.IsComplete);

            Assert.AreEqual("COMPLETE", model.SupportedBrowsersStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.PlugInsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }
    }
}
