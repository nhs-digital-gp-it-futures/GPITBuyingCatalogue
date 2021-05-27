using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class Hybrid : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {

        public Hybrid(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-hybrid")
        {
        }

        [Fact]
        public async Task Hybrid_CompleteAllFields()
        {
            var summary = TextGenerators.TextInputAddText(HostingTypesObjects.HybridHostingType_Summary, 500);
            var link = TextGenerators.UrlInputAddText(HostingTypesObjects.HybridHostingType_Link, 1000);
            var hostingModel = TextGenerators.TextInputAddText(HostingTypesObjects.HybridHostingType_HostingModel, 1000);
            var expected = new HybridHostingType
            {
                HostingModel = hostingModel,
                Link = link,
                Summary = summary,
            };

            MarketingPages.HostingTypeActions.ToggleHSCNCheckbox();

            CommonActions.ClickSave();

            using var context = GetBCContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).Hosting;
            
            var actual = JsonConvert.DeserializeObject<ServiceContracts.Solutions.Hosting>(hosting);
            actual.HybridHostingType.Should().BeEquivalentTo(expected, opt => opt.Excluding(h => h.RequiresHscn));
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

        public void Dispose()
        {
            ClearHostingTypes("99999-99");
        }
    }
}
