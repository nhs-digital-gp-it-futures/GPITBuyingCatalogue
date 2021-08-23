using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AdditionalServices
{
    public sealed class AdditionalServicesSelectService
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private static readonly CallOffId CallOffId = new(90007, 1);

        private static readonly Dictionary<string, string> Parameters =
            new() { { "OdsCode", "03F" }, { nameof(CallOffId), CallOffId.ToString() } };

        public AdditionalServicesSelectService(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesSelectService_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtons).Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectService_ClickGoBackButton_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectService_DontSelectSolution_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectAdditionalService)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.AdditionalServices.SelectAdditionalServiceErrorMessage, "Error: Select an Additional Service")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectService_SelectSolution_MultiplePrices_ExpectedResult()
        {
            InitializeTestSession();

            const string expectedCatalogueItemName = "E2E Multiple Prices Additional Service";

            var expectedCatalogueItemId = new CatalogueItemId(99998, "001A999");

            CommonActions.ClickRadioButtonWithText(expectedCatalogueItemName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectAdditionalServicePrice)).Should().BeTrue();

            CreateOrderItemModel cacheModel = Session.GetOrderStateFromSession(CallOffId.ToString());

            cacheModel.CallOffId.Should().Be(CallOffId);
            cacheModel.CatalogueItemId.Should().Be(expectedCatalogueItemId);
            cacheModel.CatalogueItemName.Should().BeEquivalentTo(expectedCatalogueItemName);
        }

        [Fact]
        public void AdditionalServicesSelectService_SelectSolution_SingePrice_ExpectedResult()
        {
            InitializeTestSession();

            const string expectedCatalogueItemName = "E2E No Contact Single Price Additional Service";

            var expectedCatalogueItemId = new CatalogueItemId(99998, "002A999");

            CommonActions.ClickRadioButtonWithText(expectedCatalogueItemName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
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
