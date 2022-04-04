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
    public sealed class AdditionalServicesSelectFlatDeclarativeQuantity
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90004, 01);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() } };

        public AdditionalServicesSelectFlatDeclarativeQuantity(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesSelectFlatDeclarativeQuantity_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectFlatDeclarativeQuantity_ClickGoBackButton_ExpectedResult()
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
        public void AdditionalServicesSelectFlatDeclarativeQuantity_IncorrectInput_ThrowsError(
            string errorValue,
            string expectedErrorMessage)
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput,
                errorValue);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectFlatDeclarativeQuantity))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInputErrorMessage,
                expectedErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectFlatDeclarativeQuantity_CorrectInput_ExpectedResult()
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput,
                "123");

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

            InitializeServiceRecipientMemoryCacheHandler(InternalOrgId);

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
                nameof(AdditionalServicesController.SelectFlatDeclarativeQuantity),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
