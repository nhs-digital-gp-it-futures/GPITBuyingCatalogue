using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class ConnectivityAndResolution : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ConnectivityAndResolution(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/connectivity-and-resolution")
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

            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task ConnectivityAndResolution_SelectBothFields()
        {
            MarketingPages.ClientApplicationTypeActions.SelectConnectionSpeedDropdown(1);
            MarketingPages.ClientApplicationTypeActions.SelectResolutionDropdown(1);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"""MinimumConnectionSpeed"":""0.5Mbps""");
            clientApplication.Should().ContainEquivalentOf(@$"""MinimumDesktopResolution"":""16:9 - 640 x 360""");
        }

        [Fact]
        public void ConnectivityAndResolution_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.SelectConnectionSpeedDropdown(1);
            MarketingPages.ClientApplicationTypeActions.SelectResolutionDropdown(1);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity and resolution").Should().BeTrue();
        }

        [Fact]
        public void ConnectivityAndResolution_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity and resolution").Should().BeFalse();
        }
    }
}
