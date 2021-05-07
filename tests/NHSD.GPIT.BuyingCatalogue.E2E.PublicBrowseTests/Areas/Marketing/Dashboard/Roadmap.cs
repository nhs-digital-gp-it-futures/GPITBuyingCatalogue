using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class Roadmap : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Roadmap(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/roadmap")
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.RoadMap = null;
            context.SaveChanges();
        }

        [Fact]
        public async Task Roadmap_CompleteSummary()
        {
            var summary = MarketingPages.RoadmapActions.EnterSummary(1000);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");

            solution.RoadMap.Should().Be(summary);
        }

        [Fact]
        public void Roadmap_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();
            MarketingPages.DashboardActions.SectionMarkedComplete("Roadmap").Should().BeFalse();
        }

        [Fact]
        public void Roadmap_SectionComplete()
        {
            MarketingPages.RoadmapActions.EnterSummary(1000);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Roadmap").Should().BeTrue();
        }
    }
}
