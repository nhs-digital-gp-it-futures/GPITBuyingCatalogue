using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.ClientApplicationType
{
    public static class BrowserBasedModelTests
    {
        [Fact]
        public static void WithoutCatalogueItem_PropertiesAreDefaulted()
        {
            var model = new BrowserBasedModel();

            Assert.Equal("./", model.BackLink);
            Assert.False(model.IsComplete);
        }

        [Fact]
        public static void WithEmptyCatalogueItem_AllStatusesIncomplete()
        {
            var model = new BrowserBasedModel();

            Assert.False(model.IsComplete);

            Assert.Equal("Incomplete", model.SupportedBrowsersStatus);
            Assert.Equal("Incomplete", model.MobileFirstApproachStatus);
            Assert.Equal("Incomplete", model.PlugInsStatus);
            Assert.Equal("Incomplete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WithSupportedBrowsersStatusComplete_SupportedBrowsersStatus_IsComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                    MobileResponsive = true,
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Complete", model.SupportedBrowsersStatus);
            Assert.Equal("Incomplete", model.MobileFirstApproachStatus);
            Assert.Equal("Incomplete", model.PlugInsStatus);
            Assert.Equal("Incomplete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WithMobileFirstApproachComplete_MobileFirstApproachStatus_IsComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    MobileFirstDesign = true,
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Incomplete", model.SupportedBrowsersStatus);
            Assert.Equal("Complete", model.MobileFirstApproachStatus);
            Assert.Equal("Incomplete", model.PlugInsStatus);
            Assert.Equal("Incomplete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WithPlugInsStatusComplete_PlugInsStatusStatus_IsComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    Plugins = new Plugins { Required = true },
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Incomplete", model.SupportedBrowsersStatus);
            Assert.Equal("Incomplete", model.MobileFirstApproachStatus);
            Assert.Equal("Complete", model.PlugInsStatus);
            Assert.Equal("Incomplete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WithConnectivityStatusComplete_ConnectivityStatusStatus_IsComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    MinimumConnectionSpeed = "15Mbs",
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Incomplete", model.SupportedBrowsersStatus);
            Assert.Equal("Incomplete", model.MobileFirstApproachStatus);
            Assert.Equal("Incomplete", model.PlugInsStatus);
            Assert.Equal("Complete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WithHardwareRequirementsComplete_HardwareRequirementsStatus_IsComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    HardwareRequirements = "Some hardware requirements",
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Incomplete", model.SupportedBrowsersStatus);
            Assert.Equal("Incomplete", model.MobileFirstApproachStatus);
            Assert.Equal("Incomplete", model.PlugInsStatus);
            Assert.Equal("Incomplete", model.ConnectivityStatus);
            Assert.Equal("Complete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WithAdditionalInformationStatusComplete_AdditionalInformationStatus_IsComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    AdditionalInformation = "Some additional information",
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Incomplete", model.SupportedBrowsersStatus);
            Assert.Equal("Incomplete", model.MobileFirstApproachStatus);
            Assert.Equal("Incomplete", model.PlugInsStatus);
            Assert.Equal("Incomplete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Complete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WithMandatorySectionsComplete_IsComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                    MobileResponsive = true,
                    MobileFirstDesign = true,
                    Plugins = new Plugins { Required = true },
                    MinimumConnectionSpeed = "15Mbs",
                },
            };

            Assert.True(model.IsComplete);

            Assert.Equal("Complete", model.SupportedBrowsersStatus);
            Assert.Equal("Complete", model.MobileFirstApproachStatus);
            Assert.Equal("Complete", model.PlugInsStatus);
            Assert.Equal("Complete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WhenSupportedBrowsersIncomplete_IsntComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    MobileFirstDesign = true,
                    Plugins = new Plugins { Required = true },
                    MinimumConnectionSpeed = "15Mbs",
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Incomplete", model.SupportedBrowsersStatus);
            Assert.Equal("Complete", model.MobileFirstApproachStatus);
            Assert.Equal("Complete", model.PlugInsStatus);
            Assert.Equal("Complete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WhenMobileFirstApproachIncomplete_IsntComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                    MobileResponsive = true,
                    Plugins = new Plugins { Required = true },
                    MinimumConnectionSpeed = "15Mbs",
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Complete", model.SupportedBrowsersStatus);
            Assert.Equal("Incomplete", model.MobileFirstApproachStatus);
            Assert.Equal("Complete", model.PlugInsStatus);
            Assert.Equal("Complete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WhenPluginsIncomplete_IsntComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                    MobileResponsive = true,
                    MobileFirstDesign = true,
                    MinimumConnectionSpeed = "15Mbs",
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Complete", model.SupportedBrowsersStatus);
            Assert.Equal("Complete", model.MobileFirstApproachStatus);
            Assert.Equal("Incomplete", model.PlugInsStatus);
            Assert.Equal("Complete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }

        [Fact]
        public static void WhenConnectivityIncomplete_IsntComplete()
        {
            var model = new BrowserBasedModel
            {
                ClientApplication = new ClientApplication
                {
                    BrowsersSupported = new HashSet<string> { "Microsoft Edge" },
                    MobileResponsive = true,
                    MobileFirstDesign = true,
                    Plugins = new Plugins { Required = true },
                },
            };

            Assert.False(model.IsComplete);

            Assert.Equal("Complete", model.SupportedBrowsersStatus);
            Assert.Equal("Complete", model.MobileFirstApproachStatus);
            Assert.Equal("Complete", model.PlugInsStatus);
            Assert.Equal("Incomplete", model.ConnectivityStatus);
            Assert.Equal("Incomplete", model.HardwareRequirementsStatus);
            Assert.Equal("Incomplete", model.AdditionalInformationStatus);
        }
    }
}
