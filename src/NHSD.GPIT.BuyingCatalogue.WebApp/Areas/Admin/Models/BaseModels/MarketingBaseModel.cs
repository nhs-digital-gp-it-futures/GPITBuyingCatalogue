﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BaseModels
{
    public abstract class MarketingBaseModel : NavBaseModel
    {
        protected MarketingBaseModel(CatalogueItem catalogueItem)
        {
            CatalogueItem = catalogueItem;
            ClientApplication = CatalogueItem?.Solution?.EnsureAndGetApplicationType();
            SolutionId = CatalogueItem?.Id;
            SupplierId = CatalogueItem?.Supplier?.Id ?? 0;
        }

        public CatalogueItemId? SolutionId { get; set; }

        public int SupplierId { get; set; }

        public ApplicationTypeDetail ClientApplication { get; set; }

        public ApplicationTypeProgress ClientApplicationProgress => new ApplicationTypeProgress(ClientApplication);

        protected CatalogueItem CatalogueItem { get; set; }
    }
}
