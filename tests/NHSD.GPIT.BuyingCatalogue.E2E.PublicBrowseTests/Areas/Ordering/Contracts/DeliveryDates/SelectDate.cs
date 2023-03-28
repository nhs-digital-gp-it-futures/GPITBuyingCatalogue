using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates
{
    [Collection(nameof(OrderingCollection))]
    public class SelectDate : BuyerTestBase, IDisposable
    {
        private const int OrderId = 90006;
        private const string InternalOrgId = "IB-QWO";
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId),  $"{CallOffId}" },
        };

        public SelectDate(LocalWebApplicationFactory factory)
            : base(factory, typeof(DeliveryDatesController), nameof(DeliveryDatesController.SelectDate), Parameters)
        {
        }

        [Fact]
        public void SelectDate_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Planned delivery date - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.SelectDateDayInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.SelectDateMonthInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.SelectDateYearInput).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void SelectDate_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void SelectDate_NoDayEntered_ClickSave_ExpectedResult()
        {
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateDayInput);
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateMonthInput);
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateYearInput);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.SelectDateError,
                $"Error: {DateInputModelValidator.DayMissingErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void SelectDate_NoMonthEntered_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "01");
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateMonthInput);
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateYearInput);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.SelectDateError,
                $"Error: {DateInputModelValidator.MonthMissingErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void SelectDate_NoYearEntered_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, "01");
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateYearInput);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.SelectDateError,
                $"Error: {DateInputModelValidator.YearMissingErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void SelectDate_YearTooShort_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, "22");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.SelectDateError,
                $"Error: {DateInputModelValidator.YearWrongLengthErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void SelectDate_DateInvalid_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "99");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, "99");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, "9999");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.SelectDateError,
                $"Error: {DateInputModelValidator.DateInvalidErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void SelectDate_DateInThePast_ClickSave_ExpectedResult()
        {
            var date = DateTime.Today.AddDays(-1);

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.SelectDateError,
                $"Error: {SelectDateModelValidator.DeliveryDateInThePastErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void SelectDate_DateBeforeCommencementDate_ClickSave_ExpectedResult()
        {
            var context = GetEndToEndDbContext();
            var date = context.Orders.First(x => x.Id == OrderId).CommencementDate!.Value;

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var errorMessage = string.Format(
                SelectDateModelValidator.DeliveryDateBeforeCommencementDateErrorMessage,
                $"{date:d MMMM yyyy}");

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.SelectDateError,
                $"Error: {errorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void SelectDate_ValidDateEntered_NoExistingDate_ClickSave_ExpectedResult()
        {
            var date = DateTime.Today.AddDays(2);

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.EditDates)).Should().BeTrue();

            var context = GetEndToEndDbContext();

            context.Orders.First(x => x.Id == OrderId).DeliveryDate.Should().Be(date);
            context.OrderItemRecipients.Where(x => x.OrderId == OrderId).ForEach(x => x.DeliveryDate.Should().Be(date));
        }

        [Fact]
        public void SelectDate_ValidDateEntered_WithExistingDate_ClickSave_ExpectedResult()
        {
            var context = GetEndToEndDbContext();

            context.Orders.First(x => x.Id == OrderId).DeliveryDate = DateTime.Today;

            context.SaveChanges();

            var date = DateTime.Today.AddDays(2);

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.ConfirmChanges)).Should().BeTrue();

            context = GetEndToEndDbContext();

            context.Orders.First(x => x.Id == OrderId).DeliveryDate.Should().Be(DateTime.Today);
            context.OrderItemRecipients.Where(x => x.OrderId == OrderId).ForEach(x => x.DeliveryDate.Should().BeNull());
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();

            context.Orders
                .First(x => x.Id == OrderId).DeliveryDate = null;

            context.OrderItemRecipients
                .Where(x => x.OrderId == OrderId)
                .ToList()
                .ForEach(x => x.DeliveryDate = null);

            context.SaveChanges();
        }

        private void VerifyNoChangesMade()
        {
            var context = GetEndToEndDbContext();

            context.Orders.First(x => x.Id == OrderId).DeliveryDate.Should().BeNull();
            context.OrderItemRecipients.Where(x => x.OrderId == OrderId).ForEach(x => x.DeliveryDate.Should().BeNull());
        }
    }
}
