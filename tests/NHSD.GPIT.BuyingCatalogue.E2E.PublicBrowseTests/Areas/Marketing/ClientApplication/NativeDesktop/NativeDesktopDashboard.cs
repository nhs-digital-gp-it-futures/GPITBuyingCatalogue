using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class NativeDesktopDashboard : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public NativeDesktopDashboard(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop")
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.ClientApplication = @"{
                        ""ClientApplicationTypes"": [
                            ""native-desktop""
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

        [Theory]
        [InlineData("Supported operating systems")]
        [InlineData("Connectivity")]
        [InlineData("Memory, storage, processing and resolution")]
        [InlineData("Third-party components and device capabilities")]
        [InlineData("Hardware requirements")]
        [InlineData("Additional information")]
        public void NativeDesktopDashboard_SectionsDisplayed(string section)
        {
            MarketingPages.DashboardActions.SectionDisplayed(section).Should().BeTrue();
        }
    }
}
