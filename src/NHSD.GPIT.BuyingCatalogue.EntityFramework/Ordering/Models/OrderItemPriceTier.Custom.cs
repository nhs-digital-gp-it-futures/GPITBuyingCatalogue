using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemPriceTier
    {
        public OrderItemPriceTier(OrderItemPrice price, CataloguePriceTier tier)
        {
            OrderId = price.OrderId;
            CatalogueItemId = price.CatalogueItemId;
            Price = tier.Price;
            ListPrice = tier.Price;
            LowerRange = tier.LowerRange;
            UpperRange = tier.UpperRange;
            OrderItemPrice = price;
        }

        internal OrderItemPriceTier(OrderItemPrice price, OrderItemPriceTier tier)
        {
            OrderId = price.OrderId;
            CatalogueItemId = price.CatalogueItemId;
            Price = tier.Price;
            ListPrice = tier.Price;
            LowerRange = tier.LowerRange;
            UpperRange = tier.UpperRange;
            OrderItemPrice = price;
        }

        public string GetRangeDescription()
        {
            var upperRange = UpperRange == null
                ? "+"
                : $" to {UpperRange.Value}";

            return $"{LowerRange}{upperRange} {OrderItemPrice?.RangeDescription}".Trim();
        }
    }
}
