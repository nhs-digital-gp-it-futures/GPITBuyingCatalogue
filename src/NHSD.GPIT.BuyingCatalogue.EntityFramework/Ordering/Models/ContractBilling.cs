using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ContractBilling
    {
        public ContractBilling()
        {
            ContractBillingItems = new List<ContractBillingItem>();
            Requirements = new List<Requirement>();
        }

        public int Id { get; set; }

        public int ContractId { get; set; }

        public ICollection<ContractBillingItem> ContractBillingItems { get; set; }

        public ICollection<Requirement> Requirements { get; set; }

        public virtual Contract Contract { get; set; }
    }
}
