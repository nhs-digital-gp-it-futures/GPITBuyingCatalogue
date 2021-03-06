using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices.Base
{
    public abstract class SelectPrice : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        protected SelectPrice(LocalWebApplicationFactory factory, Dictionary<string, string> parameters)
            : base(factory, typeof(PricesController), nameof(PricesController.SelectPrice), parameters)
        {
        }

        protected abstract string PageTitle { get; }

        [Fact]
        public void SelectPrice_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo(PageTitle.FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SelectPriceObjects.SelectPriceRadio).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SelectPriceObjects.PriceTitle(1)).Should().BeTrue();
        }

        [Fact]
        public void SelectPrice_NoSelection_Error()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.SelectPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(SelectPriceObjects.SelectPriceError).Should().BeTrue();
        }

        [Fact]
        public void SelectPrice_SelectionMade_ExpectedResult()
        {
            CommonActions.ClickFirstRadio();
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(PricesController),
                nameof(PricesController.ConfirmPrice)).Should().BeTrue();
        }
    }
}
