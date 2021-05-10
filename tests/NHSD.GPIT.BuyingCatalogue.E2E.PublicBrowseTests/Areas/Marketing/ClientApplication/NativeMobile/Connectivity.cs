using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class Connectivity : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Connectivity(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/connectivity")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task Connectivity_CompleteAllFields()
        {
            MarketingPages.ClientApplicationTypeActions.SelectConnectionSpeedDropdown(1);

            MarketingPages.CommonActions.ClickFirstCheckbox();

            var description = MarketingPages.AboutSupplierActions.DescriptionAddText(200);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;

            clientApplication.Should().ContainEquivalentOf("ConnectionType\":[\"GPRS\"]");
            clientApplication.Should().ContainEquivalentOf("MinimumConnectionSpeed\":\"0.5Mbps\"");
            clientApplication.Should().ContainEquivalentOf($"Description\":\"{description}\"");
        }

        [Fact]
        public void Connectivity_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.SelectConnectionSpeedDropdown(1);

            MarketingPages.CommonActions.ClickFirstCheckbox();

            MarketingPages.AboutSupplierActions.DescriptionAddText(200);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity").Should().BeTrue();
        }

        [Fact]
        public void Connectivity_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity").Should().BeFalse();
        }
    }
}
