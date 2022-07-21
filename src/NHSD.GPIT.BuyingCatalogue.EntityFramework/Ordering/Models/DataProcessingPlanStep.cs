using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class DataProcessingPlanStep : IAudited
    {
        public int Id { get; set; }

        public int PlanId { get; set; }

        public int CategoryId { get; set; }

        public string Details { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public DataProcessingPlan Plan { get; set; }

        public DataProcessingPlanCategory Category { get; set; }
    }
}
