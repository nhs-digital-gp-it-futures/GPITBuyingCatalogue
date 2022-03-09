using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        private readonly List<OrderItemRecipient> recipients = new();
        private DateTime lastUpdated = DateTime.UtcNow;

        public CostType CostType =>
            CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService &&
            CataloguePrice.ProvisioningType == ProvisioningType.Declarative
                ? CostType.OneOff
                : CostType.Recurring;

        public void SetRecipients(IEnumerable<OrderItemRecipient> itemRecipients)
        {
            recipients.Clear();
            recipients.AddRange(itemRecipients);
            Updated();
        }

        public bool Equals(OrderItem other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return CatalogueItem.Equals(other.CatalogueItem) && OrderId == other.OrderId;
        }

        public override bool Equals(object obj) => Equals(obj as OrderItem);

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, CatalogueItem);
        }

        private void Updated() => lastUpdated = DateTime.UtcNow;
    }
}
