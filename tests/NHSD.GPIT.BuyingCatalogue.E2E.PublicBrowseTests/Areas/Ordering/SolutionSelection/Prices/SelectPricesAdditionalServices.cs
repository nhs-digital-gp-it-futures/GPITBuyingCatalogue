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
    public class SelectPricesAdditionalServices : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90012;
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), CatalogueItemId.ToString() },
        };

        public SelectPricesAdditionalServices(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(PricesController),
                  nameof(PricesController.AdditionalServiceSelectPrice),
                  Parameters)
        {
        }

        [Fact]
        public void SelectPriceAdditionalServices_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().BeEquivalentTo("List price for Additional Service - E2E Multiple Prices Additional Service".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(SelectPriceObjects.SelectPriceRadio).Should().BeTrue();
            CommonActions.ElementIsDisplayed(SelectPriceObjects.PriceTitle(1)).Should().BeTrue();
        }

        [Fact]
        public void SelectPriceAdditionalServices_NoSelection_Error()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(PricesController), nameof(PricesController.AdditionalServiceSelectPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementIsDisplayed(SelectPriceObjects.SelectPriceError).Should().BeTrue();
        }

        [Fact]
        public void SelectPriceAdditionalServices_SelectionMade_ExpectedResult()
        {
            CommonActions.ClickFirstRadio();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(typeof(OrderController), nameof(OrderController.Order)).Should().BeTrue();
        }
    }
}
