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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsSelectRecipientsDate
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "03F";
        private static readonly CallOffId CallOffId = new(90004, 01);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
            };

        public CatalogueSolutionsSelectRecipientsDate(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipientDate_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsDate)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipientDate_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(CatalogueSolutionRecipientsController),
            nameof(CatalogueSolutionRecipientsController.SelectSolutionServiceRecipients))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipientDate_DateInputEmpty_ThrowsError()
        {
            CommonActions.ClearInputElement(CommonSelectors.DateDay);
            CommonActions.ClearInputElement(CommonSelectors.DateMonth);
            CommonActions.ClearInputElement(CommonSelectors.DateYear);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionRecipientsDateController),
                nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsDateErrorMessage,
                "Error: Planned delivery date must be a real date")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipientDate_DateInputLoaded_HasExpectedValues()
        {
            var model = Session.GetOrderStateFromSession(CallOffId.ToString());

            CommonActions.InputValueEqualTo(
                CommonSelectors.DateDay,
                model.CommencementDate.Value.Day.ToString("00"))
                .Should()
                .BeTrue();

            CommonActions.InputValueEqualTo(
                CommonSelectors.DateMonth,
                model.CommencementDate.Value.Month.ToString("00"))
                .Should()
                .BeTrue();

            CommonActions.InputValueEqualTo(
                CommonSelectors.DateYear,
                model.CommencementDate.Value.Year.ToString("0000"))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipientsDate_PatientPrice_Expected()
        {
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
            var price = context.CataloguePrices.SingleOrDefault(cp => cp.CataloguePriceId == 1);

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
                typeof(CatalogueSolutionRecipientsDateController),
                nameof(CatalogueSolutionRecipientsDateController.SelectSolutionServiceRecipientsDate),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
