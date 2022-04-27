using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsSelectSolution
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90006, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public CatalogueSolutionsSelectSolution(LocalWebApplicationFactory factory)
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
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtons).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectSolution_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(CatalogueSolutionsController),
            nameof(CatalogueSolutionsController.Index))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectSolution_DontSelectSolution_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolution)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.SelectCatalogueSolutionErrorMessage, "Error: Select a Catalogue Solution")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectSolution_SelectSolution_MultiplePrices_ExpectedResult()
        {
            InitializeTestSession();

            const string expectedCatalogueItemName = "E2E With Contact Multiple Prices";

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

            const string expectedCatalogueItemName = "E2E With Contact With Single Price";

            var expectedCatalogueItemId = new CatalogueItemId(99998, "002");

            CommonActions.ClickRadioButtonWithText(expectedCatalogueItemName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution))
                .Should()
                .BeTrue();
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }

        public Task InitializeAsync() => Task.CompletedTask;

        private void InitializeTestSession()
        {
            InitializeSessionHandler();
        }
    }
}
