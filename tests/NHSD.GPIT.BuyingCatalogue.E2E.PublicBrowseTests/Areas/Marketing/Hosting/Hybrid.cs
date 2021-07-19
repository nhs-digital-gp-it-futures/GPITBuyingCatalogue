using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class Hybrid : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Hybrid(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-hybrid")
        {
            AuthorityLogin();
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

            await using var context = GetEndToEndDbContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"))).Hosting;

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
            ClearHostingTypes(new CatalogueItemId(99999, "99"));
        }
    }
}
