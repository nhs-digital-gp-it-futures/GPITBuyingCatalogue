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
        private const string OdsCode = "03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
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
        public async Task OrganisationDashboard_OrganisationDashboard_SectionsCorrectlyDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.OrganisationDashboard.CreateOrderLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ByExtensions.DataTestId("orders-table")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Pagination).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.PaginationNext).Should().BeTrue();
            CommonActions.ElementExists(CommonSelectors.PaginationPrevious).Should().BeFalse();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var organisation = await context.Organisations.SingleAsync(o => o.OdsCode == Parameters["OdsCode"]);

            CommonActions.PageTitle().Should().BeEquivalentTo($"Orders dashboard - {organisation.Name}".FormatForComparison());
            CommonActions.LedeText().Should().BeEquivalentTo("Manage orders currently in progress and view completed orders.".FormatForComparison());
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
            var countOfOrders = context.Organisations.Where(o => o.OdsCode == OdsCode).SelectMany(o => o.Orders).AsNoTracking().Count();

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
            var countOfOrders = context.Organisations.Where(o => o.OdsCode == OdsCode).SelectMany(o => o.Orders).AsNoTracking().Count();

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
        public void OrganisationDashboard_OrderInProgress_DisplaysCorrectLinkText(
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
        public void OrganisationDashboard_OrderCompleted_ClickEdit_Redirects()
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
        public void OrganisationDashboard_CreateOrder_Click()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.OrganisationDashboard.CreateOrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderTriageController),
                nameof(OrderTriageController.Index)).Should().BeTrue();
        }
    }
}
