﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class AdditionalService : IAudited
    {
        public CatalogueItemId CatalogueItemId { get; set; }

        public string Summary { get; set; }

        public string FullDescription { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public Solution Solution { get; set; }
    }
}
