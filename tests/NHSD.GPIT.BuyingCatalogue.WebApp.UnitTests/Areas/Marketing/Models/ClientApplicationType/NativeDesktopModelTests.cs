using System;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeDesktopModelTests
    {
        [Test]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeDesktopModel(null));
        }

        [Test]
        public static void Constructor_WithCatalogueItem_SetsBackLink()
        {
            var catalogueItem = new CatalogueItem { CatalogueItemId = "123" };

            var model = new NativeDesktopModel(catalogueItem);

            Assert.AreEqual("/marketing/supplier/solution/123", model.BackLink);
        }

        [Test]
        public static void WithEmptyCatalogueItem_Incomplete()
        {
            var catalogueItem = new CatalogueItem { CatalogueItemId = "123" };

            var model = new NativeDesktopModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("INCOMPLETE", model.SupportedOperatingSystemsStatus);            
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.MemoryStatus);
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

            var model = new NativeDesktopModel(catalogueItem);

            Assert.True(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryStatus);
            Assert.AreEqual("COMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("COMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("COMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithMandatoryComplete_Complete()
        {
            var clientApplication = GetCompleteClientApplication();
            clientApplication.NativeDesktopThirdParty = null;
            clientApplication.NativeDesktopHardwareRequirements = null;
            clientApplication.NativeDesktopAdditionalInformation = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeDesktopModel(catalogueItem);

            Assert.True(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryStatus);
            Assert.AreEqual("INCOMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithoutSupportedOperatingSystems_InComplete()
        {
            var clientApplication = GetCompleteClientApplication();
            clientApplication.NativeDesktopOperatingSystemsDescription = null;
            clientApplication.NativeDesktopThirdParty = null;
            clientApplication.NativeDesktopHardwareRequirements = null;
            clientApplication.NativeDesktopAdditionalInformation = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeDesktopModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("INCOMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryStatus);
            Assert.AreEqual("INCOMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithoutConnectivityStatus_InComplete()
        {
            var clientApplication = GetCompleteClientApplication();
            clientApplication.NativeDesktopMinimumConnectionSpeed = null;
            clientApplication.NativeDesktopThirdParty = null;
            clientApplication.NativeDesktopHardwareRequirements = null;
            clientApplication.NativeDesktopAdditionalInformation = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeDesktopModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("INCOMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("COMPLETE", model.MemoryStatus);
            Assert.AreEqual("INCOMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        [Test]
        public static void WithoutMemoryStatus_InComplete()
        {
            var clientApplication = GetCompleteClientApplication();
            
            clientApplication.NativeDesktopMemoryAndStorage = null;
            clientApplication.NativeDesktopThirdParty = null;
            clientApplication.NativeDesktopHardwareRequirements = null;
            clientApplication.NativeDesktopAdditionalInformation = null;
            var catalogueItem = new CatalogueItem
            {
                CatalogueItemId = "123",
                Solution = new Solution { ClientApplication = JsonConvert.SerializeObject(clientApplication) }
            };

            var model = new NativeDesktopModel(catalogueItem);

            Assert.False(model.IsComplete);
            Assert.AreEqual("COMPLETE", model.SupportedOperatingSystemsStatus);
            Assert.AreEqual("COMPLETE", model.ConnectivityStatus);
            Assert.AreEqual("INCOMPLETE", model.MemoryStatus);
            Assert.AreEqual("INCOMPLETE", model.ThirdPartyStatus);
            Assert.AreEqual("INCOMPLETE", model.HardwareRequirementsStatus);
            Assert.AreEqual("INCOMPLETE", model.AdditionalInformationStatus);
        }

        private static ClientApplication GetCompleteClientApplication()
        {
            return new ClientApplication
            {
                NativeDesktopOperatingSystemsDescription = "Some operating system details",
                NativeDesktopMinimumConnectionSpeed = "15Mbs",
                MinimumDesktopResolution = "21:9 - 3440 x 1440",
                NativeDesktopMemoryAndStorage = new NativeDesktopMemoryAndStorage
                {
                    MinimumMemoryRequirement = "1GB",
                    StorageRequirementsDescription = "Storage requirements",
                    MinimumCpu = "Xeon",
                    RecommendedResolution = "4:3 - 1024 x 768"
                },
                NativeDesktopThirdParty = new NativeDesktopThirdParty 
                { 
                    ThirdPartyComponents = "Third party components", 
                    DeviceCapabilities = "Device capabilities" 
                },
                NativeDesktopHardwareRequirements = "Some hardware requirements",
                NativeDesktopAdditionalInformation = "Some additional information"
            };
        }
    }
}
