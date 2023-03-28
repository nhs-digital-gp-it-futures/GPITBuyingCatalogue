using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.Prices
{
    public class EditPriceCatalogueSolution : EditPrice
    {
        private const string InternalOrgId = "IB-QWO";
        private const int OrderId = 90005;
        private static readonly CallOffId CallOffId = new(OrderId, 1);
        private static readonly CatalogueItemId CatalogueItemId = new(99999, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(OrderId), $"{OrderId}" },
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
            { nameof(CatalogueItemId), $"{CatalogueItemId}" },
        };

        public EditPriceCatalogueSolution(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override decimal ListPrice => 999.9999M;

        protected override string PageTitle => "Price of Catalogue Solution - DFOCVC Solution Full";
    }
}
