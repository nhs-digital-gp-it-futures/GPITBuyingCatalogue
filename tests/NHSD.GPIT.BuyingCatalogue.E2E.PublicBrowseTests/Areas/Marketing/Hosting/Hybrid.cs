using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class Hybrid : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Hybrid(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-hybrid")
        {
            ClearHostingTypes("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task Hybrid_CompleteAllFields()
        {
            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.HybridHostingType_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.HybridHostingType_Link, 1000);
            var hostingModel = TextGenerators.TextInputAddText(HostingTypesObjects.HybridHostingType_HostingModel, 1000);

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            using var context = GetBCContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).Hosting;

            hosting.Should().ContainEquivalentOf($"\"HybridHostingType\":{{\"Summary\":\"{summary}\",\"Link\":\"{link}\",\"HostingModel\":\"{hostingModel}\"");
        }

        [Fact]
        public void Hybrid_SectionComplete()
        {
            TextGenerators.TextInputAddText(HostingTypesObjects.HybridHostingType_Summary, 500);
            TextGenerators.UrlInputAddText(HostingTypesObjects.HybridHostingType_Link, 1000);
            TextGenerators.TextInputAddText(HostingTypesObjects.HybridHostingType_HostingModel, 1000);

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hybrid").Should().BeTrue();
        }

        [Fact]
        public void Hybrid_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hybrid").Should().BeFalse();
        }
    }
}
