using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class DataProcessingPlanCategory : IAudited
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }
    }
}
