using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    [Collection(nameof(OrderingCollection))]
    public sealed class OrganisationDashboard
        : BuyerTestBase
    {
        private const string InternalOrgId = "IB-QWO";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrganisationDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(DashboardController),
                  nameof(DashboardController.Organisation),
                  Parameters)
        {
        }

        [Fact]
        public async Task OrganisationDashboard_SectionsCorrectlyDisplayed()
        {
            await using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.FirstAsync(o => o.InternalIdentifier == Parameters["InternalOrgId"]);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Orders dashboard - {organisation.Name}".FormatForComparison());
            CommonActions.LedeText().Should().BeEquivalentTo("Manage orders currently in progress and view completed orders.".FormatForComparison());

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.ActOnBehalf).Should().BeFalse();
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.CreateOrderLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchBar).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("orders-table")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();
            CommonActions.ElementExists(CommonSelectors.PaginationPrevious).Should().BeFalse();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                  typeof(HomeController),
                  nameof(HomeController.Index))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_Pagination_CorrectNumberOfPages()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var countOfOrders = context.Organisations.Where(o => o.InternalIdentifier == InternalOrgId).SelectMany(o => o.Orders).AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions()
            {
                TotalNumberOfItems = countOfOrders,
                PageSize = 10,
            }.NumberOfPages;

            CommonActions.ElementTextEqualTo(CommonSelectors.PaginationNextSubText, $"2 of {expectedNumberOfPages}")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_Pagination_ClickNext_ExpectedResults()
        {
            CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();

            Driver.Url.Should().EndWith("page=2");

            CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();

            Driver.Url.Should().EndWith("page=3");

            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();

            CommonActions.ClickLinkElement(CommonSelectors.PaginationNext);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DashboardController),
                nameof(DashboardController.Organisation)).Should().BeTrue();

            Driver.Url.Should().EndWith("page=4");

            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();

            using var context = GetEndToEndDbContext();
            var countOfOrders = context.Organisations.Where(o => o.InternalIdentifier == InternalOrgId).SelectMany(o => o.Orders).AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions
            {
                TotalNumberOfItems = countOfOrders,
                PageSize = 10,
            }.NumberOfPages;

            CommonActions.ElementTextEqualTo(CommonSelectors.PaginationPreviousSubText, $"3 of {expectedNumberOfPages}")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_OrderInProgress_ClickEdit_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var orders = context.Organisations
                .Where(o => o.InternalIdentifier == InternalOrgId)
                .SelectMany(o => o.Orders)
                .OrderByDescending(o => o.LastUpdated)
                .ToList();
            var order = orders.First(o => o.OrderStatus == OrderStatus.InProgress);

            CommonActions.ClickLinkElement(ByExtensions.DataTestId($"link-{order.CallOffId}"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
                .Should().BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_OrderCompleted_ClickView_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.OrderBy(x => x.Revision).First(o => o.OrderingParty.InternalIdentifier == InternalOrgId && o.Completed == null);
            order.Completed = DateTime.UtcNow;
            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ClickLinkElement(ByExtensions.DataTestId($"link-{order.CallOffId}"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary))
                .Should().BeTrue();

            order.Completed = null;
            context.SaveChanges();
        }

        [Fact]
        public async Task OrderDashboard_SearchValid_DisplaysOrders()
        {
            await RunTestWithRetryAsync(async () =>
            {
                using var context = GetEndToEndDbContext();
                var organisation = await context.Organisations.Include(o => o.Orders).FirstAsync(o => o.InternalIdentifier == InternalOrgId);
                var order = organisation.Orders.First();

                CommonActions.ElementAddValue(Objects.Ordering.OrganisationDashboard.SearchBar, order.CallOffId.ToString());

                CommonActions.WaitUntilElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchListBox);

                CommonActions.ElementExists(Objects.Ordering.OrganisationDashboard.SearchResult(0))
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(
                    Objects.Ordering.OrganisationDashboard.SearchResultTitle(0),
                    order.Description)
                    .Should()
                    .BeTrue();

                CommonActions.ElementTextEqualTo(
                    Objects.Ordering.OrganisationDashboard.SearchResultDescription(0),
                    order.CallOffId.ToString())
                    .Should()
                    .BeTrue();
            });
        }

        [Fact]
        public void OrderDashboard_SearchInvalid_NoResults()
        {
            RunTestWithRetry(() =>
            {
                TextGenerators.TextInputAddText(Objects.Ordering.OrganisationDashboard.SearchBar, 5);

                CommonActions.WaitUntilElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchListBox);

                CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.NoResults)
                    .Should()
                    .BeTrue();

                CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.SearchButton);

                CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchBar).Should().BeTrue();
                CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.NoResultsElement).Should().BeTrue();
                CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("orders-table")).Should().BeFalse();
            });
        }

        [Fact]
        public void OrderingDashboard_SearchInvalid_ClickLink_NavigatesCorrectly()
        {
            RunTestWithRetry(() =>
            {
                TextGenerators.TextInputAddText(Objects.Ordering.OrganisationDashboard.SearchBar, 5);
                CommonActions.WaitUntilElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchListBox);

                CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.SearchButton);
                CommonActions.WaitUntilElementIsDisplayed(Objects.Ordering.OrganisationDashboard.NoResultsElement);

                CommonActions.ClickLinkElement(ByExtensions.DataTestId("clear-results-link"));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(DashboardController),
                    nameof(DashboardController.Organisation))
                    .Should().BeTrue();
            });
        }

        [Fact]
        public async Task OrderDashboard_Search_FiltersTable()
        {
            await RunTestWithRetryAsync(async () =>
            {
                using var context = GetEndToEndDbContext();
                var organisation = await context.Organisations.Include(o => o.Orders).FirstAsync(o => o.InternalIdentifier == InternalOrgId);
                var order = organisation.Orders.First();

                CommonActions.ElementAddValue(Objects.Ordering.OrganisationDashboard.SearchBar, order.CallOffId.ToString());
                CommonActions.WaitUntilElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchListBox);

                CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.SearchButton);

                CommonActions.GetNumberOfTableRowsDisplayed().Should().Be(1);
            });
        }

        [Fact]
        public void OrderDashboard_NoOrders_SectionsCorrectlyDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.AsNoTracking().First(o => o.InternalIdentifier == InternalOrgId);
            var orders = context.Orders.Where(o => o.OrderingPartyId == organisation.Id && !o.IsDeleted).ToList();
            orders.ForEach(x => x.IsDeleted = true);

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.CreateOrderLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchBar).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("orders-table")).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeFalse();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();
            CommonActions.ElementExists(CommonSelectors.PaginationPrevious).Should().BeFalse();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(By.Id("no-orders")).Should().BeTrue();

            orders.ForEach(x => x.IsDeleted = false);
            context.SaveChanges();
        }

        [Fact]
        public void OrganisationDashboard_CreateOrder_Click()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.OrderItemType)).Should().BeTrue();
        }
    }
}
