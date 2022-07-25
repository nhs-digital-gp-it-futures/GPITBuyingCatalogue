using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class Contract : IAudited
    {
        public Contract()
        {
            BillingItems = new List<ContractBillingItem>();
        }

        public int Id { get; set; }

        public int OrderId { get; set; }

        public int? ImplementationPlanId { get; set; }

        public int? DataProcessingPlanId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public ImplementationPlan ImplementationPlan { get; set; }

        public DataProcessingPlan DataProcessingPlan { get; set; }

        public ICollection<ContractBillingItem> BillingItems { get; set; }
    }
}
