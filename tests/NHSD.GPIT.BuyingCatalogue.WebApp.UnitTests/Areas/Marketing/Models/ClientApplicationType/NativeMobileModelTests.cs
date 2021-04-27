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
    internal static class NativeMobileModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeMobileModel(null));
        }

        [Test]
        public static void Constructor_WithCatalogueItem_SetsBackLink()
        {
            var catalogueItem = new CatalogueItem { CatalogueItemId = "123" };

            var model = new NativeMobileModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
        }

        [Test]
        public static void WithEmptyCatalogueItem_Incomplete()
        {
            var catalogueItem = new CatalogueItem { CatalogueItemId = "123" };

            var model = new NativeMobileModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("INCOMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.MemoryAndStorageStatus);
            Assert.AreEqual("INCOMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithCompleteCatalogueItem_Complete()
        {
            var clientApplication = GetCompleteClientApplication();            
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };
            
            var model = new NativeMobileModel(catalogueItem);

            Assert.True(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryAndStorageStatus);
            Assert.AreEqual("COMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("COMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("COMPLETE", model.AdditionalInformationStatus);
        }

        [Test]        
        public static void WithMandatoryComplete_Complete()
        {
            var clientApplication = GetCompleteClientApplication();
            clientApplication.NativeMobileAdditionalInformation = null;
            clientApplication.MobileThirdParty = null;
            clientApplication.MobileConnectionDetails = null;
            clientApplication.NativeMobileHardwareRequirements = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeMobileModel(catalogueItem);

            Assert.True(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryAndStorageStatus);
            Assert.AreEqual("INCOMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithoutSupportedOperatingSystems_InComplete()
        {
            var clientApplication = GetCompleteClientApplication();
            clientApplication.MobileOperatingSystems = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeMobileModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("INCOMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryAndStorageStatus);
            Assert.AreEqual("COMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("COMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("COMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithoutMobileFirstApproach_InComplete()
        {
            var clientApplication = GetCompleteClientApplication();
            clientApplication.NativeMobileFirstDesign = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeMobileModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("INCOMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryAndStorageStatus);
            Assert.AreEqual("COMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("COMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("COMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithoutMemoryAndStorage_InComplete()
        {
            var clientApplication = GetCompleteClientApplication();
            clientApplication.MobileMemoryAndStorage = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeMobileModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.MobileFirstApproachStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.MemoryAndStorageStatus);
            Assert.AreEqual("COMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("COMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("COMPLETE", model.AdditionalInformationStatus);
        }

        private static ClientApplication GetCompleteClientApplication()
        {
            return new ClientApplication 
            {
                NativeMobileAdditionalInformation = "Some additional information",
                MobileConnectionDetails = new MobileConnectionDetails
                {
                    MinimumConnectionSpeed = "15Mbs",
                    ConnectionType = new HashSet<string> { "3G", "4G" },
                    Description = "A description"
                },
                NativeMobileHardwareRequirements = "Some hardware requirements",
                MobileMemoryAndStorage = new MobileMemoryAndStorage
                {
                    MinimumMemoryRequirement = "1GB",
                    Description = "Storage requirements"
                },
                NativeMobileFirstDesign = true,
                MobileOperatingSystems = new MobileOperatingSystems
                {
                    OperatingSystems = new HashSet<string> { "Android", "Other" },
                    OperatingSystemsDescription = "A description"
                },
                MobileThirdParty = new MobileThirdParty 
                { 
                    ThirdPartyComponents = "Third party components", 
                    DeviceCapabilities = "Device capabilities" 
                }
            };            
        }
    }
}
