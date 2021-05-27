using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class NativeMobileDashboard : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public NativeMobileDashboard(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile")
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.ClientApplication = null;
            context.SaveChanges();
        }

        [Theory]
        [InlineData("Supported operating systems")]
        [InlineData("Mobile first approach")]
        [InlineData("Connectivity")]
        [InlineData("Memory and storage")]
        [InlineData("Third-party components and device capabilities")]
        [InlineData("Hardware requirements")]
        [InlineData("Additional information")]
        public void NativeMobileDashboard_SectionsDisplayed(string section)
        {
            MarketingPages.DashboardActions.SectionDisplayed(section).Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
