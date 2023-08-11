using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public class ContractFlags : IAudited
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public bool? UseDefaultDataProcessing { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime LastUpdated { get; set; }

        public virtual Order Order { get; set; }
    }
}
