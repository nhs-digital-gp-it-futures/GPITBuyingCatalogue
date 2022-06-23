using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Tiered;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution.Tiered
{
    public sealed class EditTierPrice : EditTierPriceBase
    {
        private const int CataloguePriceId = 4;
        private const int TierId = 3;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(CataloguePriceId), CataloguePriceId.ToString() },
            { nameof(TierId), TierId.ToString() },
        };

        public EditTierPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionListPriceController),
                Parameters)
        {
        }

        protected override Type Controller => typeof(CatalogueSolutionListPriceController);

        protected override CatalogueItemId CatalogueItemId => SolutionId;
    }
}
