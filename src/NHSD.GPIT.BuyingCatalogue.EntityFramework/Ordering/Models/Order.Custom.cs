using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class Order
    {
        private readonly List<ServiceInstanceItem> serviceInstanceItems = new();

        private DateTime? completed;

        public IReadOnlyList<ServiceInstanceItem> ServiceInstanceItems => serviceInstanceItems.AsReadOnly();

        public void Complete()
        {
            OrderStatus = OrderStatus.Completed;
            completed = DateTime.UtcNow;
        }

        public bool CanComplete()
        {
            return
                !string.IsNullOrWhiteSpace(Description)
                && OrderingPartyContact is not null
                && Supplier is not null
                && CommencementDate is not null
                && (HasSolution() || HasAssociatedService())
                && OrderItems.Count > 0
                && OrderItems.All(oi => oi.OrderItemFunding is not null)
                && OrderStatus != OrderStatus.Completed;
        }

        public bool HasAssociatedService()
        {
            return OrderItems.Any(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);
        }

        public bool HasSolution()
        {
            return OrderItems.Any(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
        }

        public bool Equals(Order other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return ReferenceEquals(this, other) || Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Order);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
