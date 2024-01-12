using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed partial class OrderItem : IAudited
    {
        public OrderItem()
        {
        }

        public OrderItem(
            CatalogueItemId catalogueItemId)
        {
            CatalogueItemId = catalogueItemId;
        }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public int? Quantity { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderItemFunding OrderItemFunding { get; set; }

        public OrderItemPrice OrderItemPrice { get; set; }
    }
}
