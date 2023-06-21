using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Transactions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Fixtures;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database
{
    public class ApplicationTypeJsonTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;

        public ApplicationTypeJsonTests(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void ApplicationTypeNull()
        {
            var catalogueItem = GetCatalogueItem();

            using var scope = new TransactionScope();
            SaveAndUpdateApplicationTypeJson(catalogueItem);

            using var dbContext = fixture.GetDbContext();
            var solution = dbContext.Solutions.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id);
            solution.ApplicationTypeDetail.Should().BeNull();
        }

        [Fact]
        public void ApplicationType_EmptyString_Throws()
        {
            var catalogueItem = GetCatalogueItem();

            using var scope = new TransactionScope();
            SaveAndUpdateApplicationTypeJson(catalogueItem, string.Empty);

            using var dbContext = fixture.GetDbContext();
            dbContext.Solutions.Invoking(s => s.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id))
                .Should().Throw<JsonException>();
        }

        [Fact]
        public void ApplicationTypeEmptyJSONObject()
        {
            var catalogueItem = GetCatalogueItem();

            using var scope = new TransactionScope();
            SaveAndUpdateApplicationTypeJson(catalogueItem, "{}");

            using var dbContext = fixture.GetDbContext();
            var solution = dbContext.Solutions.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id);
            solution.ApplicationTypeDetail.Should().NotBeNull();
            solution.ApplicationTypeDetail.Should().BeEquivalentTo(new ApplicationTypeDetail());
        }

        [Fact]
        public void ApplicationType_AdditionalInformation()
        {
            var catalogueItem = GetCatalogueItem();

            using var scope = new TransactionScope();
            var json = @"{ ""AdditionalInformation"": ""value"" }";
            SaveAndUpdateApplicationTypeJson(catalogueItem, json);

            using var dbContext = fixture.GetDbContext();
            var solution = dbContext.Solutions.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id);
            solution.ApplicationTypeDetail.Should().NotBeNull();
            solution.ApplicationTypeDetail.Should().BeEquivalentTo(new ApplicationTypeDetail() { AdditionalInformation = "value" });
        }

        [Fact]
        public void ApplicationType_ApplicationTypes()
        {
            var catalogueItem = GetCatalogueItem();

            using var scope = new TransactionScope();
            var json = @"{
                ""ClientApplicationTypes"": [
                ""browser-based""
            ]}";

            SaveAndUpdateApplicationTypeJson(catalogueItem, json);

            using var dbContext = fixture.GetDbContext();
            var solution = dbContext.Solutions.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id);
            solution.ApplicationTypeDetail.Should().NotBeNull();
            solution.ApplicationTypeDetail.Should().BeEquivalentTo(new ApplicationTypeDetail() { ApplicaitonTypes = new HashSet<string>() { "browser-based" } });
        }

        [Fact]
        public void ApplicationTypeBrowsersSupportedOldSTyle()
        {
            var catalogueItem = GetCatalogueItem();

            using var scope = new TransactionScope();
            var json = GetJSONWithOldBrowsersSupportedSchema();
            SaveAndUpdateApplicationTypeJson(catalogueItem, json);

            using var dbContext = fixture.GetDbContext();
            var solution = dbContext.Solutions.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id);
            solution.ApplicationTypeDetail.Should().NotBeNull();
            var expected = new ApplicationTypeDetail()
            {
                BrowsersSupported = new HashSet<SupportedBrowser>
                        {
                            new SupportedBrowser() { BrowserName = "Google Chrome" },
                            new SupportedBrowser() { BrowserName = "Chromium" },
                            new SupportedBrowser() { BrowserName = "Internet Explorer 11" },
                            new SupportedBrowser() { BrowserName = "Internet Explorer 10" },
                        },
            };
            solution.ApplicationTypeDetail.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ApplicationTypeBrowsersSupportedNewSTyle()
        {
            var catalogueItem = GetCatalogueItem();

            using var scope = new TransactionScope();
            var json = GetJSONWithNewBrowsersSupportedSchema();
            SaveAndUpdateApplicationTypeJson(catalogueItem, json);

            using var dbContext = fixture.GetDbContext();
            var solution = dbContext.Solutions.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id);
            solution.ApplicationTypeDetail.Should().NotBeNull();
            var expected = new ApplicationTypeDetail()
            {
                BrowsersSupported = new HashSet<SupportedBrowser>
                        {
                            new SupportedBrowser() { BrowserName = "Google Chrome" },
                            new SupportedBrowser() { BrowserName = "Chromium" },
                            new SupportedBrowser() { BrowserName = "Internet Explorer 11" },
                            new SupportedBrowser() { BrowserName = "Internet Explorer 10" },
                        },
            };
            solution.ApplicationTypeDetail.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ApplicationTypeInstance()
        {
            var catalogueItem = GetCatalogueItem();

            using (var scope = new TransactionScope())
            {
                var json = GetJSON();
                SaveAndUpdateApplicationTypeJson(catalogueItem, json);

                using (var dbContext = fixture.GetDbContext())
                {
                    var solution = dbContext.Solutions.FirstOrDefault(s => s.CatalogueItemId == catalogueItem.Id);
                    solution.ApplicationTypeDetail.Should().NotBeNull();
                    solution.ApplicationTypeDetail.BrowsersSupported.Should().NotBeNull();
                }
            }
        }

        private static string GetJSONWithNewBrowsersSupportedSchema()
        {
            return @"
            {
                ""BrowsersSupported"": [
                    {
                        ""BrowserName"": ""Google Chrome"",
                        ""MinimumBrowserVersion"": null
                    },
                    {
                        ""BrowserName"": ""Chromium"",
                        ""MinimumBrowserVersion"": null
                    },
                    {
                        ""BrowserName"": ""Internet Explorer 11"",
                        ""MinimumBrowserVersion"": null
                    },
                    {
                        ""BrowserName"": ""Internet Explorer 10"",
                        ""MinimumBrowserVersion"": null
                    }
                ]
            }";
        }

        private static string GetJSONWithOldBrowsersSupportedSchema()
        {
            return @"
            {
                ""BrowsersSupported"": [
                    ""Google Chrome"",
                    ""Chromium"",
                    ""Internet Explorer 11"",
                    ""Internet Explorer 10""
                ]
            }";
        }

        private static string GetJSON()
        {
            return @"
            {
                ""ClientApplicationTypes"": [
                    ""browser-based"",
                    ""native-mobile"",
                    ""native-desktop""
                ],
                ""BrowsersSupported"": [
                    {
                        ""BrowserName"": ""Google Chrome"",
                        ""MinimumBrowserVersion"": null
                    },
                    {
                        ""BrowserName"": ""Chromium"",
                        ""MinimumBrowserVersion"": null
                    },
                    {
                        ""BrowserName"": ""Internet Explorer 11"",
                        ""MinimumBrowserVersion"": null
                    },
                    {
                        ""BrowserName"": ""Internet Explorer 10"",
                        ""MinimumBrowserVersion"": null
                    }
                ],
                ""MobileResponsive"": true,
                ""Plugins"": {
                    ""Required"": false,
                    ""AdditionalInformation"": """"
                },
                ""MinimumConnectionSpeed"": ""2Mbps"",
                ""MinimumDesktopResolution"": ""16:9 – 1366 x 768"",
                ""HardwareRequirements"": ""The browser activities are only supported in relation to the native desktop client and therefore mirror the native desktop client hardware requirements detailed below."",
                ""NativeMobileHardwareRequirements"": ""Any device capable of supporting the listed supported operating systems is compliant."",
                ""NativeDesktopHardwareRequirements"": ""The spoke server is an important part of the solution. It provides a patch distribution system for client updates and acts as a local cache. \r\n\r\nEMIS Health recommends that your spoke is a dedicated device. However, if you use your spoke to perform other functions, such as act as a domain controller, store business documents or host other applications, then a Windows server class operating system will be required, along with an appropriate specification of server hardware."",
                ""AdditionalInformation"": """",
                ""MobileFirstDesign"": false,
                ""NativeMobileFirstDesign"": false,
                ""MobileOperatingSystems"": {
                    ""OperatingSystems"": [
                        ""Apple IOS"",
                        ""Android"",
                        ""Other""
                    ],
                    ""OperatingSystemsDescription"": ""•\tiOS v 10.3.3.3 and above\r\n\r\n•\tAndroid v 6 and above\r\n\r\n•\tWindows 10 (Build 14393)""
                },
                ""MobileConnectionDetails"": {
                    ""MinimumConnectionSpeed"": """",
                    ""ConnectionType"": [
                        ""GPRS"",
                        ""3G"",
                        ""LTE"",
                        ""4G"",
                        ""5G"",
                        ""Wifi""
                    ],
                    ""Description"": ""The mobile application only requires internet connectivity to synchronise, therefore there is no minimum connection speed required.""
                },
                ""MobileMemoryAndStorage"": {
                    ""MinimumMemoryRequirement"": ""2GB"",
                    ""Description"": ""All compliant devices must have a minimum 16GB storage.""
                },
                ""MobileThirdParty"": {
                    ""ThirdPartyComponents"": """",
                    ""DeviceCapabilities"": ""The device should have access to the relevant App Store to enable the installation of the respective application although deployment via mobile device management solutions is supported.""
                },
                ""NativeMobileAdditionalInformation"": ""Apple have recently announced that a new operating system, designed specifically for iPad devices.\r\n\r\nWe have tested this and can confirm that EMIS Mobile is fully compatible."",
                ""NativeDesktopOperatingSystemsDescription"": ""Microsoft Windows 7 (x86 x64)\r\n\r\nMicrosoft Windows 8.1 (x86 x64)\r\n\r\nMicrosoft Windows 10 (x86 x64)"",
                ""NativeDesktopMinimumConnectionSpeed"": ""2Mbps"",
                ""NativeDesktopThirdParty"": {
                    ""ThirdPartyComponents"": "".NET framework 4."",
                    ""DeviceCapabilities"": ""The application requires connectivity to the EMIS Data Centre.""
                },
                ""NativeDesktopMemoryAndStorage"": {
                    ""MinimumMemoryRequirement"": ""4GB"",
                    ""StorageRequirementsDescription"": ""10GB free disk space."",
                    ""MinimumCpu"": ""Intel Core i3 equivalent or higher."",
                    ""RecommendedResolution"": ""16:9 – 1366 x 768""
                },
                ""NativeDesktopAdditionalInformation"": ""The minimum connection speed is dependent on the number of clients that need to be supported.\r\n\r\nEMIS Health do not support the use of on-screen keyboards for 2 in 1 devices.""
            }";
        }

        private static CatalogueItem GetCatalogueItem()
        {
            return new CatalogueItem
            {
                Id = new CatalogueItemId(12345, "123"),
                SupplierId = 99999,
                CatalogueItemType = CatalogueItemType.Solution,
                Created = DateTime.UtcNow,
                Name = "Solution Test",
                Solution = new Solution
                {
                    CatalogueItemId = new CatalogueItemId(12345, "123"),
                    AboutUrl = "https://test.com",
                    Summary = "SUMMARY - DFOCVC Solution",
                    Features = @"[""Digital Online Consultation"",""Video Consultation"", ""Fully interoperable with all major GP IT solutions"", ""Compliant with all relevant ISO standards""]",
                    Hosting = null,
                    ApplicationTypeDetail = null,
                    LastUpdated = DateTime.UtcNow,
                    FullDescription = "FULL DESCRIPTION – Digital First, Online Consultation and Video Consultation Solution.",
                    ImplementationDetail = "Some implementation detail",
                    MarketingContacts = new List<MarketingContact>
                        {
                            new()
                            {
                                FirstName = "Bob",
                                LastName = "Smith",
                                Email = "test@test.com",
                                Department = "Fruit controller",
                                LastUpdated = DateTime.UtcNow,
                            },
                        },
                    Integrations = null,
                },
                CatalogueItemContacts = new List<SupplierContact>
                    {
                        new()
                        {
                            SupplierId = 99999,
                            FirstName = "Jan",
                            LastName = "Bob",
                            Email = "test@test.com",
                            Department = "Soaps and Socks",
                            LastUpdated = DateTime.UtcNow,
                        },
                    },
                CatalogueItemCapabilities = new List<CatalogueItemCapability>
                    {
                        new() { CapabilityId = 43, LastUpdated = DateTime.UtcNow, StatusId = 1 },
                    },
                CatalogueItemEpics = new List<CatalogueItemEpic>
                    {
                        new() { CapabilityId = 43, EpicId = "E00001", StatusId = 1 },
                    },
                PublishedStatus = PublicationStatus.Published,
            };
        }

        private void SaveAndUpdateApplicationTypeJson(CatalogueItem catalogueItem, string json = null)
        {
            using var dbContext = fixture.GetDbContext();
            dbContext.Add(catalogueItem);
            dbContext.SaveChanges();
            if (json != null)
            {
                dbContext.Database.ExecuteSql($"UPDATE [Catalogue].[Solutions] SET [ClientApplication] = {json} WHERE CatalogueItemId = {catalogueItem.Id.ToString()}");
            }
        }
    }
}
