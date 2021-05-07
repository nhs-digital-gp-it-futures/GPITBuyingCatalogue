using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class BrowserBasedDashboard : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public BrowserBasedDashboard(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based")
        {
            ClearClientApplication();

            driver.Navigate().Refresh();
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
    }
}
