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

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.CatalogueSolutions
{
    public sealed class CatalogueSolutionsSelectRecipientsDateOnDemand
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

        public CatalogueSolutionsSelectRecipientsDateOnDemand(
            LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SelectSolution),
                  Parameters)
        {
        }

        [Fact]
        public void CatalogueSolutionsSelectRecipientsDate_PatientOnDemand_Expected()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.SelectFlatOnDemandQuantity))
                .Should()
                .BeTrue();
        }

        public Task InitializeAsync()
        {
            InitializeSessionHandler();

            InitializeServiceRecipientMemoryCacheHandler(InternalOrgId);

            using var context = GetEndToEndDbContext();
            var price = context.CataloguePrices.SingleOrDefault(cp => cp.CataloguePriceId == 2);

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
