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
    public sealed class AdditionalServicesSelectRecipientsDateDeclarative
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string OdsCode = "03F";
        private static readonly CallOffId CallOffId = new(90007, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A999");

        private static readonly Dictionary<string, string> Parameters =
            new() { { nameof(OdsCode), OdsCode }, { nameof(CallOffId), CallOffId.ToString() } };

        public AdditionalServicesSelectRecipientsDateDeclarative(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesSelectRecipientsDate_PatientDeclarative_Expected()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectFlatDeclarativeQuantity))
                .Should()
                .BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeServiceRecipientMemoryCacheHandler(OdsCode);

            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.SingleOrDefault(cp => cp.CataloguePriceId == 8);

            var firstServiceRecipient = MemoryCache.GetServiceRecipients().FirstOrDefault();

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = new System.DateTime(2111, 1, 1),
                CatalogueItemName = "E2E Multiple Prices Additional Service",
                CataloguePrice = price,
                ServiceRecipients =
                new List<OrderItemRecipientModel>
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
