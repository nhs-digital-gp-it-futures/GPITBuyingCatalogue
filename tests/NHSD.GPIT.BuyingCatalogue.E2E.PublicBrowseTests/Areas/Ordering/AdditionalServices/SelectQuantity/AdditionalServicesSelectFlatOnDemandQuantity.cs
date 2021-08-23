using System.Collections.Generic;
using System.Linq;
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
    public sealed class AdditionalServicesSelectFlatOnDemandQuantity
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string OdsCode = "03F";
        private static readonly CallOffId CallOffId = new(90004, 01);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(OdsCode), OdsCode }, { nameof(CallOffId), CallOffId.ToString() } };

        public AdditionalServicesSelectFlatOnDemandQuantity(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesSelectFlatOnDemandQuantity_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput)
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(CommonSelectors.RadioButtons)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectFlatOnDemandQuantity_ClickGoBackButton_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AdditionalServiceRecipientsDateController),
            nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate))
            .Should()
            .BeTrue();
        }

        [Theory]
        [InlineData("", "Enter a quantity")]
        [InlineData("ABC", "Quantity must be a number")]
        [InlineData("0", "Quantity must be greater than zero")]
        public void AdditionalServicesSelectFlatOnDemandQuantity_IncorrectInput_ThrowsError(
            string errorValue,
            string expectedErrorMessage)
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput,
                errorValue);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectFlatOnDemandQuantity))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInputErrorMessage,
                expectedErrorMessage)
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatOnDemandRadioInputErrorMessage,
                "Error: Time Unit is required")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectFlatOnDemandQuantity_CorrectInput_ExpectedResult()
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput,
                "123");

            CommonActions.ClickRadioButtonWithText("per month");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService))
                .Should()
                .BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeMemoryCacheHander(OdsCode);

            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.SingleOrDefault(cp => cp.CataloguePriceId == 7);

            var firstServiceRecipient = MemoryCache.GetServiceRecipients().FirstOrDefault();

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = new System.DateTime(2111, 1, 1),
                CatalogueItemName = "E2E Multiple Prices Additional Service",
                CataloguePrice = price,
                ServiceRecipients = new()
                {
                    new()
                    {
                        Name = firstServiceRecipient.Name,
                        OdsCode = firstServiceRecipient.OrgId,
                    },
                },
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectFlatOnDemandQuantity),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
