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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.DeliveryDates;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.Contracts.DeliveryDates
{
    [Collection(nameof(OrderingCollection))]
    public class AmendDate : BuyerTestBase, IDisposable
    {
        private const int OrderNumber = 90031;
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(OrderNumber, 2);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");
        private static readonly RoutingSource Source = RoutingSource.TaskList;

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        private static readonly Dictionary<string, string> QueryParameters = new()
        {
            { nameof(Source), $"{Source}" },
        };

        public AmendDate(LocalWebApplicationFactory factory)
            : base(factory, typeof(DeliveryDatesController), nameof(DeliveryDatesController.AmendDate), Parameters, queryParameters: QueryParameters)
        {
        }

        [Fact]
        public void AmendDate_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("Planned delivery date - E2E Multiple Prices Additional Service".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.SelectDateDayInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.SelectDateMonthInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(DeliveryDatesObjects.SelectDateYearInput).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AmendDate_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        [Fact]
        public void AmendDate_NoDayEntered_ClickSave_ExpectedResult()
        {
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateDayInput);
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateMonthInput);
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateYearInput);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {DateInputModelValidator.DayMissingErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void AmendDate_NoMonthEntered_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "01");
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateMonthInput);
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateYearInput);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {DateInputModelValidator.MonthMissingErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void AmendDate_NoYearEntered_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, "01");
            CommonActions.ClearInputElement(DeliveryDatesObjects.SelectDateYearInput);
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {DateInputModelValidator.YearMissingErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void AmendDate_YearTooShort_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, "01");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, "22");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {DateInputModelValidator.YearWrongLengthErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void AmendDate_DateInvalid_ClickSave_ExpectedResult()
        {
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, "99");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, "99");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, "9999");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {DateInputModelValidator.DateInvalidErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void AmendDate_DateInThePast_ClickSave_ExpectedResult()
        {
            var date = DateTime.Today.AddDays(-1);

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {AmendDateModelValidator.DeliveryDateInThePastErrorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void AmendDate_DateBeforeCommencementDate_ClickSave_ExpectedResult()
        {
            var context = GetEndToEndDbContext();
            var date = context.Order(CallOffId).Result.CommencementDate!.Value;

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var errorMessage = string.Format(
                AmendDateModelValidator.DeliveryDateBeforeCommencementDateErrorMessage,
                $"{date:d MMMM yyyy}");

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {errorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Theory]
        [InlineData(OrderTriageValue.Under40K)]
        [InlineData(OrderTriageValue.Between40KTo250K)]
        [InlineData(OrderTriageValue.Over250K)]
        public void AmendDate_DateAfterContractEndDate_ClickSave_ExpectedResult(OrderTriageValue triageValue)
        {
            var context = GetEndToEndDbContext();
            var order = context.Order(CallOffId).Result;

            order.OrderTriageValue = triageValue;

            context.SaveChanges();

            var endDate = new EndDate(order.CommencementDate.Value, order.MaximumTerm.Value);
            var invalidDate = endDate.DateTime.Value!.AddDays(1);

            Driver.Navigate().Refresh();

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{invalidDate.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{invalidDate.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{invalidDate.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.AmendDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            var errorMessage = string.Format(
                AmendDateModelValidator.DeliveryDateAfterContractEndDateErrorMessage,
                $"{invalidDate.AddDays(-1):d MMMM yyyy}");

            CommonActions.ElementTextEqualTo(
                DeliveryDatesObjects.AmendDateError,
                $"Error: {errorMessage}").Should().BeTrue();

            VerifyNoChangesMade();
        }

        [Fact]
        public void AmendDate_ValidDateEntered_ClickSave_ExpectedResult()
        {
            var date = DateTime.Today.AddDays(2);

            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateDayInput, $"{date.Day:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateMonthInput, $"{date.Month:00}");
            CommonActions.ElementAddValue(DeliveryDatesObjects.SelectDateYearInput, $"{date.Year:0000}");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(TaskListController),
                nameof(TaskListController.TaskList)).Should().BeTrue();

            var context = GetEndToEndDbContext();
            var order = context.Order(CallOffId).Result;

            order.DeliveryDate.Should().BeNull();

            context.OrderItemRecipients
                .Where(x => x.OrderId == order.Id && x.CatalogueItemId == CatalogueItemId)
                .ForEach(x => x.DeliveryDate.Should().Be(date));
        }

        public void Dispose()
        {
            var context = GetEndToEndDbContext();
            var order = context.Order(CallOffId).Result;

            order.DeliveryDate = null;
            order.OrderTriageValue = OrderTriageValue.Under40K;

            context.OrderItemRecipients
                .Where(x => x.OrderId == order.Id && x.CatalogueItemId == CatalogueItemId)
                .ForEach(x => x.DeliveryDate = null);

            context.SaveChanges();
        }

        private void VerifyNoChangesMade()
        {
            var context = GetEndToEndDbContext();
            var order = context.Order(CallOffId).Result;

            order.DeliveryDate.Should().BeNull();

            context.OrderItemRecipients
                .Where(x => x.OrderId == order.Id && x.CatalogueItemId == CatalogueItemId)
                .ForEach(x => x.DeliveryDate.Should().BeNull());
        }
    }
}
