using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public sealed class OrderItemPriceTier : IAudited, IOrderablePriceTier
    {
        public OrderItemPriceTier()
        {
        }

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

        public OrderItemPriceTier(IOrderablePriceTier priceTier)
        {
            Price = priceTier.Price;
            ListPrice = priceTier.ListPrice;
            LowerRange = priceTier.LowerRange;
            UpperRange = priceTier.UpperRange;
        }

        internal OrderItemPriceTier(OrderItemPrice price, IOrderablePriceTier tier)
        {
            OrderId = price.OrderId;
            CatalogueItemId = price.CatalogueItemId;
            Price = tier.Price;
            ListPrice = tier.Price;
            LowerRange = tier.LowerRange;
            UpperRange = tier.UpperRange;
            OrderItemPrice = price;
        }

        public int Id { get; set; }

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public decimal Price { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public decimal ListPrice { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderItemPrice OrderItemPrice { get; set; }

        public string GetRangeDescription()
        {
            var upperRange = UpperRange == null
                ? "+"
                : $" to {UpperRange.Value}";

            return $"{LowerRange}{upperRange} {OrderItemPrice?.RangeDescription}".Trim();
        }
    }
}
