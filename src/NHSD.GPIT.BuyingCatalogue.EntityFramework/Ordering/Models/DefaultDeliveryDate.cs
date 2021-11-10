using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class DefaultDeliveryDate : IEquatable<DefaultDeliveryDate>,  IAudited
    {
        public int OrderId { get; init; }

        public CatalogueItemId CatalogueItemId { get; init; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Order Order { get; set; }

        public bool Equals(DefaultDeliveryDate other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return OrderId == other.OrderId && CatalogueItemId == other.CatalogueItemId;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DefaultDeliveryDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, CatalogueItemId);
        }
    }
}
