﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BaseModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels
{
    public sealed class ImplementationTimescaleModel : MarketingBaseModel
    {
        public ImplementationTimescaleModel()
            : base(null)
        {
        }

        public ImplementationTimescaleModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Description = CatalogueItem?.Solution?.ImplementationDetail;
            SolutionName = catalogueItem?.Name;
        }

        [StringLength(1100)]
        public string Description { get; set; }

        public string SolutionName { get; set; }
    }
}
