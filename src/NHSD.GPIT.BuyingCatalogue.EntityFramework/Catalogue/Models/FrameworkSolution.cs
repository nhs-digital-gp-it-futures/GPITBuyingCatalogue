﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class FrameworkSolution
    {
        public string FrameworkId { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public bool IsFoundation { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public Framework Framework { get; set; }
    }
}
