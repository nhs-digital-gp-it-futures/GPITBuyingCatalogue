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
    public sealed class AdditionalServicesSelectRecipientsDate
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "CG-03F";
        private static readonly CallOffId CallOffId = new(90007, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(InternalOrgId), InternalOrgId }, { nameof(CallOffId), CallOffId.ToString() } };

        public AdditionalServicesSelectRecipientsDate(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesSelectRecipientDate_AllSectionsDisplayed()
        {
            CommonActions.SaveButtonDisplayed().Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions
                .ElementIsDisplayed(Objects.Ordering.CatalogueSolutions.CatalogueSolutionsRecipientsDate)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectRecipientDate_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
            typeof(AdditionalServiceRecipientsController),
            nameof(AdditionalServiceRecipientsController.SelectAdditionalServiceRecipients))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectRecipientDate_DateInputEmpty_ThrowsError()
        {
            CommonActions.ClearInputElement(CommonSelectors.DateDay);
            CommonActions.ClearInputElement(CommonSelectors.DateMonth);
            CommonActions.ClearInputElement(CommonSelectors.DateYear);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServiceRecipientsDateController),
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Ordering.AdditionalServices.AdditionalServiceRecipientsDateErrorMessage,
                "Error: Planned delivery date must be a real date")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AdditionalServicesSelectRecipientDate_DateInputLoaded_HasExpectedValues()
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
        public void AdditionalServicesSelectRecipientsDate_PatientPrice_Expected()
        {
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
            var price = context.CataloguePrices
                .Include(cp => cp.PricingUnit)
                .SingleOrDefault(cp => cp.CataloguePriceId == 6);

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
                SkipPriceSelection = false,
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AdditionalServiceRecipientsDateController),
                nameof(AdditionalServiceRecipientsDateController.SelectAdditionalServiceRecipientsDate),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
