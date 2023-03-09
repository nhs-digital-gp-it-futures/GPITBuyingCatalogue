using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemPrice
    {
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

        public OrderItemPrice(OrderItemPrice existingPrice)
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

        public string ToPriceUnitString()
        {
            return $"{Description} {BillingPeriod?.Description() ?? string.Empty}".Trim();
        }

        public bool IsPerServiceRecipient() => ProvisioningType.IsPerServiceRecipient()
            || CataloguePriceQuantityCalculationType is Catalogue.Models.CataloguePriceQuantityCalculationType.PerServiceRecipient;
    }
}
