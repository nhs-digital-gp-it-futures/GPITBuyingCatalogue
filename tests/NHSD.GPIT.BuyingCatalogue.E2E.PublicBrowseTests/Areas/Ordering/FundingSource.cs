using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering
{
    public sealed class FundingSource
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CallOffId CallOffId = new(90008, 1);
        private static readonly string OdsCode = "03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(OdsCode), OdsCode },
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
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
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
                Objects.Ordering.FundingSource.FundingSourceErrorMessage,
                "Error: Select yes if you're paying for this order in full using your GP IT Futures centrally held funding allocation")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void FundingSource_SelectFundingSource_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderController),
                nameof(OrderController.Order)).Should().BeTrue();
        }

        public void Dispose()
        {
            using var context = GetEndToEndDbContext();
            var order = context.Orders
                .Single(o => o.Id == CallOffId.Id);

            order.FundingSourceOnlyGms = null;

            context.SaveChanges();
        }
    }
}
