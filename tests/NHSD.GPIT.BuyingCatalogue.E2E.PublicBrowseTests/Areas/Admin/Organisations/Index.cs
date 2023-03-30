using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Organisations
{
    [Collection(nameof(AdminCollection))]
    public sealed class Index : AuthorityTestBase
    {
        public Index(LocalWebApplicationFactory factory)
            : base(factory, typeof(OrganisationsController), nameof(OrganisationsController.Index), null)
        {
        }

        [Fact]
        public void Index_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(BreadcrumbObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.AddOrganisationLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.ImportPracticeListsButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.SearchButton).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.OrganisationsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrganisationObjects.SearchErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(OrganisationObjects.SearchErrorMessageLink).Should().BeFalse();
        }

        [Fact]
        public async Task Index_AllOrganisationsDisplayed()
        {
            await VerifyAllOrganisationsDisplayed();
        }

        [Fact]
        public void Index_ClickHomeBreadcrumbLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(BreadcrumbObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void Index_ClickAddOrganisationLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(OrganisationObjects.AddOrganisationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrganisationsController),
                nameof(OrganisationsController.Find)).Should().BeTrue();
        }

        [Fact]
        public void Index_ClickImportPracticeLists_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(OrganisationObjects.ImportPracticeListsButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImportController),
                nameof(ImportController.ImportGpPracticeList)).Should().BeTrue();
        }

        [Fact]
        public async Task Index_SearchTermEmpty_AllOrganisationsDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                CommonActions.ElementAddValue(OrganisationObjects.SearchBar, string.Empty);
                CommonActions.ClickLinkElement(OrganisationObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrganisationsController),
                    nameof(OrganisationsController.Index)).Should().BeTrue();

                await VerifyAllOrganisationsDisplayed();
            });
        }

        [Fact]
        public async Task Index_SearchTermValid_FilteredOrganisationsDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();

                var sampleOrganisation = context.Organisations.First();

                await CommonActions.InputCharactersWithDelay(OrganisationObjects.SearchBar, sampleOrganisation.ExternalIdentifier);
                CommonActions.WaitUntilElementIsDisplayed(OrganisationObjects.SearchListBox);

                CommonActions.ElementExists(OrganisationObjects.SearchResult(0)).Should().BeTrue();
                CommonActions.ElementTextEqualTo(OrganisationObjects.SearchResultTitle(0), sampleOrganisation.Name).Should().BeTrue();
                CommonActions.ElementTextEqualTo(OrganisationObjects.SearchResultDescription(0), sampleOrganisation.ExternalIdentifier).Should().BeTrue();

                CommonActions.ClickLinkElement(OrganisationObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrganisationsController),
                    nameof(OrganisationsController.Index)).Should().BeTrue();

                var pageSummary = GetPageSummary();

                pageSummary.OdsCodes.First().Should().Be(sampleOrganisation.ExternalIdentifier);
                pageSummary.OrganisationIds.First().Should().Be(sampleOrganisation.Id);
                pageSummary.OrganisationNames.First().Should().Be(sampleOrganisation.Name);
            });
        }

        [Fact]
        public async Task Index_SearchTermValid_NoMatches_ErrorMessageDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await CommonActions.InputCharactersWithDelay(OrganisationObjects.SearchBar, Strings.RandomString(10));
                CommonActions.WaitUntilElementIsDisplayed(OrganisationObjects.SearchListBox);

                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchResultsErrorMessage).Should().BeTrue();

                CommonActions.ClickLinkElement(OrganisationObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrganisationsController),
                    nameof(OrganisationsController.Index)).Should().BeTrue();

                CommonActions.ElementIsDisplayed(OrganisationObjects.AddOrganisationLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchButton).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.OrganisationsTable).Should().BeFalse();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchErrorMessage).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchErrorMessageLink).Should().BeTrue();

                var pageSummary = GetPageSummary();

                pageSummary.OdsCodes.Should().BeEmpty();
                pageSummary.OrganisationIds.Should().BeEmpty();
                pageSummary.OrganisationNames.Should().BeEmpty();

                CommonActions.ClickLinkElement(OrganisationObjects.SearchErrorMessageLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrganisationsController),
                    nameof(OrganisationsController.Index)).Should().BeTrue();

                CommonActions.ElementIsDisplayed(OrganisationObjects.AddOrganisationLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchButton).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.OrganisationsTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchErrorMessage).Should().BeFalse();
                CommonActions.ElementIsDisplayed(OrganisationObjects.SearchErrorMessageLink).Should().BeFalse();
            });
        }

        private async Task VerifyAllOrganisationsDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var organisations = await context.Organisations.Select(o => new
            {
                o.Name,
                o.ExternalIdentifier,
                OrganisationId = o.Id,
            }).ToListAsync();

            var pageSummary = GetPageSummary();

            pageSummary.OdsCodes.Should().BeEquivalentTo(organisations.Select(o => o.ExternalIdentifier)).And.HaveCount(organisations.Count);
            pageSummary.OrganisationIds.Should().BeEquivalentTo(organisations.Select(o => o.OrganisationId)).And.HaveCount(organisations.Count);
            pageSummary.OrganisationNames.Should().BeEquivalentTo(organisations.Select(o => o.Name)).And.HaveCount(organisations.Count);
        }

        private PageSummary GetPageSummary() => new()
        {
            OdsCodes = Driver.FindElements(OrganisationObjects.OdsCodes).Select(x => x.Text.Trim()),
            OrganisationIds = Driver.FindElements(OrganisationObjects.OrganisationLinks).Select(s => int.Parse(s.GetAttribute("org-id").Trim())),
            OrganisationNames = Driver.FindElements(OrganisationObjects.OrganisationNames).Select(x => x.Text.Trim()),
        };

        private class PageSummary
        {
            public IEnumerable<string> OdsCodes { get; init; }

            public IEnumerable<string> OrganisationNames { get; init; }

            public IEnumerable<int> OrganisationIds { get; init; }
        }
    }
}
