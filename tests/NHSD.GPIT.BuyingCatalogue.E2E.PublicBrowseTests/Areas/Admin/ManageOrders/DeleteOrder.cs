using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ManageOrders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageOrders
{
    [Collection(nameof(AdminCollection))]
    public sealed class DeleteOrder
        : AuthorityTestBase, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90009, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public DeleteOrder(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(ManageOrdersController),
              nameof(ManageOrdersController.DeleteOrder),
              Parameters)
        {
        }

        [Fact]
        public void DeleteOrder_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.WarningCallout);
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.NameOfApprover);
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.NameOfRequester);
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DateOfApproval);
        }

        [Fact]
        public void DeleteOrder_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.ViewOrder))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void DeleteOrder_ClickCancel_ExpectedResult()
        {
            CommonActions.ClickCancel();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(ManageOrdersController),
                nameof(ManageOrdersController.ViewOrder))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void DeleteOrder_SelectDeleteOrder_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfRequester, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfApprover, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalDayInput, DateTime.Now.Day.ToString("00"));
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalMonthInput, DateTime.Now.Month.ToString("00"));
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalYearInput, DateTime.Now.Year.ToString("0000"));

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.Index))
                .Should()
                .BeTrue();

            var id = CommonActions.GetTableRowCells();
            id.First().Should().Contain(CallOffId.ToString());
            var status = CommonActions.GetTableRowCells(3);
            status.First().Should().Contain("Deleted");
        }

        [Fact]
        public void DeleteOrder_NoSelection_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.DeleteOrder))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteRequestNameError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteApproveNameError).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteDayMissingError).Should().BeTrue();
        }

        [Fact]
        public void DeleteOrder_OnlyDay_ThrowsError()
        {
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfRequester, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfApprover, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalDayInput, "01");

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.DeleteOrder))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteMonthMissingError).Should().BeTrue();
        }

        [Fact]
        public void DeleteOrder_OnlyDayAndMonth_ThrowsError()
        {
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfRequester, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfApprover, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalDayInput, "01");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalMonthInput, "01");

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.DeleteOrder))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteYearMissingError).Should().BeTrue();
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        public void DeleteOrder_YearTooShort_ThrowsError(string year)
        {
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfRequester, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfApprover, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalDayInput, "01");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalMonthInput, "01");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalYearInput, year);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.DeleteOrder))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteYearTooShortError).Should().BeTrue();
        }

        [Theory]
        [InlineData("99", "01", "2000")]
        [InlineData("01", "99", "2000")]
        public void DeleteOrder_InvalidDate_ThrowsError(string day, string month, string year)
        {
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfRequester, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfApprover, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalDayInput, day);
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalMonthInput, month);
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalYearInput, year);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.DeleteOrder))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteInvalidDateError).Should().BeTrue();
        }

        [Fact]
        public void DeleteOrder_DateInTheFuture_ThrowsError()
        {
            var nextYear = DateTime.Now.Year + 1;
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfRequester, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.NameOfApprover, "Test");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalDayInput, "01");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalMonthInput, "01");
            CommonActions.ElementAddValue(DeleteOrderObjects.DateOfApprovalYearInput, nextYear.ToString());

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(ManageOrdersController),
                    nameof(ManageOrdersController.DeleteOrder))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeleteOrderObjects.DeleteDateInFutureError).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.Where(x => x.OrderNumber == CallOffId.OrderNumber && x.Revision == CallOffId.Revision).ToList();

            order.ForEach(x => x.IsDeleted = false);

            context.SaveChanges();
        }
    }
}
