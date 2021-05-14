using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class SupportedOperatingSystems : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SupportedOperatingSystems(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/operating-systems")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task SupportedOperatingSystems_CompleteAllFields()
        {
            MarketingPages.CommonActions.ClickFirstCheckbox();

            var description = MarketingPages.AboutSupplierActions.DescriptionAddText(1000);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;

            clientApplication.Should().ContainEquivalentOf($"\"OperatingSystems\":[\"Apple IOS\"],\"OperatingSystemsDescription\":\"{description}\"");
        }

        [Fact]
        public void SupportedOperatingSystems_SectionComplete()
        {
            MarketingPages.CommonActions.ClickFirstCheckbox();

            MarketingPages.AboutSupplierActions.DescriptionAddText(1000);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Supported operating systems").Should().BeTrue();
        }

        [Fact]
        public void SupportedOperatingSystems_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Supported operating systems").Should().BeFalse();
        }
    }
}
