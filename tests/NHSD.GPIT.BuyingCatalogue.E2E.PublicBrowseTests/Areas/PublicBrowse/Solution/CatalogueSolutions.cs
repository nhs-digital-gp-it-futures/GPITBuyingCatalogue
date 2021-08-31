using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solution
{
    public sealed class CatalogueSolutions
        : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Dictionary<string, string> Parameters = new();

        private static readonly List<CatalogueItem> CatalogueItems = new();

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
        }

        [Fact]
        public void CatalogueSolutions_ClickDefaultSortByLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SortByLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index))
            .Should()
            .BeTrue();

            Driver.Url.Should().EndWith($"sortBy={PageOptions.SortOptions.LastUpdated.ToString().ToLowerInvariant()}");
        }

        [Fact]
        public void CatalogueSolutions_ClickAToZSortByLink_ExpectedResult()
        {
            var queryParam = new Dictionary<string, string> { { "sortBy", "lastupdated" } };

            NavigateToUrl(
                typeof(SolutionsController),
                nameof(SolutionsController.Index),
                queryParameters: queryParam);

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.SortByLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index))
            .Should()
            .BeTrue();

            Driver.Url.Should().EndWith($"sortBy={PageOptions.SortOptions.Alphabetical.ToString().ToLowerInvariant()}");
        }

        [Fact]
        public void CatalogueSolutions_Pagination_CorrectNumberOfPages()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

            CommonActions.ElementTextEqualToo(CommonSelectors.PaginationNextSubText, $"2 of {expectedNumberOfPages}")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_Pagination_ClickNext_ExpectedResults()
        {
            CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(SolutionsController),
            nameof(SolutionsController.Index))
            .Should()
            .BeTrue();

            Driver.Url.Should().EndWith("Page=2");

            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

            CommonActions.ElementTextEqualToo(CommonSelectors.PaginationNextSubText, $"3 of {expectedNumberOfPages}")
                .Should()
                .BeTrue();

            CommonActions.ElementTextEqualToo(CommonSelectors.PaginationPreviousSubText, $"1 of {expectedNumberOfPages}")
            .Should()
            .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_Pagination_LastPage_ExpectedResults()
        {
            using var context = GetEndToEndDbContext();
            var countOfSolutions = context.CatalogueItems.AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions() { TotalNumberOfItems = countOfSolutions }.NumberOfPages;

            var queryParam = new Dictionary<string, string> { { "Page", expectedNumberOfPages.ToString() } };

            NavigateToUrl(
                typeof(SolutionsController),
                nameof(SolutionsController.Index),
                queryParameters: queryParam);

            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();

            CommonActions.ElementTextEqualToo(CommonSelectors.PaginationPreviousSubText, $"{expectedNumberOfPages - 1} of {expectedNumberOfPages}")
            .Should()
            .BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_ClickIntoSolution_ExpectedResult()
        {
            var element = Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.SolutionsLink);

            var solutionIdElements = element.GetAttribute("href").Split("/").Last().Split("-");

            var solutionName = element.Text;

            _ = int.TryParse(solutionIdElements[0], out int result);

            var solutionId = new CatalogueItemId(result, solutionIdElements[1]);

            element.Click();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(SolutionsController),
            nameof(SolutionsController.Description))
            .Should()
            .BeTrue();

            using var context = GetEndToEndDbContext();

            context.CatalogueItems.Where(ci => ci.Id == solutionId).Single();

            CommonActions.ElementTextEqualToo(By.CssSelector("h1 .nhsuk-caption--bottom"), solutionName).Should().BeTrue();
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

            var catalogueItemId = new EntityFramework.Ordering.Models.CatalogueItemId(supplierId, linkUrlStringArray[1]);

            var catalogueItem = context.CatalogueItems.AsNoTracking().Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Where(ci => ci.Id == catalogueItemId)
                .Single();

            CommonActions
                .ElementTextEqualToo(
                    Objects.PublicBrowse.SolutionsObjects.CapabilitesOverCountLink,
                    $"See all {catalogueItem.CatalogueItemCapabilities.Count} Capabilities")
                .Should()
                .BeTrue();

            CommonActions.ClickLinkElement(Objects.PublicBrowse.SolutionsObjects.CapabilitesOverCountLink);

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(SolutionsController),
            nameof(SolutionsController.Capabilities))
            .Should()
            .BeTrue();
        }
    }
}
