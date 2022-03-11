using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public int GetTotalRecipientQuantity() => OrderItemRecipients.ToList().Sum(oir => oir.Quantity);

        public decimal CalculateTotalCost()
        {
            var quantity = GetTotalRecipientQuantity();

            return OrderItemPrice.CataloguePriceCalculationType switch
            {
                CataloguePriceCalculationType.SingleFixed => CalculateTotalCostFlatSingle(quantity),
                CataloguePriceCalculationType.Cumulative or _ => CalculateTotalCostCumilative(quantity),
            };
        }

        public decimal CalculateCostPerMonth() =>
            OrderItemPrice.EstimationPeriod.Value == TimeUnit.PerMonth ? CalculateTotalCost() : CalculateTotalCost() / 12;

        public decimal CalculateCostPerYear() =>
            OrderItemPrice.EstimationPeriod.Value == TimeUnit.PerYear ? CalculateTotalCost() : CalculateTotalCost() * 12;

        public bool Equals(OrderItem other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return CatalogueItem.Equals(other.CatalogueItem) && OrderId == other.OrderId;
        }

        public override bool Equals(object obj) => Equals(obj as OrderItem);

        public override int GetHashCode()
        {
            return HashCode.Combine(OrderId, CatalogueItem);
        }

        private decimal CalculateTotalCostCumilative(int quantity)
        {
            var lastUpperRange = 0;
            var totalCost = 0M;

            var tiers = OrderItemPrice.OrderItemPriceTiers.OrderBy(t => t.LowerRange);

            for (var i = 0; quantity > 0; i++)
            {
                var tier = tiers.ElementAt(i);

                int range;
                if (!tier.UpperRange.HasValue)
                {
                    range = quantity;
                }
                else
                {
                    range = tier.UpperRange.Value + 1;
                    range -= lastUpperRange;

                    if (range > quantity)
                        range = quantity;
                }

                totalCost += tier.Price * range;
                quantity -= range;

                if (tier.UpperRange.HasValue)
                    lastUpperRange = tier.UpperRange.Value + 1;
            }

            return totalCost;
        }

        private decimal CalculateTotalCostFlatSingle(int quantity) =>
            OrderItemPrice.OrderItemPriceTiers
            .OrderBy(t => t.LowerRange)
            .First(t => (t.UpperRange.HasValue && t.UpperRange.Value > quantity) || !t.UpperRange.HasValue).Price;
    }
}
