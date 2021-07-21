using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class SolutionDetails : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SolutionDetails(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/99999-001")
        {
        }

        [Theory]
        [InlineData("solution name")]
        [InlineData("solution id")]
        [InlineData("supplier name")]
        [InlineData("foundation solution")]
        [InlineData("framework")]
        public void SolutionDetails_AllFieldsDisplayed(string rowHeader)
        {
            PublicBrowsePages.SolutionAction.GetTableRowContent(rowHeader).Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task SolutionDetails_VerifySummary()
        {
            await using var context = GetEndToEndDbContext();
            var summary = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "001"))).Summary;

            var summaryAndDescription = PublicBrowsePages.SolutionAction.GetSummaryAndDescriptions();

            summaryAndDescription
                .Any(s => s.Contains(summary, StringComparison.CurrentCultureIgnoreCase))
                .Should().BeTrue();
        }

        [Fact]
        public async Task SolutionDetails_VerifyDescription()
        {
            await using var context = GetEndToEndDbContext();
            var description = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "001"))).FullDescription;

            var summaryAndDescription = PublicBrowsePages.SolutionAction.GetSummaryAndDescriptions();

            summaryAndDescription
                .Any(s => s.Contains(description, StringComparison.CurrentCultureIgnoreCase))
                .Should().BeTrue();
        }

        [Fact]
        public void SolutionDetails_Breadcrumbs_BreadcrumbBannerDisplayed()
        {
            CommonActions.ElementIsDisplayed(SolutionObjects.BreadcrumbsBanner).Should().BeTrue();
        }

        [Theory]
        [InlineData("Home")]
        [InlineData("Catalogue Solutions")]
        public void SolutionDetails_Breadcrumbs_BreadcrumbItemsDisplayed(string breadcrumbItem)
        {
            PublicBrowsePages.CommonActions.GetBreadcrumbNames(breadcrumbItem).Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void SolutionDetails_Breadcrumbs_CatalogueSolutionPageDisplayed()
        {
            CommonActions.ClickLinkElement(SolutionObjects.CatalogueSolutionCrumb);
            PublicBrowsePages.SolutionAction.CatalogueSolutionPageDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SolutionDetails_Breadcrumbs_HomePageDisplayed()
        {
            CommonActions.ClickLinkElement(HomepageObjects.HomePageCrumb);
            PublicBrowsePages.HomePageActions.HomePageDisplayed().Should().BeTrue();
        }
    }
}
