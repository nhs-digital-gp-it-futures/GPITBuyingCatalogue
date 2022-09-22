using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class CommencementDate : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90003, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), CallOffId.ToString() },
        };

        public CommencementDate(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CommencementDateController),
                  nameof(CommencementDateController.CommencementDate),
                  Parameters)
        {
        }

        [Fact]
        public void CommencementDate_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo($"Timescales for Call-off Agreement - Order {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.CommencementDateDayInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.CommencementDateYearInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.InitialPeriodInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.MaximumTermInput).Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_NoInputThrowsError()
        {
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).Clear();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.DayMissingErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_OnlyDay_ThrowsError()
        {
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).Clear();

            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).SendKeys("1");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.MonthMissingErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_OnlyDayAndMonth_ThrowsError()
        {
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).Clear();

            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).SendKeys("1");
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).SendKeys("1");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.YearMissingErrorMessage}").Should().BeTrue();
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        public void CommencementDate_YearTooShort_ThrowsError(string year)
        {
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).Clear();

            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).SendKeys("1");
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).SendKeys("1");
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).SendKeys(year);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.YearWrongLengthErrorMessage}").Should().BeTrue();
        }

        [Theory]
        [InlineData("99", "01", "2000")]
        [InlineData("01", "99", "2000")]
        public void CommencementDate_InvalidDate_ThrowsError(string day, string month, string year)
        {
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).Clear();

            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).SendKeys(day);
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).SendKeys(month);
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).SendKeys(year);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.DateInvalidErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_DateInThePast_ThrowsError()
        {
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.CommencementDateDayInput, "1");
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.CommencementDateMonthInput, "1");
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.CommencementDateYearInput, "2000");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                $"Error:{CommencementDateModelValidator.CommencementDateInThePastErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_InitialPeriodAndMaximumTermMissing_ThrowsError()
        {
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.InitialPeriodInput, string.Empty);
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.MaximumTermInput, string.Empty);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.MaximumTermError,
                CommencementDateModelValidator.MaximumTermMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_InitialPeriodAndMaximumTermNonNumeric_ThrowsError()
        {
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.InitialPeriodInput, "A");
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.MaximumTermInput, "B");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodNotANumberErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.MaximumTermError,
                CommencementDateModelValidator.MaximumTermNotANumberErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_InitialPeriodAndMaximumTermTooLow_ThrowsError()
        {
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.InitialPeriodInput, "0");
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.MaximumTermInput, "0");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodTooLowErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.MaximumTermError,
                CommencementDateModelValidator.MaximumTermTooLowErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_InitialPeriodAndMaximumTermTooHigh_ThrowsError()
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CommencementDate.InitialPeriodInput,
                $"{CommencementDateModelValidator.MaximumInitialPeriod + 1}");

            CommonActions.ElementAddValue(
                Objects.Ordering.CommencementDate.MaximumTermInput,
                "37");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodTooHighErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.MaximumTermError,
                string.Format(CommencementDateModelValidator.MaximumTermTooHighErrorMessage, 36)).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_OrderValue40K_InitialPeriodAndMaximumTermTooHigh_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.First(o => o.Id == CallOffId.Id);
            order.OrderTriageValue = OrderTriageValue.Under40K;

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.ElementAddValue(
                Objects.Ordering.CommencementDate.InitialPeriodInput,
                $"{CommencementDateModelValidator.MaximumInitialPeriod + 1}");

            CommonActions.ElementAddValue(
                Objects.Ordering.CommencementDate.MaximumTermInput,
                $"13");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodTooHighErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.MaximumTermError,
                string.Format(CommencementDateModelValidator.MaximumTermTooHighErrorMessage, 12)).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_MaximumTermInvalid_ThrowsError()
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CommencementDate.InitialPeriodInput,
                "6");

            CommonActions.ElementAddValue(
                Objects.Ordering.CommencementDate.MaximumTermInput,
                "5");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.MaximumTermError,
                CommencementDateModelValidator.MaximumTermInvalidErrorMessage).Should().BeTrue();
        }

        [Fact]
        public async Task CommencementDate_EverythingOk_AddsCommencementDate()
        {
            const int initialPeriod = 3;
            const int maximumTerm = 12;

            var date = TextGenerators.DateInputAddDateSoon(
                Objects.Ordering.CommencementDate.CommencementDateDayInput,
                Objects.Ordering.CommencementDate.CommencementDateMonthInput,
                Objects.Ordering.CommencementDate.CommencementDateYearInput);

            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.InitialPeriodInput, $"{initialPeriod}");
            CommonActions.ElementAddValue(Objects.Ordering.CommencementDate.MaximumTermInput, $"{maximumTerm}");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var order = await context.Orders.SingleAsync(o => o.Id == CallOffId.Id);

            order.CommencementDate.Should().NotBeNull();
            order.CommencementDate!.Value.Date.Should().Be(date.Date);
            order.InitialPeriod.Should().Be(initialPeriod);
            order.MaximumTerm.Should().Be(maximumTerm);
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.First(o => o.Id == CallOffId.Id);
            order.OrderTriageValue = OrderTriageValue.Over250K;

            context.SaveChanges();
        }
    }
}
