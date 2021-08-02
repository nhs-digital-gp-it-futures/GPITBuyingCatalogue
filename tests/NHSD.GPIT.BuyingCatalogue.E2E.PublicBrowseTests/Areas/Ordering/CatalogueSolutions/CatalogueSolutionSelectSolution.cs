using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionSelectSolution
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly CallOffId CallOffId = new(90004, 01);

        private static readonly Dictionary<string, string> Parameters =
            new() { { "OdsCode", "03F" }, { nameof(CallOffId), CallOffId.ToString() } };

        public CatalogueSolutionSelectSolution(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsSelectSolution_AllSectionsDisplayed()
        {
            InitializeTestSession();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtons).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectSolution_DontSelectSolution_ThrowsError()
        {
            InitializeTestSession();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolution)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.SelectCatalogueSolutionErrorMessage, "Error: Select a Catalogue Solution")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectSolution_SelectSolution_MultiplePrices_ExpectedResult()
        {
            InitializeTestSession();

            var expectedCatalogueItemName = "E2E With Contact Multiple Prices";

            var expectedCatalogueItemId = new CatalogueItemId(99998, "001");

            CommonActions.ClickRadioButtonWithText(expectedCatalogueItemName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolutionPrice)).Should().BeTrue();

            CreateOrderItemModel cacheModel = Session.GetOrderStateFromSession(CallOffId.ToString());

            cacheModel.CallOffId.Should().Be(CallOffId);
            cacheModel.CatalogueItemId.Should().Be(expectedCatalogueItemId);
            cacheModel.CatalogueItemName.Should().BeEquivalentTo(expectedCatalogueItemName);
        }

        [Fact]
        public void CatalogueSolutionsSelectSolution_SelectSolution_SingePrice_ExpectedResult()
        {
            InitializeTestSession();

            var expectedCatalogueItemName = "E2E With Contact With Single Price";

            var expectedCatalogueItemId = new CatalogueItemId(99998, "002");

            CommonActions.ClickRadioButtonWithText(expectedCatalogueItemName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients)).Should().BeTrue();

            CreateOrderItemModel cacheModel = Session.GetOrderStateFromSession(CallOffId.ToString());

            cacheModel.CallOffId.Should().Be(CallOffId);
            cacheModel.CatalogueItemId.Should().Be(expectedCatalogueItemId);
            cacheModel.CatalogueItemName.Should().BeEquivalentTo(expectedCatalogueItemName);
            cacheModel.SkipPriceSelection.Should().BeTrue();
            cacheModel.AgreedPrice.Should().NotBeNull();
        }

        private void InitializeTestSession()
        {
            InitializeSession();
        }
    }
}
