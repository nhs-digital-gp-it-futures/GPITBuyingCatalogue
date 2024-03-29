﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class CatalogueItemCapability : IAudited
    {
        public CatalogueItemCapability()
        {
        }

        public CatalogueItemCapability(
            CatalogueItemId catalogueItemId,
            int capabilityId)
        {
            CatalogueItemId = catalogueItemId;
            CapabilityId = capabilityId;
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int CapabilityId { get; set; }

        public int StatusId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Capability Capability { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public CatalogueItemCapabilityStatus Status { get; set; }
    }
}
