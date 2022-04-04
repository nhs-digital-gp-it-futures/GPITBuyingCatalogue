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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AssociatedServices
{
    public sealed class AssociatedServicesSelectPrice
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90008, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "S-997");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public AssociatedServicesSelectPrice(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(AssociatedServicesController),
              nameof(AssociatedServicesController.SelectAssociatedService),
              Parameters)
        {
        }

        [Fact]
        public async Task AssociatedServicesSelectPrice_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtonItems).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var numberOfPrices = await context.CataloguePrices.Where(cp => cp.CatalogueItemId == CatalogueItemId && cp.PublishedStatus == EntityFramework.Catalogue.Models.PublicationStatus.Published).CountAsync();

            CommonActions.GetNumberOfRadioButtonsDisplayed().Should().Be(numberOfPrices);
        }

        [Fact]
        public void AssociatedServicesSelectPrice_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AssociatedServicesController),
            nameof(AssociatedServicesController.SelectAssociatedService)).Should().BeTrue();
        }

        [Fact]
        public void AssociatedServicesSelectPrice_DontSelectPrice_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                  typeof(AssociatedServicesController),
                  nameof(AssociatedServicesController.SelectAssociatedServicePrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(
                Objects.Ordering.AssociatedServices.SelectAssociatedServicePriceErrorMessage,
                "Error: Select a price")
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("£999.9999 per test patient per year")]
        [InlineData("£999.9999 per test declarative per year")]
        [InlineData("£999.9999 per test on demand per year")]
        public void AssociatedServicesSelectPrice_SelectPrice_ExpectedResult(string priceSelection)
        {
            CommonActions.ClickRadioButtonWithText(priceSelection);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService)).Should().BeTrue();

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
                CatalogueItemName = "E2E Multiple Prices Associated Service",
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServicePrice),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
