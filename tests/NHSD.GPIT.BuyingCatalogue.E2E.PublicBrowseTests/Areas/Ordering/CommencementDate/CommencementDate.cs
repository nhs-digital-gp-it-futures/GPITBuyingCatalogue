using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CommencementDate
{
    [Collection(nameof(OrderingCollection))]
    public sealed class CommencementDate : BuyerTestBase, IDisposable
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

            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateReadOnlyDisplay).Should().BeFalse();
            CommonActions.ContinueButtonDisplayed().Should().BeFalse();

            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateDayInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateMonthInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.CommencementDateYearInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.InitialPeriodInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommencementDateObjects.MaximumTermInput).Should().BeTrue();
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
            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateYearInput).Clear();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.DayMissingErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_OnlyDay_ThrowsError()
        {
            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateYearInput).Clear();

            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).SendKeys("1");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.MonthMissingErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_OnlyDayAndMonth_ThrowsError()
        {
            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateYearInput).Clear();

            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).SendKeys("1");
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).SendKeys("1");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.YearMissingErrorMessage}").Should().BeTrue();
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        public void CommencementDate_YearTooShort_ThrowsError(string year)
        {
            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateYearInput).Clear();

            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).SendKeys("1");
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).SendKeys("1");
            Driver.FindElement(CommencementDateObjects.CommencementDateYearInput).SendKeys(year);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.YearWrongLengthErrorMessage}").Should().BeTrue();
        }

        [Theory]
        [InlineData("99", "01", "2000")]
        [InlineData("01", "99", "2000")]
        public void CommencementDate_InvalidDate_ThrowsError(string day, string month, string year)
        {
            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).Clear();
            Driver.FindElement(CommencementDateObjects.CommencementDateYearInput).Clear();

            Driver.FindElement(CommencementDateObjects.CommencementDateDayInput).SendKeys(day);
            Driver.FindElement(CommencementDateObjects.CommencementDateMonthInput).SendKeys(month);
            Driver.FindElement(CommencementDateObjects.CommencementDateYearInput).SendKeys(year);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.CommencementDateErrorMessage,
                $"Error:{DateInputModelValidator.DateInvalidErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_DateInThePast_ThrowsError()
        {
            CommonActions.ElementAddValue(CommencementDateObjects.CommencementDateDayInput, "1");
            CommonActions.ElementAddValue(CommencementDateObjects.CommencementDateMonthInput, "1");
            CommonActions.ElementAddValue(CommencementDateObjects.CommencementDateYearInput, "2000");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.CommencementDateErrorMessage,
                $"Error:{CommencementDateModelValidator.CommencementDateInThePastErrorMessage}").Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_InitialPeriodAndMaximumTermMissing_ThrowsError()
        {
            CommonActions.ElementAddValue(CommencementDateObjects.InitialPeriodInput, string.Empty);
            CommonActions.ElementAddValue(CommencementDateObjects.MaximumTermInput, string.Empty);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodMissingErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.MaximumTermError,
                CommencementDateModelValidator.MaximumTermMissingErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_InitialPeriodAndMaximumTermNonNumeric_ThrowsError()
        {
            CommonActions.ElementAddValue(CommencementDateObjects.InitialPeriodInput, "A");
            CommonActions.ElementAddValue(CommencementDateObjects.MaximumTermInput, "B");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodNotANumberErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.MaximumTermError,
                CommencementDateModelValidator.MaximumTermNotANumberErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_InitialPeriodAndMaximumTermTooLow_ThrowsError()
        {
            CommonActions.ElementAddValue(CommencementDateObjects.InitialPeriodInput, "0");
            CommonActions.ElementAddValue(CommencementDateObjects.MaximumTermInput, "0");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.InitialPeriodError,
                CommencementDateModelValidator.InitialPeriodTooLowErrorMessage).Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.MaximumTermError,
                CommencementDateModelValidator.MaximumTermTooLowErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_MaximumTermInvalid_ThrowsError()
        {
            CommonActions.ElementAddValue(
                CommencementDateObjects.InitialPeriodInput,
                "6");

            CommonActions.ElementAddValue(
                CommencementDateObjects.MaximumTermInput,
                "5");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                CommencementDateObjects.MaximumTermError,
                CommencementDateModelValidator.DurationInvalidErrorMessage).Should().BeTrue();
        }

        [Fact]
        public async Task CommencementDate_EverythingOk_AddsCommencementDate()
        {
            const int initialPeriod = 3;
            const int maximumTerm = 12;

            var date = TextGenerators.DateInputAddDateSoon(
                CommencementDateObjects.CommencementDateDayInput,
                CommencementDateObjects.CommencementDateMonthInput,
                CommencementDateObjects.CommencementDateYearInput);

            CommonActions.ElementAddValue(CommencementDateObjects.InitialPeriodInput, $"{initialPeriod}");
            CommonActions.ElementAddValue(CommencementDateObjects.MaximumTermInput, $"{maximumTerm}");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();

            var order = await context.Orders.FirstAsync(o => o.OrderNumber == CallOffId.OrderNumber && o.Revision == CallOffId.Revision);

            order.CommencementDate.Should().NotBeNull();
            order.CommencementDate!.Value.Date.Should().Be(date.Date);
            order.InitialPeriod.Should().Be(initialPeriod);
            order.MaximumTerm.Should().Be(maximumTerm);
        }

        [Fact]
        public void CommencementDate_NewDateAfterPlannedDeliveryDates_RedirectsToConfirmChanges()
        {
            var callOffId = new CallOffId(90022, 1);

            var parameters = new Dictionary<string, string>
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), $"{callOffId}" },
            };

            NavigateToUrl(typeof(CommencementDateController), nameof(CommencementDateController.CommencementDate), parameters);

            var nextWeek = DateTime.Today.AddDays(7);

            CommonActions.ElementAddValue(CommencementDateObjects.CommencementDateDayInput, $"{nextWeek.Day}");
            CommonActions.ElementAddValue(CommencementDateObjects.CommencementDateMonthInput, $"{nextWeek.Month}");
            CommonActions.ElementAddValue(CommencementDateObjects.CommencementDateYearInput, $"{nextWeek.Year}");
            CommonActions.ElementAddValue(CommencementDateObjects.InitialPeriodInput, "3");
            CommonActions.ElementAddValue(CommencementDateObjects.MaximumTermInput, "12");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.ConfirmChanges)).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();

            var order = context.Order(CallOffId).Result;

            context.SaveChanges();
        }
    }
}
