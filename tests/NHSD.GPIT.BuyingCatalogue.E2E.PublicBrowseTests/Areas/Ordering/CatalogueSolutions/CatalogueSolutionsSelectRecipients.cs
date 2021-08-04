using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsSelectRecipients
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private static readonly CallOffId CallOffId = new(90004, 01);
        private static readonly string OdsCode = "03F";
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(OdsCode), OdsCode }, { nameof(CallOffId), CallOffId.ToString() } };

        // We navigate to the catalogue solution selection page to initialize a session
        // but the initializeAsync will always redirect us to SelectSolutionRecipients
        public CatalogueSolutionsSelectRecipients(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipient_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton)
                .Should()
                .BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsTable)
                .Should()
                .BeTrue();

            Factory.GetMemoryCache.TryGetValue(
                $"ServiceRecipients-ODS-{OdsCode}",
                out IEnumerable<ServiceContracts.Models.ServiceRecipient> cachedResults);

            CommonActions.GetNumberOfCheckBoxesDisplayed().Should().Be(cachedResults.Count());
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipient_DontSelectRecipients_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsErrorMessage,
                "Error: Select a service recipient").Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipient_SelectAll_ExpectedResult()
        {
            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients)).Should().BeTrue();

            CommonActions.AllCheckBoxesSelected().Should().BeTrue();

            CommonActions.ElementTextEqualToo(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton,
                "Deselect all").Should().BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipient_DeselectAll_ExpectedResult()
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "selectionMode", "all" },
            };

            NavigateToUrl(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients),
                Parameters,
                queryParameters);

            CommonActions.ClickLinkElement(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients)).Should().BeTrue();

            CommonActions.AllCheckBoxesSelected().Should().BeFalse();

            CommonActions.GetNumberOfSelectedCheckBoxes().Should().Be(0);

            CommonActions.ElementTextEqualToo(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsSelectAllButton,
                "Select all").Should().BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.SingleOrDefault(cp => cp.CataloguePriceId == 1);

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CatalogueItemName = "E2E With Contact Multiple Prices",
                CataloguePrice = price,
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(CatalogueSolutionRecipientsController),
                nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
