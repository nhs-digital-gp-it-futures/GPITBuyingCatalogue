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

        public bool IsAmendment => previous.Any();

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

                var output = previous.First();

                previous.Skip(1).ToList().ForEach(output.Apply);

                return output;
            }
        }

        public Order RolledUp
        {
            get
            {
                var output = Previous;

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
            if (Previous == null)
            {
                return false;
            }

            return Order.OrderItems.Any(x => x.CatalogueItemId == orderItem.CatalogueItemId)
                && Previous.OrderItems.Any(x => x.CatalogueItemId == orderItem.CatalogueItemId);
        }
    }
}
