﻿using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Tiered;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Solution.Tiered
{
    public sealed class TieredPriceTiers : TieredPriceTiersBase
    {
        private const int PriceId = 4;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(CataloguePriceId), PriceId.ToString() },
        };

        public TieredPriceTiers(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(CatalogueSolutionListPriceController),
                Parameters)
        {
        }

        protected override CatalogueItemId CatalogueItemId => SolutionId;

        protected override int CataloguePriceId => PriceId;

        protected override Type Controller => typeof(CatalogueSolutionListPriceController);
    }
}
