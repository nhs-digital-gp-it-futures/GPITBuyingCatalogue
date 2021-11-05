using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem : IAudited
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public int PriceId { get; set; }

        public CataloguePrice CataloguePrice { get; set; }

        public decimal? Price { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public DateTime? DefaultDeliveryDate { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public int LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public IReadOnlyList<OrderItemRecipient> OrderItemRecipients => recipients;
    }
}
