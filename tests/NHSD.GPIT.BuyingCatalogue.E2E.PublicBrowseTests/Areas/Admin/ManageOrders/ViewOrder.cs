using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageOrders
{
    public sealed class ViewOrder : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CallOffId CallOffId = new(90010, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public ViewOrder(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(ManageOrdersController),
                  nameof(ManageOrdersController.ViewOrder),
                  Parameters)
        {
        }

        [Fact]
        public void AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be($"Order summary - {CallOffId}".FormatForComparison());
            CommonActions.LedeText().Should().Be("View information about this order.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ViewOrderObjects.OrganisationSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.DescriptionSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.LastUpdatedBySection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.SupplierSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.StatusSection).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ViewOrderObjects.DownloadPdf).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.FullOrderCsv).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.PatientOnlyCsv).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ViewOrderObjects.OrdersItemsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.NoOrderItems).Should().BeFalse();
        }

        [Fact]
        public void NoOrderItems_SectionsDisplayedCorrectly()
        {
            var callOffId = new CallOffId(90004, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(CallOffId), callOffId.ToString() },
            };

            NavigateToUrl(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.ViewOrder),
                parameters);

            CommonActions.PageTitle().Should().Be($"Order summary - {callOffId}".FormatForComparison());
            CommonActions.LedeText().Should().Be("View information about this order.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ContinueButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(ViewOrderObjects.OrganisationSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.DescriptionSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.LastUpdatedBySection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.SupplierSection).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.StatusSection).Should().BeTrue();

            CommonActions.ElementIsDisplayed(ViewOrderObjects.DownloadPdf).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.FullOrderCsv).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.PatientOnlyCsv).Should().BeFalse();

            CommonActions.ElementIsDisplayed(ViewOrderObjects.OrdersItemsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.NoOrderItems).Should().BeTrue();
        }

        [Fact]
        public void ClickGoBack_NavigatesCorrectly()
        {
            CommonActions.ClickGoBackLink();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.Index))
                .Should().BeTrue();
        }

        [Fact]
        public void ClickContinue_NavigatesCorrectly()
        {
            CommonActions.ClickContinue();
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.Index))
                .Should().BeTrue();
        }

        [Fact]
        public void NoSupplier_CorrectSupplierSection()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.Include(o => o.Supplier).Single(o => o.Id == CallOffId.Id);
            var supplierId = order.Supplier.Id;

            order.SupplierId = null;
            context.SaveChangesAs(UserSeedData.SueId);

            Driver.Navigate().Refresh();

            CommonActions.ElementTextEqualTo(ViewOrderObjects.SupplierSection, "No supplier has been selected for this order yet".FormatForComparison()).Should().BeTrue();

            order.SupplierId = supplierId;
            context.SaveChangesAs(UserSeedData.SueId);
        }

        [Fact]
        public void Supplier_CorrectSupplierSection()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.Include(o => o.Supplier).Single(o => o.Id == CallOffId.Id);
            var supplier = order.Supplier;

            CommonActions.ElementTextEqualTo(ViewOrderObjects.SupplierSection, supplier.Name.FormatForComparison()).Should().BeTrue();
        }

        [Fact]
        public void InProgressOrder_NoCsvButtons()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.Single(o => o.Id == CallOffId.Id);

            order.OrderStatus = OrderStatus.InProgress;
            context.SaveChangesAs(UserSeedData.SueId);

            Driver.Navigate().Refresh();

            CommonActions.ElementIsDisplayed(ViewOrderObjects.DownloadPdf).Should().BeTrue();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.FullOrderCsv).Should().BeFalse();
            CommonActions.ElementIsDisplayed(ViewOrderObjects.PatientOnlyCsv).Should().BeFalse();

            order.OrderStatus = OrderStatus.Completed;
            context.SaveChangesAs(UserSeedData.SueId);
        }

        [Fact]
        public void Funding_NotStarted_AsExpected()
        {
            using var context = GetEndToEndDbContext();
            var orderItem = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemFunding)
                .Where(o => o.Id == CallOffId.Id)
                .Select(o => o.OrderItems.First())
                .Single();

            orderItem.OrderItemFunding = null;

            context.SaveChangesAs(UserSeedData.SueId);

            Driver.Navigate().Refresh();

            RunTest(() =>
            {
                CommonActions.ElementTextEqualTo(ViewOrderObjects.FundingType, "Not started".FormatForComparison()).Should().BeTrue();
            });
        }

        [Fact]
        public void Funding_CentalFunding_AsExpected()
        {
            using var context = GetEndToEndDbContext();
            var orderItem = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemFunding)
                .Where(o => o.Id == CallOffId.Id)
                .Select(o => o.OrderItems.First())
                .Single();

            if (orderItem.OrderItemFunding is null)
                SetOrderItemFunding(orderItem);

            orderItem.OrderItemFunding.CentralAllocation = orderItem.OrderItemFunding.TotalPrice;
            orderItem.OrderItemFunding.LocalAllocation = 0;

            context.SaveChangesAs(UserSeedData.SueId);

            Driver.Navigate().Refresh();
            RunTest(() =>
            {
                CommonActions.ElementTextEqualTo(ViewOrderObjects.FundingType, "Central funding".FormatForComparison()).Should().BeTrue();
            });
        }

        [Fact]
        public void Funding_LocalFunding_AsExpected()
        {
            using var context = GetEndToEndDbContext();
            var orderItem = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemFunding)
                .Where(o => o.Id == CallOffId.Id)
                .Select(o => o.OrderItems.First())
                .Single();

            if (orderItem.OrderItemFunding is null)
                SetOrderItemFunding(orderItem);

            orderItem.OrderItemFunding.CentralAllocation = 0;
            orderItem.OrderItemFunding.LocalAllocation = orderItem.OrderItemFunding.TotalPrice;

            context.SaveChangesAs(UserSeedData.SueId);

            Driver.Navigate().Refresh();

            RunTest(() =>
            {
                CommonActions.ElementTextEqualTo(ViewOrderObjects.FundingType, "Local funding".FormatForComparison()).Should().BeTrue();
            });
        }

        private static void SetOrderItemFunding(OrderItem item)
        {
            item.OrderItemFunding = new OrderItemFunding
            {
                OrderId = item.OrderId,
                CatalogueItemId = item.CatalogueItemId,
                OrderItem = item,
                TotalPrice = 1000,
            };
        }
    }
}
