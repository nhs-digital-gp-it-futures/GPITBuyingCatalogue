using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class CatalogueSolutions
        : AnonymousTestBase
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public CatalogueSolutions(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                 factory,
                 typeof(SolutionsController),
                 nameof(SolutionsController.Index),
                 Parameters,
                 testOutputHelper)
        {
        }

        [Fact]
        public void CatalogueSolutions_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(SolutionsObjects.SortBySelect).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.Header3).Should().BeTrue();
                CommonActions.ElementExists(CommonSelectors.Pagination).Should().BeTrue();
                CommonActions.ElementIsDisplayed(SolutionsObjects.FilterCatalogueSolutionsLink).Should().BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_SortByLastPublished_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.SelectDropDownItemByValue(SolutionsObjects.SortBySelect, PageOptions.SortOptions.LastPublished.ToString());

                CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index))
                    .Should()
                    .BeTrue();

                Driver.Url.Should().Contain($"sortBy={PageOptions.SortOptions.LastPublished}");
            });
        }

        [Fact]
        public void CatalogueSolutions_Pagination_CorrectNumberOfPages()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();

                using var context = GetEndToEndDbContext();
                var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

                var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

                CommonActions.ElementTextEqualTo(CommonSelectors.PaginationNextSubText, $"2 of {expectedNumberOfPages}")
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_Pagination_ClickNext_ExpectedResults()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

                CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index))
                    .Should()
                    .BeTrue();

                Driver.Url.Should().EndWith("page=2");

                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();

                using var context = GetEndToEndDbContext();
                var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

                var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

                CommonActions.ElementTextEqualTo(CommonSelectors.PaginationNextSubText, $"3 of {expectedNumberOfPages}")
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(CommonSelectors.PaginationPreviousSubText, $"1 of {expectedNumberOfPages}")
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_Pagination_LastPage_ExpectedResults()
        {
            RunTest(() =>
            {
                using var context = GetEndToEndDbContext();
                var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

                var expectedNumberOfPages = new PageOptions { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

                var queryParam = new Dictionary<string, string> { { "Page", expectedNumberOfPages.ToString() } };

                NavigateToUrl(
                    typeof(SolutionsController),
                    nameof(SolutionsController.Index),
                    queryParameters: queryParam);

                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();

                CommonActions.ElementTextEqualTo(CommonSelectors.PaginationPreviousSubText, $"{expectedNumberOfPages - 1} of {expectedNumberOfPages}")
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_ClickIntoSolution_ExpectedResult()
        {
            RunTest(() =>
            {
                var element = Driver.FindElement(SolutionsObjects.SolutionsLink);
                var solutionName = "-" + element.Text;

                element.Click();

                CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Description))
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(By.CssSelector("h1 .nhsuk-caption--bottom"), solutionName).Should().BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_MoreThan5Capabilities_ShowsLink()
        {
            RunTest(() =>
            {
                using var context = GetEndToEndDbContext();
                var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

                var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

                for (int i = 0; i <= expectedNumberOfPages; i++)
                {
                    if (CommonActions.ElementExists(SolutionsObjects.CapabilitesOverCountLink))
                        break;

                    CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);
                }

                var element = Driver.FindElement(SolutionsObjects.CapabilitesOverCountLink);

                var linkUrl = element.GetAttribute("href");

                var linkUrlStringArray = linkUrl.Split("/", System.StringSplitOptions.RemoveEmptyEntries)[3].Split("-");

                _ = int.TryParse(linkUrlStringArray[0], out int supplierId);

                var catalogueItemId = new CatalogueItemId(supplierId, linkUrlStringArray[1]);

                var catalogueItem = context.CatalogueItems.AsNoTracking()
                    .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                    .First(ci => ci.Id == catalogueItemId);

                CommonActions.ElementTextEqualTo(
                        SolutionsObjects.CapabilitesOverCountLink,
                        $"See all {catalogueItem.CatalogueItemCapabilities.Count} Capabilities")
                    .Should()
                    .BeTrue();

                CommonActions.ClickLinkElement(SolutionsObjects.CapabilitesOverCountLink);

                CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Capabilities))
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_SearchTerm_DisplaysFilterSummary()
        {
            RunTestWithRetry(() =>
            {
                using var context = GetEndToEndDbContext();
                var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);
                var solutionName = solution.CatalogueItem.Name;

                CommonActions.ElementAddValue(SolutionsObjects.SearchBar, solutionName);

                CommonActions.ClickLinkElement(SolutionsObjects.SearchButton);

                CommonActions.ElementIsDisplayed(SolutionsObjects.SearchTermTitle).Should().BeTrue();
                CommonActions.ElementIsDisplayed(SolutionsObjects.SearchTermContent).Should().BeTrue();
                CommonActions.ElementTextContains(SolutionsObjects.SearchTermContent, $"'{solutionName}'").Should().BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_ClickFilterFrameworkButton_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(SolutionsObjects.FilterCatalogueSolutionsLink);

                CommonActions.PageLoadedCorrectGetIndex(typeof(FilterController), nameof(FilterController.FilterCapabilities))
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_ClickStartNewSearchLink_ExpectedResult()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);

            CommonActions.ElementAddValue(SolutionsObjects.SearchBar, solution.CatalogueItem.Name);
            CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.SearchListBox);

            CommonActions.ClickLinkElement(SolutionsObjects.SearchButton);

            CommonActions.ClickLinkElement(SolutionsObjects.StartNewSearch);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_ClickEditCapabilitiesLink_ExpectedResult()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);

            CommonActions.ElementAddValue(SolutionsObjects.SearchBar, solution.CatalogueItem.Name);
            CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.SearchListBox);

            CommonActions.ClickLinkElement(SolutionsObjects.SearchButton);

            CommonActions.ClickLinkElement(SolutionsObjects.EditCapabilities);

            CommonActions.PageLoadedCorrectGetIndex(typeof(FilterController), nameof(FilterController.FilterCapabilities))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_ClickEditEpicsLink_ExpectedResult()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);

            CommonActions.ElementAddValue(SolutionsObjects.SearchBar, solution.CatalogueItem.Name);
            CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.SearchListBox);

            CommonActions.ClickLinkElement(SolutionsObjects.SearchButton);

            CommonActions.ClickLinkElement(SolutionsObjects.EditEpics);

            CommonActions.PageLoadedCorrectGetIndex(typeof(FilterController), nameof(FilterController.FilterEpics))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task CatalogueSolutions_SearchValid_DisplaysSolutions()
        {
            await RunTestWithRetryAsync(async () =>
            {
                using var context = GetEndToEndDbContext();
                var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);

                await CommonActions.ElementAddValueWithDelay(SolutionsObjects.SearchBar, solution.CatalogueItem.Name);

                CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.SearchListBox);

                CommonActions.ElementExists(SolutionsObjects.SearchResult(0))
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(
                    SolutionsObjects.SearchResultTitle(0),
                    solution.CatalogueItem.Name)
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(
                    SolutionsObjects.SearchResultDescription(0),
                    "Solution")
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_SearchInvalid_NoResults()
        {
            RunTestWithRetry(() =>
            {
                TextGenerators.TextInputAddText(SolutionsObjects.SearchBar, 5);

                CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.SearchListBox);

                CommonActions.ElementIsDisplayed(SolutionsObjects.NoResults)
                    .Should()
                    .BeTrue();

                CommonActions.ClickLinkElement(SolutionsObjects.SearchButton);

                CommonActions.ElementIsDisplayed(SolutionsObjects.NoResultsElement).Should().BeTrue();
                CommonActions.ElementIsDisplayed(SolutionsObjects.SolutionsList).Should().BeFalse();
            });
        }

        [Fact]
        public void CatalogueSolutions_SearchInvalid_ClickLink_NavigatesCorrectly()
        {
            RunTestWithRetry(() =>
            {
                TextGenerators.TextInputAddText(SolutionsObjects.SearchBar, 5);
                CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.SearchListBox);

                CommonActions.ClickLinkElement(SolutionsObjects.SearchButton);
                CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.NoResultsElement);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SolutionsController),
                    nameof(SolutionsController.Index))
                    .Should().BeTrue();
            });
        }

        [Fact]
        public void CatalogueSolutions_Search_FiltersResults()
        {
            RunTestWithRetry(() =>
            {
                using var context = GetEndToEndDbContext();
                var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);

                CommonActions.ElementAddValue(SolutionsObjects.SearchBar, solution.CatalogueItem.Name);
                CommonActions.WaitUntilElementIsDisplayed(SolutionsObjects.SearchListBox);

                CommonActions.ClickLinkElement(SolutionsObjects.SearchButton);

                var solutions = new ByChained(SolutionsObjects.SolutionsList, ByExtensions.DataTestId("solutions-card"));
                Driver.FindElements(solutions).Count.Should().Be(1);
            });
        }
    }
}
