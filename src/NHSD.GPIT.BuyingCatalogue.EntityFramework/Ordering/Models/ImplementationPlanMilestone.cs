using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ImplementationPlanMilestone : IAudited
    {
        public ImplementationPlanMilestone()
        {
            AcceptanceCriteria = new List<ImplementationPlanAcceptanceCriteria>();
        }

        public int Id { get; set; }

        public int PlanId { get; set; }

        public int Order { get; set; }

        public string Title { get; set; }

        public string PaymentTrigger { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public virtual ImplementationPlan Plan { get; set; }

        public virtual ContractBillingItem ContractBillingItem { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ICollection<ImplementationPlanAcceptanceCriteria> AcceptanceCriteria { get; set; }
    }
}
