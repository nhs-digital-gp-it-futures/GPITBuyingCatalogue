using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ContractBilling : IAudited
    {
        public int Id { get; set; }

        public int ContractId { get; set; }

        public ICollection<ContractBillingItem> ContractBillingItems { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public virtual Contract Contract { get; set; }
    }
}
