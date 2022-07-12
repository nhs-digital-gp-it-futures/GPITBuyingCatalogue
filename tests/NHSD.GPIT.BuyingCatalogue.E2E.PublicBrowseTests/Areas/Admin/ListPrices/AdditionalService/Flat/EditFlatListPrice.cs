﻿using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.Base.Flat;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ListPrices.AdditionalService.Flat
{
    public sealed class EditFlatListPrice : EditFlatListPriceBase
    {
        private const int PriceId = 10;
        private static readonly CatalogueItemId SolutionId = new(99998, "001");
        private static readonly CatalogueItemId AdditionalServiceId = new(99998, "001A99");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
            { nameof(AdditionalServiceId), AdditionalServiceId.ToString() },
            { nameof(CataloguePriceId), PriceId.ToString() },
        };

        public EditFlatListPrice(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(AdditionalServiceListPriceController),
                Parameters)
        {
        }

        protected override CatalogueItemId CatalogueItemId => AdditionalServiceId;

        protected override int CataloguePriceId => PriceId;

        protected override Type Controller => typeof(AdditionalServiceListPriceController);
    }
}
