using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemPrice
    {
        public OrderItemPrice(OrderItem item, CataloguePrice cataloguePrice)
            : base()
        {
            OrderId = item.OrderId;
            CatalogueItemId = cataloguePrice.CatalogueItemId;
            ProvisioningType = cataloguePrice.ProvisioningType;
            CataloguePriceType = cataloguePrice.CataloguePriceType;
            CataloguePriceCalculationType = cataloguePrice.CataloguePriceCalculationType;
            EstimationPeriod = cataloguePrice.TimeUnit;
            CurrencyCode = cataloguePrice.CurrencyCode;
            Description = cataloguePrice.PricingUnit.Description;
            OrderItem = item;

            AddTiers(cataloguePrice);
        }

        public OrderItemPrice(CataloguePrice cataloguePrice)
            : base()
        {
            CatalogueItemId = cataloguePrice.CatalogueItemId;
            ProvisioningType = cataloguePrice.ProvisioningType;
            CataloguePriceType = cataloguePrice.CataloguePriceType;
            CataloguePriceCalculationType = cataloguePrice.CataloguePriceCalculationType;
            EstimationPeriod = cataloguePrice.TimeUnit;
            CurrencyCode = cataloguePrice.CurrencyCode;
            Description = cataloguePrice.PricingUnit.Description;

            AddTiers(cataloguePrice);
        }

        public CostType CostType =>
            OrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService &&
            ProvisioningType == ProvisioningType.Declarative
                ? CostType.OneOff
                : CostType.Recurring;

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
