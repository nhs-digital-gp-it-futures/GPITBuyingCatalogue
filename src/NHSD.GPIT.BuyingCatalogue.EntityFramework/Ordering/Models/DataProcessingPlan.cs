using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class DataProcessingPlan : IAudited
    {
        public DataProcessingPlan()
        {
            Steps = new List<DataProcessingPlanStep>();
        }

        public int Id { get; set; }

        public bool IsDefault { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public ICollection<DataProcessingPlanStep> Steps { get; set; }
    }
}
