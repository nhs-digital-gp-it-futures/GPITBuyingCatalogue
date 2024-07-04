using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

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

        public int? ContractId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ICollection<ImplementationPlanMilestone> Milestones { get; set; }

        public virtual Contract Contract { get; set; }
    }
}
