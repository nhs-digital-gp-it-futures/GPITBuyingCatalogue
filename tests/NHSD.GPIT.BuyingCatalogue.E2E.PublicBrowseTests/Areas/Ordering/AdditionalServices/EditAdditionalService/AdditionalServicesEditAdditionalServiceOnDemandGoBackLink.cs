using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.AdditionalServices
{
    public sealed class AdditionalServicesEditAdditionalServiceOnDemandGoBackLink
        : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>, IAsyncLifetime
    {
        private const string InternalOrgId = "03F";
        private const string CatalogueItemName = "E2E Multiple Prices Additional Service";
        private static readonly CallOffId CallOffId = new(90007, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
                { nameof(CallOffId), CallOffId.ToString() },
                { nameof(CatalogueItemId), CatalogueItemId.ToString() },
            };

        public AdditionalServicesEditAdditionalServiceOnDemandGoBackLink(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(AdditionalServicesController),
                  nameof(AdditionalServicesController.SelectAdditionalService),
                  Parameters)
        {
        }

        [Fact]
        public void AdditionalServicesEditAdditionalServiceOnDemandGoBackLink_ClickGobackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
            .PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.SelectFlatOnDemandQuantity))
            .Should()
            .BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeServiceRecipientMemoryCacheHandler(InternalOrgId);

            using var context = GetEndToEndDbContext();
            var price = context
                .CataloguePrices
                .Include(cp => cp.PricingUnit)
                .SingleOrDefault(cp => cp.CataloguePriceId == 7);

            var firstServiceRecipient = MemoryCache.GetServiceRecipients().FirstOrDefault();

            var model = new CreateOrderItemModel
            {
                CallOffId = CallOffId,
                CatalogueItemId = CatalogueItemId,
                CommencementDate = DateTime.UtcNow.AddDays(1),
                CatalogueItemName = CatalogueItemName,
                CataloguePrice = price,
                ServiceRecipients = new()
                {
                    new()
                    {
                        Name = firstServiceRecipient.Name,
                        OdsCode = firstServiceRecipient.OrgId,
                        Selected = true,
                        Quantity = 123,
                        DeliveryDate = DateTime.UtcNow.AddDays(2),
                    },
                },
                Quantity = 123,
                EstimationPeriod = EntityFramework.Catalogue.Models.TimeUnit.PerMonth,
                IsNewSolution = true,
                CurrencySymbol = "£",
                AgreedPrice = 100,
            };

            Session.SetOrderStateToSessionAsync(model);

            NavigateToUrl(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditAdditionalService),
                Parameters);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return DisposeSession();
        }
    }
}
