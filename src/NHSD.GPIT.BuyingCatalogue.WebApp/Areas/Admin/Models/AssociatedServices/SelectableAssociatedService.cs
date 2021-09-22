﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices
{
    public sealed class SelectableAssociatedService
    {
        public string Name { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Description { get; set; }

        public string OrderGuidance { get; set; }

        public bool Selected { get; set; }
    }
}
