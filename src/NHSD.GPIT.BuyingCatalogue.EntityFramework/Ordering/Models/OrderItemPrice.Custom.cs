using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemPrice
    {
        public OrderItemPrice(OrderItem item, CataloguePrice cataloguePrice)
            : this()
        {
            OrderId = item.OrderId;
            CatalogueItemId = cataloguePrice.CatalogueItemId;
            ProvisioningType = cataloguePrice.ProvisioningType;
            CataloguePriceType = cataloguePrice.CataloguePriceType;
            CataloguePriceCalculationType = cataloguePrice.CataloguePriceCalculationType;
            EstimationPeriod = cataloguePrice.TimeUnit;
            CurrencyCode = cataloguePrice.CurrencyCode;
            Description = cataloguePrice.PricingUnit.Description;
            RangeDescription = cataloguePrice.PricingUnit.RangeDescription;
            OrderItem = item;

            AddTiers(cataloguePrice);
        }

        public OrderItemPrice(CataloguePrice cataloguePrice)
            : this()
        {
            CatalogueItemId = cataloguePrice.CatalogueItemId;
            ProvisioningType = cataloguePrice.ProvisioningType;
            CataloguePriceType = cataloguePrice.CataloguePriceType;
            CataloguePriceCalculationType = cataloguePrice.CataloguePriceCalculationType;
            EstimationPeriod = cataloguePrice.TimeUnit;
            CurrencyCode = cataloguePrice.CurrencyCode;
            Description = cataloguePrice.PricingUnit.Description;
            RangeDescription = cataloguePrice.PricingUnit.RangeDescription;

            AddTiers(cataloguePrice);
        }

        public CostType CostType =>
            OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService &&
            ProvisioningType == ProvisioningType.Declarative
                ? CostType.OneOff
                : CostType.Recurring;

        public string ToPriceUnitString()
        {
            var timeUnit = EstimationPeriod.HasValue
                ? $" {EstimationPeriod.Value.Description()}"
                : string.Empty;

            return $"{Description}{timeUnit}";
        }

        private void AddTiers(CataloguePrice cataloguePrice)
        {
            foreach (var cataloguePriceTier in cataloguePrice.CataloguePriceTiers)
            {
                var orderItemPriceTier = new OrderItemPriceTier(this, cataloguePriceTier);

                OrderItemPriceTiers.Add(orderItemPriceTier);
            }
        }
    }
}
