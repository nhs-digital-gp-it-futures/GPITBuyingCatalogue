using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderWrapper
    {
        private readonly List<Order> previous = new();

        public OrderWrapper()
        {
        }

        public OrderWrapper(Order order)
        {
            Order = order;
        }

        public OrderWrapper(IEnumerable<Order> orders)
        {
            var ordered = orders.OrderBy(x => x.CallOffId.Revision).ToList();

            Order = ordered.Any()
                ? ordered.Last()
                : null;

            previous = ordered.Count > 1
                ? ordered.SkipLast(1).ToList()
                : new List<Order>();
        }

        public bool IsAmendment => Order.CallOffId.IsAmendment;

        public Order Last => previous.Any()
            ? previous.Last()
            : null;

        public Order Order { get; set; }

        public Order Previous
        {
            get
            {
                if (!previous.Any())
                {
                    return null;
                }

                var output = previous.First().Clone();

                previous.Skip(1).ToList().ForEach(output.Apply);

                return output;
            }
        }

        public Order RolledUp
        {
            get
            {
                var output = Previous?.Clone();

                if (output == null)
                {
                    return Order;
                }

                output.Apply(Order);
                output.Revision = Order.Revision;

                return output;
            }
        }

        public bool HasCurrentAmendments(OrderItem orderItem)
        {
            var previousOrder = Previous;

            if (previousOrder == null)
            {
                return false;
            }

            if (Order.OrderItems.Any(x => x.CatalogueItemId == orderItem.CatalogueItemId)
                && previousOrder.OrderItems.All(x => x.CatalogueItemId != orderItem.CatalogueItemId))
            {
                return true;
            }

            return Order.OrderItems.Any(x => x.CatalogueItemId == orderItem.CatalogueItemId)
                && previousOrder.OrderItems.Any(x => x.CatalogueItemId == orderItem.CatalogueItemId);
        }

        public IEnumerable<OrderItemFundingType> FundingTypesForItem(CatalogueItemId catalogueItemId)
        {
            var fundingTypes = previous
                .SelectMany(o => o.OrderItems.Where(oi => oi.CatalogueItemId == catalogueItemId).Select(oi => oi.FundingType))
                .ToList();

            fundingTypes.AddRange(Order.OrderItems.Where(oi => oi.CatalogueItemId == catalogueItemId).Select(oi => oi.FundingType));
            return fundingTypes.Distinct();
        }
    }
}
