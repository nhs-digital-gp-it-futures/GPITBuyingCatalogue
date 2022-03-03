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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AdditionalServices
{
    public sealed class AdditionalServicesSelectPrice
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "03F";
        private static readonly CallOffId CallOffId = new(90007, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() } };

        public AdditionalServicesSelectPrice(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public async Task AdditionalServicesSelectPrice_AllSectionsDisplayed()
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
        public void AdditionalServicesSelectPrice_ClickGoBackLink_ExpectedResults()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(AdditionalServicesController),
              nameof(AdditionalServicesController.SelectAdditionalService)).Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectPrice_DontSelectPrice_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalServicePrice)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions
                .ElementShowingCorrectErrorMessage(Objects.Ordering.AdditionalServices.SelectAdditionalServicePriceErrorMessage, "Error: Select a price")
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("£999.9999 per test patient per year")]
        [InlineData("£999.9999 per test declarative per year")]
        [InlineData("£999.9999 per test on demand per year")]
        public void AdditionalServicesSelectPrice_SelectPrice_ExpectedResult(string priceSelection)
        {
            CommonActions.ClickRadioButtonWithText(priceSelection);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients)).Should().BeTrue();

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
                CatalogueItemName = "E2E Multiple Prices Additional Service",
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectAdditionalServicePrice),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
