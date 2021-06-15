using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class OrderItem
    {
        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string PricingUnitName { get; set; }

        public int? PriceId { get; set; }

        public string CurrencyCode { get; set; }

        public decimal? Price { get; set; }

        public DateTime? DefaultDeliveryDate { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public virtual CatalogueItem CatalogueItem { get; set; }

        public virtual CataloguePriceType CataloguePriceType { get; set; }

        public virtual TimeUnit EstimationPeriod { get; set; }

        public virtual Order Order { get; set; }

        // TODO - Called PricingUnit in old code
        public virtual PricingUnit PricingUnitNameNavigation { get; set; }

        public virtual ProvisioningType ProvisioningType { get; set; }

        // TODO - Called PriceTimeUnit in old code
        public virtual TimeUnit TimeUnit { get; set; }

        public IReadOnlyList<OrderItemRecipient> OrderItemRecipients => recipients;
    }
}
