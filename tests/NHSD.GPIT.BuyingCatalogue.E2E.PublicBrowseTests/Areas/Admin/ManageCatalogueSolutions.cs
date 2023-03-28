using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin
{
    [Collection(nameof(AdminCollection))]
    public sealed class ManageCatalogueSolutions : AuthorityTestBase
    {
        public ManageCatalogueSolutions(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.Index),
                  null)
        {
        }

        [Fact]
        public void ManageCatalogueSolutions_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.AddSolutionLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchButton).Should().BeTrue();
            AdminPages.AddSolution.NumberOfFilterRadioButtonsDisplayed().Should().Be(5);
            AdminPages.AddSolution.CatalogueSolutionTableDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessage).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessageLink).Should().BeFalse();
        }

        [Fact]
        public void ManageCatalogueSolutions_ClickAddOrganisationLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CatalogueSolutionObjects.AddSolutionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AddCatalogueSolutionController),
                nameof(AddCatalogueSolutionController.Index)).Should().BeTrue();
        }

        [Fact]
        public async Task ManageCatalogueSolutions_SearchTermEmpty_AllSolutionsDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();

                var solutions = await context.CatalogueItems
                    .Include(x => x.Supplier)
                    .Where(x => x.CatalogueItemType == CatalogueItemType.Solution)
                    .ToListAsync();

                CommonActions.ElementAddValue(CatalogueSolutionObjects.SearchBar, string.Empty);
                CommonActions.ClickLinkElement(CatalogueSolutionObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.Index)).Should().BeTrue();

                var pageSummary = GetPageSummary();

                pageSummary.SolutionIds.Should().BeEquivalentTo(solutions.Select(x => $"{x.Id}"));
                pageSummary.SolutionNames.Should().BeEquivalentTo(solutions.Select(x => x.Name.Trim()));
                pageSummary.SupplierNames.Should().BeEquivalentTo(solutions.Select(x => x.Supplier.Name.Trim()));
            });
        }

        [Fact]
        public async Task ManageCatalogueSolutions_SearchTermValid_FilteredEpicsDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();

                var sampleSolution = context.CatalogueItems
                    .Include(x => x.Supplier)
                    .Where(x => x.CatalogueItemType == CatalogueItemType.Solution)
                    .OrderByDescending(x => x.Name.Length)
                    .First();

                await CommonActions.InputCharactersWithDelay(CatalogueSolutionObjects.SearchBar, sampleSolution.Name);
                CommonActions.WaitUntilElementIsDisplayed(CatalogueSolutionObjects.SearchListBox);

                CommonActions.ElementExists(CatalogueSolutionObjects.SearchResult(0)).Should().BeTrue();
                CommonActions.ElementTextEqualTo(
                    CatalogueSolutionObjects.SearchResultTitle(0),
                    sampleSolution.Name).Should().BeTrue();
                CommonActions.ElementTextEqualTo(
                    CatalogueSolutionObjects.SearchResultDescription(0),
                    sampleSolution.Supplier.Name).Should().BeTrue();

                CommonActions.ClickLinkElement(CatalogueSolutionObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.Index)).Should().BeTrue();

                var pageSummary = GetPageSummary();

                pageSummary.SolutionIds.First().Should().Be($"{sampleSolution.Id}");
                pageSummary.SolutionNames.First().Should().Be(sampleSolution.Name.Trim());
                pageSummary.SupplierNames.First().Should().Be(sampleSolution.Supplier.Name.Trim());
            });
        }

        [Fact]
        public async Task ManageCatalogueSolutions_SearchTermValid_NoMatches_ErrorMessageDisplayed()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await CommonActions.InputCharactersWithDelay(CatalogueSolutionObjects.SearchBar, Strings.RandomString(10));
                CommonActions.WaitUntilElementIsDisplayed(CatalogueSolutionObjects.SearchListBox);

                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchResultsErrorMessage).Should().BeTrue();

                CommonActions.ClickLinkElement(CatalogueSolutionObjects.SearchButton);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.Index)).Should().BeTrue();

                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.AddSolutionLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchButton).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessage).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessageLink).Should().BeTrue();

                var pageSummary = GetPageSummary();

                pageSummary.SolutionIds.Should().BeEmpty();
                pageSummary.SolutionNames.Should().BeEmpty();
                pageSummary.SupplierNames.Should().BeEmpty();

                CommonActions.ClickLinkElement(CatalogueSolutionObjects.SearchErrorMessageLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.Index)).Should().BeTrue();

                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.AddSolutionLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchButton).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessage).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessageLink).Should().BeFalse();
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task ManageCatalogueSolutions_FilteredCatalogueSolutionsDisplayedAsync(int index)
        {
            var publicationStatus = AdminPages.AddSolution.FilterCatalogueSolutions(index);

            await using var context = GetEndToEndDbContext();

            var dbSolutions = await context.CatalogueItems
                .Where(c => c.PublishedStatus == publicationStatus)
                .Where(c => c.CatalogueItemType == CatalogueItemType.Solution)
                .ToListAsync();

            if (dbSolutions.Count > 0)
            {
                AdminPages.AddSolution.GetNumberOfItemsInTable().Should().Be(dbSolutions.Count);
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessage).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessageLink).Should().BeFalse();
            }
            else
            {
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessage).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CatalogueSolutionObjects.SearchErrorMessageLink).Should().BeTrue();
            }
        }

        private PageSummary GetPageSummary() => new()
        {
            SolutionIds = Driver.FindElements(CatalogueSolutionObjects.SolutionIds).Select(s => s.Text.Trim()).ToList(),
            SolutionNames = Driver.FindElements(CatalogueSolutionObjects.SolutionNames).Select(s => s.Text.Trim()).ToList(),
            SupplierNames = Driver.FindElements(CatalogueSolutionObjects.SupplierNames).Select(s => s.Text.Trim()).ToList(),
        };

        private class PageSummary
        {
            public IEnumerable<string> SolutionNames { get; init; }

            public IEnumerable<string> SolutionIds { get; init; }

            public IEnumerable<string> SupplierNames { get; init; }
        }
    }
}
