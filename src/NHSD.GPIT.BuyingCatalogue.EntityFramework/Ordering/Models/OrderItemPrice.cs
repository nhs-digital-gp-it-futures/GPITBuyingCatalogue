using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed class OrderItemPrice : IAudited, IPrice
    {
        public OrderItemPrice()
        {
            OrderItemPriceTiers = new HashSet<OrderItemPriceTier>();
        }

        public OrderItemPrice(OrderItem item, CataloguePrice cataloguePrice)
            : this(cataloguePrice)
        {
            OrderId = item.OrderId;
            OrderItem = item;
        }

        public OrderItemPrice(CataloguePrice cataloguePrice)
            : this()
        {
            CatalogueItemId = cataloguePrice.CatalogueItemId;
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            ProvisioningType = cataloguePrice.ProvisioningType;
            CataloguePriceType = cataloguePrice.CataloguePriceType;
            CataloguePriceCalculationType = cataloguePrice.CataloguePriceCalculationType;
            BillingPeriod = cataloguePrice.TimeUnit;
            CataloguePriceQuantityCalculationType = cataloguePrice.CataloguePriceQuantityCalculationType;
            CurrencyCode = cataloguePrice.CurrencyCode;
            Description = cataloguePrice.PricingUnit.Description;
            RangeDescription = cataloguePrice.PricingUnit.RangeDescription;

            foreach (var tier in cataloguePrice.CataloguePriceTiers)
            {
                OrderItemPriceTiers.Add(new OrderItemPriceTier(this, tier));
            }
        }

        private OrderItemPrice(OrderItemPrice existingPrice)
            : this()
        {
            CatalogueItemId = existingPrice.CatalogueItemId;
            CataloguePriceId = existingPrice.CataloguePriceId;
            ProvisioningType = existingPrice.ProvisioningType;
            CataloguePriceType = existingPrice.CataloguePriceType;
            CataloguePriceCalculationType = existingPrice.CataloguePriceCalculationType;
            BillingPeriod = existingPrice.BillingPeriod;
            CataloguePriceQuantityCalculationType = existingPrice.CataloguePriceQuantityCalculationType;
            CurrencyCode = existingPrice.CurrencyCode;
            Description = existingPrice.Description;
            RangeDescription = existingPrice.RangeDescription;

            foreach (var tier in existingPrice.OrderItemPriceTiers)
            {
                OrderItemPriceTiers.Add(new OrderItemPriceTier(this, tier));
            }
        }

        public ICollection<IPriceTier> PriceTiers => OrderItemPriceTiers.Cast<IPriceTier>().ToList();

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int CataloguePriceId { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

        public CataloguePriceQuantityCalculationType? CataloguePriceQuantityCalculationType { get; set; }

        public TimeUnit? BillingPeriod { get; set; }

        public string CurrencyCode { get; set; }

        public string Description { get; set; }

        public string RangeDescription { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderItem OrderItem { get; set; }

        public ICollection<OrderItemPriceTier> OrderItemPriceTiers { get; set; }

        public OrderItemPrice Copy() => new(this);

        public bool IsPerServiceRecipient() => ProvisioningType.IsPerServiceRecipient()
            || CataloguePriceQuantityCalculationType is Catalogue.Models.CataloguePriceQuantityCalculationType.PerServiceRecipient;

        public string ToPriceUnitString()
        {
            return $"{Description} {BillingPeriod?.Description() ?? string.Empty}".Trim();
        }
    }
}
