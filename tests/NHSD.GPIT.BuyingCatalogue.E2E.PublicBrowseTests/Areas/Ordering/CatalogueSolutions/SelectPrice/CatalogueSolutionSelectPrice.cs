using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionSelectPrice
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private static readonly CallOffId CallOffId = new(90004, 01);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new() { { "OdsCode", "03F" }, { nameof(CallOffId), CallOffId.ToString() } };

        // We navigate to the catalogue solution selection page to initialize a session
        // but the InitializeAsync will always redirect us to SelectSolutionPrice
        public CatalogueSolutionSelectPrice(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public async Task CatalogueSolutionsSelectPrice_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var numberOfPrices = await context.CataloguePrices.Where(cp => cp.CatalogueItemId == CatalogueItemId).CountAsync();

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Equals(numberOfPrices);
        }

        [Fact]
        public void CatalogueSolutionsSelectPrice_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(CatalogueSolutionsController),
              nameof(CatalogueSolutionsController.SelectSolution)).Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectPrice_DontSelectPrice_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolutionPrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(Objects.Ordering.CatalogueSolutions.SelectCatalogueSolutionPriceErrorMessage, "Error: Select a price")
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("£999.9999 per test patient per year")]
        [InlineData("£999.9999 per test declarative per year")]
        [InlineData("£999.9999 per test on demand per year")]
        public void CatalogueSolutionsSelectPrice_SelectPrice_ExpectedResult(string priceSelection)
        {
            CommonActions.ClickRadioButtonWithText(priceSelection);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients)).Should().BeTrue();

            var model = Session.GetOrderStateFromSession(CallOffId.ToString());

            model.AgreedPrice.Should().NotBeNull();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CatalogueItemName = "E2E With Contact Multiple Prices",
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectSolutionPrice),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
