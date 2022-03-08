using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class CatalogueSolutions
        : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string FrameworkCacheKey = "framework-filter";
        private const string CategoryCacheKey = "category-filter";
        private static readonly Dictionary<string, string> Parameters = new();

        public CatalogueSolutions(LocalWebApplicationFactory factory)
            : base(
                 factory,
                 typeof(SolutionsController),
                 nameof(SolutionsController.Index),
                 Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutions_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SortByLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header3).Should().BeTrue();
            CommonActions.ElementExists(CommonSelectors.Pagination).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Should().BeTrue();
            CommonActions.ElementExists(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsFramework).Should().BeTrue();
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);
        }

        [Fact]
        public void CatalogueSolutions_ClickDefaultSortByLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SortByLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index))
                .Should()
                .BeTrue();

            Driver.Url.Should().Contain($"sortBy={PageOptions.SortOptions.LastPublished.ToString().ToLowerInvariant()}");
        }

        [Fact]
        public void CatalogueSolutions_ClickAToZSortByLink_ExpectedResult()
        {
            var queryParam = new Dictionary<string, string> { { "sortBy", "lastpublished" } };

            NavigateToUrl(
                typeof(SolutionsController),
                nameof(SolutionsController.Index),
                queryParameters: queryParam);

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SortByLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index))
                .Should()
                .BeTrue();

            Driver.Url.Should().Contain($"sortBy={PageOptions.SortOptions.Alphabetical.ToString().ToLowerInvariant()}");
        }

        [Fact]
        public void CatalogueSolutions_Pagination_CorrectNumberOfPages()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

            CommonActions.ElementTextEqualTo(CommonSelectors.PaginationNextSubText, $"2 of {expectedNumberOfPages}")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_Pagination_ClickNext_ExpectedResults()
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
        }

        [Fact]
        public void CatalogueSolutions_Pagination_LastPage_ExpectedResults()
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
        }

        [Fact]
        public void CatalogueSolutions_ClickIntoSolution_ExpectedResult()
        {
            var element = Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.SolutionsLink);
            var solutionName = "-" + element.Text;

            element.Click();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Description))
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualTo(By.CssSelector("h1 .nhsuk-caption--bottom"), solutionName).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_MoreThan5Capabilities_ShowsLink()
        {
            using var context = GetEndToEndDbContext();
            var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

            for (int i = 0; i <= expectedNumberOfPages; i++)
            {
                if (CommonActions.ElementExists(Objects.PublicBrowse.SolutionsObjects.CapabilitesOverCountLink))
                    break;

                CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);
            }

            var element = Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.CapabilitesOverCountLink);

            var linkUrl = element.GetAttribute("href");

            var linkUrlStringArray = linkUrl.Split("/", System.StringSplitOptions.RemoveEmptyEntries)[3].Split("-");

            _ = int.TryParse(linkUrlStringArray[0], out int supplierId);

            var catalogueItemId = new CatalogueItemId(supplierId, linkUrlStringArray[1]);

            var catalogueItem = context.CatalogueItems.AsNoTracking()
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Single(ci => ci.Id == catalogueItemId);

            CommonActions.ElementTextEqualTo(
                    Objects.PublicBrowse.SolutionsObjects.CapabilitesOverCountLink,
                    $"See all {catalogueItem.CatalogueItemCapabilities.Count} Capabilities")
                .Should()
                .BeTrue();

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.CapabilitesOverCountLink);

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Capabilities))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async void CatalogueSolutions_Filter_AllFrameworksShown()
        {
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsFramework).Click();

            await using var context = GetEndToEndDbContext();

            var numberOfFrameworkFilters = await context.FrameworkSolutions.AsNoTracking()
                .Include(fs => fs.Framework)
                .Where(fs => fs.Solution.CatalogueItem.PublishedStatus == PublicationStatus.Published)
                .Select(fs => fs.Framework).Distinct()
                .CountAsync();

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(numberOfFrameworkFilters + 1);
        }

        [Fact]
        public async void CatalogueSolutions_Filter_FilterByOption_ExpectedNumberOfResults()
        {
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsFramework).Click();

            await using var context = GetEndToEndDbContext();

            var gpitFramework = await context.Frameworks.Where(f => f.Id == "NHSDGP001").SingleAsync();

            var numberOfSolutionsForGPITFramework =
                await context.CatalogueItems.AsNoTracking()
                .Where(ci => ci.PublishedStatus == PublicationStatus.Published
                            && ci.CatalogueItemType == CatalogueItemType.Solution
                            && ci.Solution.FrameworkSolutions.Any(fs => fs.FrameworkId == gpitFramework.Id))
                .CountAsync();

            CommonActions.ClickRadioButtonWithValue(gpitFramework.Id);

            CommonActions.ClickSave();

            Driver.FindElements(ByExtensions.DataTestId("solutions-card")).Count.Should().Be(numberOfSolutionsForGPITFramework);

            Driver.Url.Should().Contain($"selectedframework={gpitFramework.Id}");
        }

        [Fact]
        public async void CatalogueSolutions_Filter_FilterByOption_BreadcrumbsCorrect()
        {
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsFramework).Click();

            await using var context = GetEndToEndDbContext();

            var gpitFramework = await context.Frameworks.Where(f => f.Id == "NHSDGP001").SingleAsync();

            Driver.FindElements(CommonSelectors.BreadcrumbItem).Count.Should().Be(1);

            CommonActions.ClickRadioButtonWithValue(gpitFramework.Id);

            CommonActions.ClickSave();

            Driver.Url.Should().Contain($"selectedframework={gpitFramework.Id}");

            Wait.Until(driver => driver.FindElements(CommonSelectors.BreadcrumbItem)).Count.Should().Be(2);
        }

        [Fact]
        public async void CatalogueSolutions_Filter_FilterByFoundationCapability_CorrectResult()
        {
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities).Click();

            Driver.FindElement(By.Id("FoundationCapabilities_0__Selected")).Click();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var filterService = new SolutionsFilterService(context);

            var results = await filterService.GetAllCategoriesAndCountForFilter();

            Driver.FindElements(ByExtensions.DataTestId("solutions-card")).Count.Should().Be(results.CountOfCatalogueItemsWithFoundationCapabilities);

            MemoryCache.Remove(CategoryCacheKey);
        }

        [Fact]
        public void CatalogueSolutions_Filter_FilterByTwoCapabilities_CorrectResults()
        {
            const int firstCapabilityId = 46;
            const int secondCapabilityId = 47;

            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities).Click();

            Driver.FindElement(By.XPath($"//input[@value='C{firstCapabilityId}']/../input[contains(@class, 'nhsuk-checkboxes__input')]")).Click();
            Driver.FindElement(By.XPath($"//input[@value='C{secondCapabilityId}']/../input[contains(@class, 'nhsuk-checkboxes__input')]")).Click();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index)).Should().BeTrue();

            Driver.FindElements(ByExtensions.DataTestId("solutions-card")).Count.Should().Be(3);
        }

        [Theory]
        [InlineData("C46", "C46E5")]
        [InlineData("C46", "C46E6")]
        public void CatalogueSolutions_Filter_FilterByOneCapabilityWithEpic_CorrectResults(string capabilityId, string epicId)
        {
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities).Click();

            Driver.FindElement(By.XPath($"//input[@value='{capabilityId}']/../input[contains(@class, 'nhsuk-checkboxes__input')]")).Click();
            Driver.FindElement(By.XPath($"//input[@value='{epicId}']/../input[contains(@class, 'nhsuk-checkboxes__input')]")).Click();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index)).Should().BeTrue();

            Driver.FindElements(ByExtensions.DataTestId("solutions-card")).Count.Should().Be(1);
        }

        [Fact]
        public void CatalogueSolutions_Filter_MayEpicsOnly()
        {
            const string mayEpic = "C46E6";
            const string mustEpic = "C46E1";

            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities).Click();

            CommonActions.ElementExists(By.XPath($"//input[@value='{mayEpic}']")).Should().BeTrue();

            CommonActions.ElementExists(By.XPath($"//input[@value='{mustEpic}']")).Should().BeFalse();
        }

        [Fact]
        public void CatalogueSolutions_Filter_IncorrectFilterOptionsInput_CorrectResults()
        {
            Dictionary<string, string> queryParams = new()
            {
                { "Capabilities", "C999" },
            };

            NavigateToUrl(typeof(SolutionsController), nameof(SolutionsController.Index), queryParameters: queryParams);

            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.NoResultsElement).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_Filter_DisplaysFilterSummary()
        {
            using var context = GetEndToEndDbContext();
            var capability = context.Capabilities.First(c => c.CapabilityRef == "C46");
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities).Click();

            Driver.FindElement(By.XPath($"//input[@value='C46']/../input[contains(@class, 'nhsuk-checkboxes__input')]")).Click();
            Driver.FindElement(By.XPath($"//input[@value='C46E6']/../input[contains(@class, 'nhsuk-checkboxes__input')]")).Click();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SolutionsController), nameof(SolutionsController.Index)).Should().BeTrue();

            var text = Driver.FindElement(ByExtensions.DataTestId("search-criteria-summary")).Text;
            CommonActions.ElementTextContains(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary, $"{capability.Name} and 1 Epic").Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_SearchTerm_DisplaysFilterSummary()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);
            var solutionName = solution.CatalogueItem.Name;

            CommonActions.ElementAddValue(Objects.PublicBrowse.SolutionsObjects.SearchBar, solutionName);

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SearchButton);

            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary).Should().BeTrue();
            CommonActions.ElementTextContains(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary, $"Search term '{solutionName}'").Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_FilterByFramework_DisplaysFilterSummary()
        {
            CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
            Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsFramework).Click();

            using var context = GetEndToEndDbContext();
            var gpitFramework = context.Frameworks.Where(f => f.Id == "NHSDGP001").Single();

            CommonActions.ClickRadioButtonWithValue(gpitFramework.Id);
            CommonActions.ClickSave();

            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary).Should().BeTrue();
            CommonActions.ElementTextContains(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary, $"Framework '{gpitFramework.ShortName}'").Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_SearchValid_DisplaysSolutions()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);

            CommonActions.ElementAddValue(Objects.PublicBrowse.SolutionsObjects.SearchBar, solution.CatalogueItem.Name);

            CommonActions.WaitUntilElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchListBox);

            CommonActions.ElementExists(Objects.PublicBrowse.SolutionsObjects.SearchResult(0))
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.PublicBrowse.SolutionsObjects.SearchResultTitle(0),
                solution.CatalogueItem.Name)
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualTo(
                Objects.PublicBrowse.SolutionsObjects.SearchResultDescription(0),
                "Solution")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_SearchInvalid_NoResults()
        {
            TextGenerators.TextInputAddText(Objects.PublicBrowse.SolutionsObjects.SearchBar, 5);

            CommonActions.WaitUntilElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchListBox);

            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.NoResults)
                .Should()
                .BeTrue();

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SearchButton);

            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.NoResultsElement).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SolutionsList).Should().BeFalse();
        }

        [Fact]
        public void CatalogueSolutions_SearchInvalid_ClickLink_NavigatesCorrectly()
        {
            TextGenerators.TextInputAddText(Objects.PublicBrowse.SolutionsObjects.SearchBar, 5);
            CommonActions.WaitUntilElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchListBox);

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SearchButton);
            CommonActions.WaitUntilElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.NoResultsElement);

            CommonActions.ClickLinkElement(ByExtensions.DataTestId("clear-results-link"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index))
                .Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_Search_FiltersResults()
        {
            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);

            CommonActions.ElementAddValue(Objects.PublicBrowse.SolutionsObjects.SearchBar, solution.CatalogueItem.Name);
            CommonActions.WaitUntilElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchListBox);

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SearchButton);

            var solutions = new ByChained(Objects.PublicBrowse.SolutionsObjects.SolutionsList, ByExtensions.DataTestId("solutions-card"));
            Driver.FindElements(solutions).Count.Should().Be(1);
        }

        [Fact]
        public void CatalogueSolutions_Search_PreservesFilters()
        {
            void ExpandCapabilitiesFilter()
            {
                CommonActions.WaitUntilElementExists(Objects.PublicBrowse.SolutionsObjects.FilterCapabilities);

                Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsExpander).Click();
                Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.FilterSolutionsFramework).Click();
            }

            void ValidateFilterSelection()
            {
                ExpandCapabilitiesFilter();
                CommonActions.GetNumberOfSelectedRadioButtons().Should().Be(1);
            }

            ExpandCapabilitiesFilter();

            using var context = GetEndToEndDbContext();
            var solution = context.Solutions.Include(s => s.CatalogueItem).First(s => s.CatalogueItem.PublishedStatus == PublicationStatus.Published);
            var gpitFramework = context.Frameworks.Where(f => f.Id == "NHSDGP001").Single();

            CommonActions.ClickRadioButtonWithValue(gpitFramework.Id);
            CommonActions.ClickSave();

            ValidateFilterSelection();

            CommonActions.ElementAddValue(Objects.PublicBrowse.SolutionsObjects.SearchBar, solution.CatalogueItem.Name);
            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SearchButton);

            ValidateFilterSelection();

            CommonActions.ElementIsDisplayed(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary).Should().BeTrue();
            CommonActions.ElementTextContains(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary, $"Search term '{solution.CatalogueItem.Name}'").Should().BeTrue();
            CommonActions.ElementTextContains(Objects.PublicBrowse.SolutionsObjects.SearchCriteriaSummary, $"Framework '{gpitFramework.ShortName}'").Should().BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeMemoryCacheHandler();

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
