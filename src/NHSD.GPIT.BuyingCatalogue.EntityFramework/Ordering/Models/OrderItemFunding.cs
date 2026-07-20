using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed class OrderItemFunding : IAudited, ICloneable<OrderItemFunding>
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }

        public OrderItemFundingType OrderItemFundingType { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderItem OrderItem { get; set; }

        public OrderItemFunding Clone(bool preserveIds = false) =>
            new() { OrderItemId = OrderItemId, OrderItemFundingType = OrderItemFundingType };
    }
}
