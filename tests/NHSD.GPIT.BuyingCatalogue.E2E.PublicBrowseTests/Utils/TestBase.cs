using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Net.Http;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils
{
    public abstract class TestBase
    {
        private readonly Uri uri;

        public TestBase(LocalWebApplicationFactory factory, string urlArea = "")
        {
            client = factory.CreateClient();
            this.factory = factory;

            driver = this.factory.Driver;
            PublicBrowsePages = new PublicBrowsePages(driver).PageActions;
            MarketingPages = new MarketingPageActions(driver).PageActions;

            driver = this.factory.Driver;
            uri = new Uri(factory.RootUri);
            var combinedUri = new Uri(uri, urlArea);
            driver.Navigate().GoToUrl(combinedUri);
        }

        internal BuyingCatalogueDbContext GetBCContext()
        {
            var options = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                .UseInMemoryDatabase(factory.DbName)
                .Options;

            return new(options);
        }

        internal void ClearClientApplication()
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.ClientApplication = @"{
                        ""ClientApplicationTypes"": [
                            ""browser-based""
                        ],
                        ""BrowsersSupported"": [],
                        ""MobileResponsive"": null,
                        ""Plugins"": null,
                        ""HardwareRequirements"": null,
                        ""NativeMobileHardwareRequirements"": null,
                        ""NativeDesktopHardwareRequirements"": null,
                        ""AdditionalInformation"": null,
                        ""MinimumConnectionSpeed"": null,
                        ""MinimumDesktopResolution"": null,
                        ""MobileFirstDesign"": null,
                        ""NativeMobileFirstDesign"": null,
                        ""MobileOperatingSystems"": null,
                        ""MobileConnectionDetails"": null,
                        ""MobileMemoryAndStorage"": null,
                        ""MobileThirdParty"": {
                            ""ThirdPartyComponents"": null,
                            ""DeviceCapabilities"": null
                        },
                        ""NativeMobileAdditionalInformation"": null,
                        ""NativeDesktopOperatingSystemsDescription"": null,
                        ""NativeDesktopMinimumConnectionSpeed"": null,
                        ""NativeDesktopThirdParty"": null,
                        ""NativeDesktopMemoryAndStorage"": null,
                        ""NativeDesktopAdditionalInformation"": null
                    }";
            context.SaveChanges();
        }

        private readonly HttpClient client;
        protected readonly LocalWebApplicationFactory factory;
        protected readonly IWebDriver driver;

        internal Actions.PublicBrowse.ActionCollection PublicBrowsePages { get; }
        internal Actions.Marketing.ActionCollection MarketingPages { get; }
    }
}
