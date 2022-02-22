using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class FundingSource
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private const string InternalOrgId = "03F";
        private static readonly CallOffId CallOffId = new(90008, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public FundingSource(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(FundingSourceController),
              nameof(FundingSourceController.FundingSource),
              Parameters)
        {
        }

        [Fact]
        public void FundingSource_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be($"Confirm funding source - Order {CallOffId}".FormatForComparison());
            CommonActions.LedeText().Should().Be("Confirm the funding source you're using to pay for this order.".FormatForComparison());
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(OrderTriageObjects.FundingSource).Should().BeTrue();
            CommonActions.ElementIsDisplayed(OrderTriageObjects.FundingInset).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
        }

        [Fact]
        public void FundingSource_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void FundingSource_DontSelectFundingSource_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions
            .PageLoadedCorrectGetIndex(
              typeof(FundingSourceController),
              nameof(FundingSourceController.FundingSource))
            .Should()
            .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                OrderTriageObjects.FundingSourceError,
                "Error: Select a funding source")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void FundingSource_SelectFundingSource_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Local funding");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var order = context.Orders.Single(o => o.Id == CallOffId.Id);
            order.ConfirmedFundingSource.Should().Be(true);
        }

        [Fact]
        public void FundingSource_Preselected_PrepopulatesFundingSource()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders.Single(o => o.Id == CallOffId.Id);
            order.FundingSourceOnlyGms = true;

            context.SaveChanges();

            Driver.Navigate().Refresh();

            CommonActions.IsRadioButtonChecked(ServiceContracts.Enums.FundingSource.Central.ToString()).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders
                .Single(o => o.Id == CallOffId.Id);

            order.FundingSourceOnlyGms = null;
            order.ConfirmedFundingSource = null;

            context.SaveChanges();
        }
    }
}
