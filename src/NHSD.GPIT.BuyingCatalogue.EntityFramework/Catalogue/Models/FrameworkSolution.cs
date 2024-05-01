using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class FrameworkSolution : IAudited
    {
        public string FrameworkId { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        [Obsolete("All solutions are foundation solutions so this will be removed as part of story #23333")]
        public bool IsFoundation { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Framework Framework { get; set; }

        public Solution Solution { get; set; }
    }
}
