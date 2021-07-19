using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class Roadmap : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Roadmap(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/roadmap")
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == new CatalogueItemId(99999, "99"));
            solution.RoadMap = null;
            context.SaveChanges();

            AuthorityLogin();
        }

        [Fact]
        public async Task Roadmap_CompleteSummary()
        {
            var summary = TextGenerators.TextInputAddText(CommonSelectors.Summary, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"));

            solution.RoadMap.Should().Be(summary);
        }

        [Fact]
        public void Roadmap_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();
            MarketingPages.DashboardActions.SectionMarkedComplete("Roadmap").Should().BeFalse();
        }

        [Fact]
        public void Roadmap_SectionComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Summary, 1000);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Roadmap").Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
