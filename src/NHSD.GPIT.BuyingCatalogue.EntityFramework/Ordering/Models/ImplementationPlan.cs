using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ImplementationPlan : IAudited
    {
        public ImplementationPlan()
        {
            Milestones = new List<ImplementationPlanMilestone>();
        }

        public int Id { get; set; }

        public bool IsDefault { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public ICollection<ImplementationPlanMilestone> Milestones { get; set; }
    }
}
