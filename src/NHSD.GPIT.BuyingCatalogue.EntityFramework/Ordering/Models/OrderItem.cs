using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public int OrderId { get; set; }

        public Order Order { get; set; }

        // TODO: should be of type CatalogueItemId
        // Had to revert to string due to CatalogueItem currently having an incorrect ID type
        public string CatalogueItemId { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public int PriceId { get; set; }

        public CataloguePrice CataloguePrice { get; set; }

        public decimal? Price { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public DateTime? DefaultDeliveryDate { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public IReadOnlyList<OrderItemRecipient> OrderItemRecipients => recipients;
    }
}
