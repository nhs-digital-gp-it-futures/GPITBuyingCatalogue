using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Extensions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageOrders
{
    [Collection(nameof(AdminCollection))]
    public sealed class Dashboard : AuthorityTestBase
    {
        public Dashboard(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                  factory,
                  typeof(ManageOrdersController),
                  nameof(ManageOrdersController.Index),
                  null,
                  testOutputHelper)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.PageTitle().Should().Be("Manage orders".FormatForComparison());
                CommonActions.LedeText().Should().Be("View all orders that have been created on the Buying Catalogue.".FormatForComparison());

                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.HomeBreadCrumb).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.OrdersTable).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.NoOrdersElement).Should().BeFalse();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.NoSearchResults).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNextSubText).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPreviousSubText).Should().BeFalse();
            });
        }

        [Fact]
        public void NoOrders_CorrectSectionsDisplayed()
        {
            RunTest(() =>
            {
                using var context = GetEndToEndDbContext();
                var orders = context.Orders
                    .Include(o => o.Supplier)
                    .Include(o => o.OrderingParty)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.CatalogueItem)
                    .ToList();

                context.Orders.RemoveRange(orders);
                context.SaveChanges();

                Driver.Navigate().Refresh();

                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.HomeBreadCrumb).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.SearchBar).Should().BeFalse();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.OrdersTable).Should().BeFalse();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.NoOrdersElement).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.NoSearchResults).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNextSubText).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPreviousSubText).Should().BeFalse();

                context.InsertRangeWithIdentity(orders);
                context.SaveChanges();
            });
        }

        [Fact]
        public void ClickBreadcrumb_NavigatesCorrectly()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(ManageOrdersDashboardObjects.HomeBreadCrumb);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index)).Should().BeTrue();
            });
        }

        [Fact]
        public void Pagination_ClickNext_NavigatesToNextPage()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNextSubText).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPreviousSubText).Should().BeFalse();

                CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.Index)).Should().BeTrue();

                Driver.GetQueryValue("page").Should().Be("2");

                CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNextSubText).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPreviousSubText).Should().BeTrue();

                CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.Index)).Should().BeTrue();

                Driver.GetQueryValue("page").Should().Be("3");

                CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNextSubText).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPreviousSubText).Should().BeTrue();

                CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.Index)).Should().BeTrue();

                Driver.GetQueryValue("page").Should().Be("4");

                CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNextSubText).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPreviousSubText).Should().BeTrue();
            });
        }

        [Fact]
        public void Pagination_ClickPrevious_NavigatesToPreviousPage()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

                Driver.GetQueryValue("page").Should().Be("2");

                CommonActions.ClickLinkElement(CommonSelectors.PaginationPrevious);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.Index)).Should().BeTrue();

                Driver.GetQueryValue("page").Should().Be("1");
            });
        }

        [Fact]
        public void Search_NoMatch_CorrectSectionsDisplayed()
        {
            RunTestWithRetry(() =>
            {
                var searchText = TextGenerators.TextInputAddText(ManageOrdersDashboardObjects.SearchBar, 15);

                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchListBox);

                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.NoSearchResults).Should().BeTrue();

                CommonActions.ClickLinkElement(ManageOrdersDashboardObjects.SearchButton);

                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.HomeBreadCrumb).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.OrdersTable).Should().BeFalse();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.NoOrdersElement).Should().BeFalse();
                CommonActions.ElementIsDisplayed(ManageOrdersDashboardObjects.NoResultsElement).Should().BeTrue();
                CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNextSubText).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
                CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPreviousSubText).Should().BeFalse();

                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.NoResultsElement, $"No results were found for \"{searchText}\"".FormatForComparison()).Should().BeTrue();
            });
        }

        [Fact]
        public async Task Search_CallOffId_ResultsDisplayedCorrectly()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var order = context.Orders.First();
                var callOffId = order.CallOffId.ToString();

                await CommonActions.InputCharactersWithDelay(ManageOrdersDashboardObjects.SearchBar, callOffId);

                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchListBox);
                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchResultDescription(0));

                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultTitle(0), callOffId.FormatForComparison()).Should().BeTrue();
                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultDescription(0), "Call-off ID".FormatForComparison()).Should().BeTrue();
            });
        }

        [Fact]
        public void Search_Organisation_ResultsDisplayedCorrectly()
        {
            RunTestWithRetry(() =>
            {
                using var context = GetEndToEndDbContext();
                var order = context.Orders.Include(o => o.OrderingParty).First();
                var organisationName = order.OrderingParty.Name;

                CommonActions.ElementAddValue(ManageOrdersDashboardObjects.SearchBar, organisationName);

                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchListBox);
                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchResultDescription(0));

                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultTitle(0), organisationName.FormatForComparison()).Should().BeTrue();
                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultDescription(0), "Organisation".FormatForComparison()).Should().BeTrue();
            });
        }

        [Fact]
        public async Task Search_Supplier_ResultsDisplayedCorrectly()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var order = context.Orders.Include(o => o.Supplier).First(o => o.Supplier != null);
                var supplierName = order.Supplier.Name;

                await CommonActions.ElementAddValueWithDelay(ManageOrdersDashboardObjects.SearchBar, supplierName);

                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchListBox);
                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchResultDescription(0));

                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultTitle(0), supplierName.FormatForComparison()).Should().BeTrue();
                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultDescription(0), "Supplier".FormatForComparison()).Should().BeTrue();
            });
        }

        [Fact]
        public async Task Search_Solution_ResultsDisplayedCorrectly()
        {
            await RunTestWithRetryAsync(async () =>
            {
                await using var context = GetEndToEndDbContext();
                var orders = context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.CatalogueItem).Where(o => o.OrderItems.Any());
                var order = orders.First(o => o.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution));
                var solutionName = order.OrderItems.First().CatalogueItem.Name;

                await CommonActions.ElementAddValueWithDelay(ManageOrdersDashboardObjects.SearchBar, solutionName);

                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchListBox);
                CommonActions.WaitUntilElementIsDisplayed(ManageOrdersDashboardObjects.SearchResultDescription(0));

                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultTitle(0), solutionName.FormatForComparison()).Should().BeTrue();
                CommonActions.ElementTextEqualTo(ManageOrdersDashboardObjects.SearchResultDescription(0), "Solution".FormatForComparison()).Should().BeTrue();
            });
        }
    }
}
