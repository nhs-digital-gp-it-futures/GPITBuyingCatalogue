using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ImplementationPlanMilestone : IAudited
    {
        public int Id { get; set; }

        public int? PlanId { get; set; }

        public int? ContractBillingItemId { get; set; }

        public int Order { get; set; }

        public string Title { get; set; }

        public string PaymentTrigger { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public virtual ImplementationPlan Plan { get; set; }

        public ContractBillingItem ContractBillingItem { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }
    }
}
