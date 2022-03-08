using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class OrganisationDashboard
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";

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
            var organisation = await context.Organisations.SingleAsync(o => o.InternalIdentifier == Parameters["InternalOrgId"]);

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
                nameof(DashboardController.Organisation))
                .Should()
                .BeTrue();

            Driver.Url.Should().EndWith("page=2");

            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationPrevious).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeFalse();

            using var context = GetEndToEndDbContext();
            var countOfOrders = context.Organisations.Where(o => o.InternalIdentifier == InternalOrgId).SelectMany(o => o.Orders).AsNoTracking().Count();

            var expectedNumberOfPages = new PageOptions()
            {
                TotalNumberOfItems = countOfOrders,
                PageSize = 10,
            }.NumberOfPages;

            CommonActions.ElementTextEqualTo(CommonSelectors.PaginationPreviousSubText, $"1 of {expectedNumberOfPages}")
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(OrderStatus.InProgress, "Edit")]
        [InlineData(OrderStatus.Complete, "View")]
        public void OrganisationDashboard_OrderStatus_DisplaysCorrectLinkText(
            OrderStatus status,
            string expectedLinkText)
        {
            using var context = GetEndToEndDbContext();
            var orders = context.Organisations.SelectMany(o => o.Orders).OrderByDescending(o => o.LastUpdated).ToList();
            var order = orders.First(o => o.OrderStatus == status);

            CommonActions.ElementTextContains(ByExtensions.DataTestId($"link-{order.CallOffId}"), expectedLinkText).Should().BeTrue();
        }

        [Fact]
        public void OrganisationDashboard_OrderInProgress_ClickEdit_Redirects()
        {
            using var context = GetEndToEndDbContext();
            var orders = context.Organisations.SelectMany(o => o.Orders).OrderByDescending(o => o.LastUpdated).ToList();
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
            var orders = context.Organisations.SelectMany(o => o.Orders).OrderByDescending(o => o.LastUpdated).ToList();
            var order = orders.First(o => o.OrderStatus == OrderStatus.Complete);

            CommonActions.ClickLinkElement(ByExtensions.DataTestId($"link-{order.CallOffId}"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Summary))
                .Should().BeTrue();
        }

        [Fact]
        public async Task OrderDashboard_SearchValid_DisplaysOrders()
        {
            using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.Include(o => o.Orders).SingleAsync(o => o.InternalIdentifier == InternalOrgId);
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
        }

        [Fact]
        public void OrderDashboard_SearchInvalid_NoResults()
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
        }

        [Fact]
        public void OrderingDashboard_SearchInvalid_ClickLink_NavigatesCorrectly()
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
        }

        [Fact]
        public async Task OrderDashboard_Search_FiltersTable()
        {
            using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.Include(o => o.Orders).SingleAsync(o => o.InternalIdentifier == InternalOrgId);
            var order = organisation.Orders.First();

            CommonActions.ElementAddValue(Objects.Ordering.OrganisationDashboard.SearchBar, order.CallOffId.ToString());
            CommonActions.WaitUntilElementIsDisplayed(Objects.Ordering.OrganisationDashboard.SearchListBox);

            CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.SearchButton);

            CommonActions.GetNumberOfTableRowsDisplayed().Should().Be(1);
        }

        [Fact]
        public void OrderDashboard_NoOrders_SectionsCorrectlyDisplayed()
        {
            using var context = GetEndToEndDbContext();
            var organisation = context.Organisations.AsNoTracking().Single(o => o.InternalIdentifier == InternalOrgId);
            var orders = context.Orders.AsNoTracking().Where(o => o.OrderingPartyId == organisation.Id).ToList();

            context.RemoveRange(orders);
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

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }

        [Fact]
        public void OrganisationDashboard_CreateOrder_Click()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.Index)).Should().BeTrue();
        }
    }
}
