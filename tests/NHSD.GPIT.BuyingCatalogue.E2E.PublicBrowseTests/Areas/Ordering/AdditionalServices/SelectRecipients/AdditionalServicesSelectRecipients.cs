using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AdditionalServices
{
    public sealed class AdditionalServicesSelectRecipients
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private static readonly CallOffId CallOffId = new(90007, 1);
        private static readonly string OdsCode = "03F";
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A999");

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(OdsCode), OdsCode }, { nameof(CallOffId), CallOffId.ToString() } };

        // We navigate to the catalogue solution selection page to initialize a session
        // but the initializeAsync will always redirect us to SelectSolutionRecipients
        public AdditionalServicesSelectRecipients(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesSelectRecipient_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsTable)
                .Should()
                .BeTrue();

            var serviceRecipients = MemoryCache.GetServiceRecipients();

            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(serviceRecipients.Count());
        }

        [Fact]
        public void AdditionalServicesSelectRecipient_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AdditionalServicesController),
            nameof(AdditionalServicesController.SelectAdditionalServicePrice)).Should().BeTrue();
        }

        [Fact]
        public async Task AdditionalServicesSelectRecipient_SkipPriceSelection_ClickGoBackLink_ExpectedResult()
        {
            await UpdateSessionToSaySkipPriceSelection();

            NavigateToUrl(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients),
                Parameters);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectAdditionalService)).Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectRecipient_DontSelectRecipients_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsErrorMessage,
                "Error: Select a service recipient").Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectRecipient_SelectAll_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients)).Should().BeTrue();

            CommonActions.AllCheckBoxesSelected().Should().BeTrue();

            CommonActions.ElementTextEqualToo(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton,
                "Deselect all").Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectRecipient_DeselectAll_ExpectedResult()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "selectionMode", "all" },
            };

            NavigateToUrl(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients),
                Parameters,
                queryParameters);

            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients)).Should().BeTrue();

            CommonActions.AllCheckBoxesSelected().Should().BeFalse();

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);

            CommonActions.ElementTextEqualToo(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton,
                "Select all").Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectRecipient_SelectRecipient_ExpectedResult()
        {
            CommonActions.ClickFirstCheckbox();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsDateController),
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate))
                .Should()
                .BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeMemoryCacheHander(OdsCode);

            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.SingleOrDefault(cp => cp.CataloguePriceId == 6);

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = new System.DateTime(2111, 1, 1),
                CatalogueItemName = "E2E Multiple Prices Additional Service",
                CataloguePrice = price,
                SkipPriceSelection = false,
                IsNewSolution = true,
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AdditionalServiceRecipientsController),
                nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }

        private async Task UpdateSessionToSaySkipPriceSelection()
        {
            var model = Session.GetOrderStateFromSession(CallOffId.ToString());

            model.SkipPriceSelection = true;

            await Session.SetOrderStateToSessionAsync(model);
        }
    }
}
