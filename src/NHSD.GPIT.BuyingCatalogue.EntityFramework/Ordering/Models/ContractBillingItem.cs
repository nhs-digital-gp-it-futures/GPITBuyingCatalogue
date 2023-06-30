using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ContractBillingItem : IAudited
    {
        public int Id { get; set; }

        public int ContractBillingId { get; set; }

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ContractBilling ContractBilling { get; set; }

        public OrderItem OrderItem { get; set; }

        public virtual ImplementationPlanMilestone Milestone { get; set; }
    }
}
