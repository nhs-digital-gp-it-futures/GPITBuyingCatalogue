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
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsSelectFlatDeclarativeQuantity
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90004, 01);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public CatalogueSolutionsSelectFlatDeclarativeQuantity(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsSelectFlatDeclarativeQuantity_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectFlatDeclarativeQuantity_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(CatalogueSolutionRecipientsDateController),
              nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate)).Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Enter a quantity")]
        [InlineData("ABC", "Quantity must be a number")]
        [InlineData("0", "Quantity must be greater than zero")]
        public void CatalogueSolutionsSelectFlatDeclarativeQuantity_IncorrectInput_ThrowsError(
            string errorValue,
            string expectedErrorMessage)
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput,
                errorValue);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectFlatDeclarativeQuantity))
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
        public void CatalogueSolutionsSelectFlatDeclarative_CorrectInput_ExpectedResult()
        {
            CommonActions.ElementAddValue(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsSelectFlatDeclarativeAndOnDemandQuantityInput,
                "123");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditSolution))
                .Should()
                .BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeServiceRecipientMemoryCacheHandler(InternalOrgId);

            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.SingleOrDefault(cp => cp.CataloguePriceId == 3);

            var firstServiceRecipient = MemoryCache.GetServiceRecipients().FirstOrDefault();

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = new System.DateTime(2111, 1, 1),
                CatalogueItemName = "E2E With Contact Multiple Prices",
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
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectFlatDeclarativeQuantity),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
