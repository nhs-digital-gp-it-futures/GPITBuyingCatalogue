﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public sealed class ManageListPricesModel : NavBaseModel
    {
        public ManageListPricesModel()
        {
        }

        public ManageListPricesModel(CatalogueItem catalogueItem)
        {
            CataloguePrices = catalogueItem.CataloguePrices;
            CatalogueName = catalogueItem.Name;
            CatalogueItemId = catalogueItem.Id;
        }

        public CatalogueItemId CatalogueItemId { get; init; }

        public string CatalogueName { get; init; }

        public ICollection<CataloguePrice> CataloguePrices { get; init; }

        public TaskProgress Status() =>
            CataloguePrices is not null && CataloguePrices.Any()
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;
    }
}
