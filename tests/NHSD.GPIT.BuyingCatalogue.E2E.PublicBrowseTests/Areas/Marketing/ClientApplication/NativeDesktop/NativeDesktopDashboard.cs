using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class NativeDesktopDashboard : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public NativeDesktopDashboard(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop")
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == new CatalogueItemId(99999, "99"));
            solution.ClientApplication = null;
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

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
