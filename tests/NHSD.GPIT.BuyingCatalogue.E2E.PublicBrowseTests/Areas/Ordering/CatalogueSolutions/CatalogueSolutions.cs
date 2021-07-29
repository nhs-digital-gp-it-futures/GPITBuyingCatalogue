using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutions
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CallOffId CallOffId = new(90004, 1);

        private static readonly Dictionary<string, string> Parameters =
            new() { { "OdsCode", "03F" }, { "CallOffId", CallOffId.ToString() } };

        public CatalogueSolutions(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.Index),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutions_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddItemButton("Catalogue Solution")).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.AddedItemsTable).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Ordering.CatalogueItems.ContinueButton).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_ClickAddSolution()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.AddItemButton("Catalogue Solution"));

            CommonActions.PageLoadedCorrectGetIndex(typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.SelectSolution)).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutions_ClickContinue()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueItems.ContinueButton);

            CommonActions.PageLoadedCorrectGetIndex(typeof(OrderController), nameof(OrderController.Order));
        }
    }
}
