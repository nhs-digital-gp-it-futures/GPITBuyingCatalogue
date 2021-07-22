using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class CommencementDate
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string RealDateErrorMessage = "Error: Commencement date must be a real date";
        private const string MustBeInFutureErrorMessage = "Error: Commencement date must be in the future or within the last 60 days";

        private static readonly CallOffId CallOffId = new(90003, 1);

        private static readonly Dictionary<string, string> Parameters = new() { { "OdsCode", "03F" }, { "CallOffId", CallOffId.ToString() } };

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
            CommonActions.PageTitle()
                .Should()
                .BeEquivalentTo($"Commencement date for {CallOffId}".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.CommencementDateDayInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CommencementDate.CommencementDateYearInput).Should().BeTrue();
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

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                RealDateErrorMessage).Should().BeTrue();
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

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                RealDateErrorMessage).Should().BeTrue();
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

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                RealDateErrorMessage).Should().BeTrue();
        }

        [Fact]
        public void CommencementDate_OnlyYear_ThrowsError()
        {
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateDayInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput).Clear();
            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).Clear();

            Driver.FindElement(Objects.Ordering.CommencementDate.CommencementDateYearInput).SendKeys(DateTime.UtcNow.Year.ToString());

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                RealDateErrorMessage).Should().BeTrue();
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

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage,
                MustBeInFutureErrorMessage).Should().BeTrue();
        }

        [Fact]
        public async Task CommencementDate_InputDate_AddsCommencementDate()
        {
            var date = TextGenerators.DateInputAddDateSoon(
                Objects.Ordering.CommencementDate.CommencementDateDayInput,
                Objects.Ordering.CommencementDate.CommencementDateMonthInput,
                Objects.Ordering.CommencementDate.CommencementDateYearInput);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var order = await context.Orders
                .SingleAsync(o => o.Id == CallOffId.Id);

            order.CommencementDate.Should().NotBeNull();
            order.CommencementDate.Value.Date.Should().Be(date.Date);
        }
    }
}
