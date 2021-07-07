using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class AboutSolution : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public AboutSolution(LocalWebApplicationFactory factory)
            : base(factory, "/marketing/supplier/solution/99999-99/section/solution-description")
        {
        }

        [Fact]
        public async Task AboutSolution_EditAllFieldsAsync()
        {
            var summary = TextGenerators.TextInputAddText(CommonSelectors.Summary, 350);
            var description = TextGenerators.TextInputAddText(CommonSelectors.Description, 1100);
            var link = TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"));
            solution.Summary.Should().Be(summary);
            solution.FullDescription.Should().Be(description);
            solution.AboutUrl.Should().Be(link);
        }

        [Fact]
        public void AboutSolution_SectionMarkedAsComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Summary, 350);
            TextGenerators.TextInputAddText(CommonSelectors.Description, 1100);
            TextGenerators.UrlInputAddText(CommonSelectors.Link, 1000);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Solution description").Should().BeTrue();
        }

        [Fact]
        public async Task AboutSolution_SectionMarkedAsIncompleteAsync()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"));
            solution.Summary = string.Empty;
            solution.FullDescription = string.Empty;
            solution.AboutUrl = string.Empty;

            await context.SaveChangesAsync();

            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Solution description").Should().BeFalse();
        }

        [Fact]
        public async Task AboutSolution_SummaryLeftEmpty()
        {
            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"));
            solution.Summary = string.Empty;

            await context.SaveChangesAsync();
            Driver.Navigate().Refresh();

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
