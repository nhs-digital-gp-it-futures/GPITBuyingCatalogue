using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class SupportedOperatingSystems : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public SupportedOperatingSystems(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/operating-systems")
        {
        }

        [Fact]
        public async Task SupportedOperatingSystems_CompleteAllFields()
        {
            var operatingSystems = TextGenerators.TextInputAddText(CommonSelectors.SupportedOperatingSystemDescription, 1000);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"\"NativeDesktopOperatingSystemsDescription\":\"{operatingSystems}\"");
        }

        [Fact]
        public void SupportedOperatingSystems_SectionComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.SupportedOperatingSystemDescription, 1000);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Supported operating systems").Should().BeTrue();
        }

        [Fact]
        public void SupportedOperatingSystems_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Supported operating systems").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
