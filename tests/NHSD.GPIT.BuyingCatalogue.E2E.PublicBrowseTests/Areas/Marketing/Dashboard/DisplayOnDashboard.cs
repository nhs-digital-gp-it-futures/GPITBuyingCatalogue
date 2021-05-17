using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class DisplayOnDashboard : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public DisplayOnDashboard(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/")
        {
        }

        [Theory]
        [InlineData("Solution description")]
        [InlineData("Features")]
        [InlineData("Client application type")]
        [InlineData("Public cloud")]
        [InlineData("Private cloud")]
        [InlineData("Hybrid")]
        [InlineData("On premise")]
        [InlineData("Contact details")]
        [InlineData("Roadmap")]
        [InlineData("About supplier")]
        [InlineData("Integrations")]
        [InlineData("Implementation timescales")]
        public void MarketingPages_DisplayOnDashboard(string section)
        {
            MarketingPages.DashboardActions.SectionDisplayed(section).Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
