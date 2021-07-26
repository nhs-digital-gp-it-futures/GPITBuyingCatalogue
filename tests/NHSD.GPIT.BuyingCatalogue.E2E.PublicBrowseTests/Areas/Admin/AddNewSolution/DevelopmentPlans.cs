using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution
{
    public sealed class DevelopmentPlans : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public DevelopmentPlans(LocalWebApplicationFactory factory)
            : base(factory, "/admin/catalogue-solutions/manage/99999-002/development-plans")
        {
            ClearRoadMap(new CatalogueItemId(99999, "003"));
            AuthorityLogin();
        }

        [Fact]
        public async Task DevelopmentPlans_TitleDisplayedCorrectly()
        {
            await using var context = GetEndToEndDbContext();
            var solutionName = (await context.CatalogueItems.SingleAsync(s => s.CatalogueItemId == new CatalogueItemId(99999, "002"))).Name;

            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Development plans - {solutionName}".FormatForComparison());
        }

        [Fact]
        public async Task DevelopmentPlans_EnterRoadMapLinkAsync()
        {
            var link = TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"));
            solution.RoadMap.Should().Be(link);
        }

        [Fact]
        public async Task DevelopmentPlans_GoBackWithoutSaving()
        {
            TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            AdminPages.CommonActions.ClickGoBack();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"));
            var roadmapUrl = solution.RoadMap;

            roadmapUrl.Should().BeNullOrEmpty();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Single(s => s.Id == new CatalogueItemId(99999, "002"));
            solution.RoadMap = null;
            context.SaveChanges();
        }
    }
}
