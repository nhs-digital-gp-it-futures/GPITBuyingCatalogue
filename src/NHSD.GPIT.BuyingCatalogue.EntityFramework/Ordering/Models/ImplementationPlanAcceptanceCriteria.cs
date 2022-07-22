using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ImplementationPlanAcceptanceCriteria : IAudited
    {
        public int Id { get; set; }

        public int MilestoneId { get; set; }

        public string Description { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ImplementationPlanMilestone Milestone { get; set; }
    }
}
