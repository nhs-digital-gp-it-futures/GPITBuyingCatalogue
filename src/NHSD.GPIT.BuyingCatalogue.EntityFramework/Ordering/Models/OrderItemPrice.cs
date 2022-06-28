using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemPrice : IAudited, IPrice
    {
        public OrderItemPrice()
        {
            OrderItemPriceTiers = new HashSet<OrderItemPriceTier>();
        }

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int CataloguePriceId { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

        public CataloguePriceQuantityCalculationType? CataloguePriceQuantityCalculationType { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public string CurrencyCode { get; set; }

        public string Description { get; set; }

        public string RangeDescription { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderItem OrderItem { get; set; }

        public ICollection<OrderItemPriceTier> OrderItemPriceTiers { get; set; }
    }
}
