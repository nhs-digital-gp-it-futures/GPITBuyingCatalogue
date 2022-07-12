using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices
{
    public class ConfirmPriceAdditionalService : ConfirmPrice
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90007;
        private const int PriceId = 10;
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrderId), $"{OrderId}" },
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
            { nameof(PriceId), $"{PriceId}" },
        };

        public ConfirmPriceAdditionalService(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override decimal ListPrice => 999.9999M;

        protected override string PageTitle => "Price of Additional Service - E2E Multiple Prices Additional Service";
    }
}
