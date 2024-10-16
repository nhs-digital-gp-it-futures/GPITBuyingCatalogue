﻿namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class ContractBillingItem
    {
        public int Id { get; set; }

        public int ContractBillingId { get; set; }

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int Quantity { get; set; }

        public ContractBilling ContractBilling { get; set; }

        public OrderItem OrderItem { get; set; }

        public virtual ImplementationPlanMilestone Milestone { get; set; }
    }
}
