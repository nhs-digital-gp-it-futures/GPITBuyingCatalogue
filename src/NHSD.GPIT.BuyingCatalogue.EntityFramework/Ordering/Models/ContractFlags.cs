using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public class ContractFlags : IAudited
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public bool? HasSpecificRequirements { get; set; }

        public bool? UseDefaultBilling { get; set; }

        public bool? UseDefaultDataProcessing { get; set; }

        public bool? UseDefaultImplementationPlan { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime LastUpdated { get; set; }

        public virtual Order Order { get; set; }
    }
}
