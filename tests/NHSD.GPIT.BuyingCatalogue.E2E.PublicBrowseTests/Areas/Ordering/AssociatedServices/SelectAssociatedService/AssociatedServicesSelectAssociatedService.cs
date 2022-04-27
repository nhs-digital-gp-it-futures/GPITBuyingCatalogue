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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AssociatedServices
{
    public sealed class AssociatedServicesSelectAssociatedService
                : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90008, 1);

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { "CallOffId", CallOffId.ToString() },
            };

        public AssociatedServicesSelectAssociatedService(LocalWebApplicationFactory factory)
        : base(
              factory,
              typeof(AssociatedServicesController),
              nameof(AssociatedServicesController.SelectAssociatedService),
              Parameters)
        {
        }

        [Fact]
        public void AssociatedServicesSelectAssociatedService_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtons).Should().BeTrue();
        }

        [Fact]
        public void AssociatedServicesSelectAssociatedService_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AssociatedServicesController),
            nameof(AssociatedServicesController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AssociatedServicesSelectAssociatedService_DontSelectService_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedService)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.AssociatedServices.SelectAssociatedServiceErrorMessage, "Error: Select an Associated Service")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AssociatedServicesSelectAssociatedService_SelectService_MultiplePrices_ExpectedResult()
        {
            InitializeTestSession();

            const string expectedCatalogueItemName = "E2E Multiple Prices Associated Service";

            var expectedCatalogueItemId = new CatalogueItemId(99998, "S-997");

            CommonActions.ClickRadioButtonWithText(expectedCatalogueItemName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.SelectAssociatedServicePrice)).Should().BeTrue();

            CreateOrderItemModel cacheModel = Session.GetOrderStateFromSession(CallOffId.ToString());

            cacheModel.CallOffId.Should().Be(CallOffId);
            cacheModel.CatalogueItemId.Should().Be(expectedCatalogueItemId);
            cacheModel.CatalogueItemName.Should().BeEquivalentTo(expectedCatalogueItemName);
        }

        [Fact]
        public void AssociatedServicesSelectAssociatedService_SelectService_SingePrice_ExpectedResult()
        {
            InitializeTestSession();

            const string expectedCatalogueItemName = "E2E Single Price Associated Service";

            var expectedCatalogueItemId = new CatalogueItemId(99998, "S-998");

            CommonActions.ClickRadioButtonWithText(expectedCatalogueItemName);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AssociatedServicesController),
                nameof(AssociatedServicesController.EditAssociatedService))
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
