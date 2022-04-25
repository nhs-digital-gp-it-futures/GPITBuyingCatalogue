using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices
{
    public class SelectPrices : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90012;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public SelectPrices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(PricesController),
                  nameof(PricesController.SelectPrice),
                  Parameters)
        {
        }

        [Fact]
        public void SelectPrice_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("List price for Catalogue Solution - E2E With Contact Multiple Prices".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SelectPriceObjects.SelectPriceRadio).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SelectPriceObjects.PriceTitle(1)).Should().BeTrue();
        }

        [Fact]
        public void SelectPrice_NoSelection_Error()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(PricesController), nameof(PricesController.SelectPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(SelectPriceObjects.SelectPriceError).Should().BeTrue();
        }

        [Fact]
        public void SelectPrice_SelectionMade_ExpectedResult()
        {
            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(OrderController), nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
