using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class WorkOffPlan : IAudited
    {
        public int Id { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public Solution Solution { get; set; }

        public string StandardId { get; set; }

        public Standard Standard { get; set; }

        public string Details { get; set; }

        public DateTime CompletionDate { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
