using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class CatalogueItemEpic : IAudited
    {
        public CatalogueItemEpic()
        {
        }

        public CatalogueItemEpic(
            CatalogueItemId catalogueItemId,
            int capabilityId,
            string epicId)
        {
            CatalogueItemId = catalogueItemId;
            CapabilityId = capabilityId;
            EpicId = epicId;
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int CapabilityId { get; set; }

        public string EpicId { get; set; }

        public int StatusId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public Epic Epic { get; set; }

        public CatalogueItemEpicStatus Status { get; set; }
    }
}
