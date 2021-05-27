using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class BrowserBasedDashboard : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public BrowserBasedDashboard(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based")
        {
        }

        [Theory]
        [InlineData("Supported browsers")]
        [InlineData("Mobile first approach")]
        [InlineData("Plug-ins or extensions required")]
        [InlineData("Connectivity and resolution")]
        [InlineData("Hardware requirements")]
        [InlineData("Additional information")]
        public void BrowserBasedDashboard_SectionsDisplayed(string section)
        {
            MarketingPages.DashboardActions.SectionDisplayed(section).Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
