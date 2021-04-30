using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class PlugInsOrExtensions : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public PlugInsOrExtensions(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/plug-ins-or-extensions")
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

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task PlugInsOrExtensions_SelectRadioButton(string label)
        {
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText(label);
            var additional = MarketingPages.ClientApplicationTypeActions.EnterAdditionalInformation(1000);

            MarketingPages.CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"Plugins"":{{""Required"":{labelConvert},""AdditionalInformation"":""{additional}""}}");
        }

        [Fact]
        public void PlugInsOrExtensions_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText("Yes");
            MarketingPages.ClientApplicationTypeActions.EnterAdditionalInformation(1000);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Plug-ins or extensions required").Should().BeTrue();
        }

        [Fact]
        public void PlugInsOrExtensions_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Plug-ins or extensions required").Should().BeFalse();
        }
    }
}
