using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed partial class OrderItem : IAudited, ICloneable<OrderItem>
    {
        public OrderItem()
        {
        }

        public OrderItem(
            CatalogueItemId catalogueItemId)
        {
            CatalogueItemId = catalogueItemId;
        }

        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int? ParentId { get; set; }

        public OrderItem Parent { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public int? Quantity { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderItemFunding OrderItemFunding { get; set; }

        public OrderItemPrice OrderItemPrice { get; set; }

        public ICollection<OrderItem> Services { get; set; } = new HashSet<OrderItem>();

        public OrderItem Clone() => new(CatalogueItemId)
        {
            OrderItemPrice = OrderItemPrice?.Clone(),
            EstimationPeriod = EstimationPeriod,
            CatalogueItem = CatalogueItem,
            OrderItemFunding = OrderItemFunding?.Clone(),
        };
    }
}
