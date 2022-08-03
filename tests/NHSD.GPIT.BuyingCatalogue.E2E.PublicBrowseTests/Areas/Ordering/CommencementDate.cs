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
        public void CommencementDate_NoInput_ThrowsErrors()
        {
            CommonActions.ClearInputElement(Objects.Ordering.CommencementDate.CommencementDateDayInput);
            CommonActions.ClearInputElement(Objects.Ordering.CommencementDate.CommencementDateMonthInput);
            CommonActions.ClearInputElement(Objects.Ordering.CommencementDate.CommencementDateYearInput);
            CommonActions.ClearInputElement(Objects.Ordering.CommencementDate.InitialPeriodInput);
            CommonActions.ClearInputElement(Objects.Ordering.CommencementDate.MaximumTermInput);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                Objects.Ordering.CommencementDate.CommencementDateErrorMessage).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                Objects.Ordering.CommencementDate.InitialPeriodError).Should().BeTrue();

            CommonActions.ElementIsDisplayed(
                Objects.Ordering.CommencementDate.MaximumTermError).Should().BeTrue();
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
