using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
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

        public IReadOnlyList<Order> PreviousOrders => previous;

        public bool IsAmendment => Order.CallOffId.IsAmendment;

        public bool HasNewOrderRecipients => Order.OrderRecipients
            .Any(r => Previous?.OrderRecipients?
            .FirstOrDefault(x => x.OdsCode == r.OdsCode) == null);

        public bool HasNewOrderItems => Order.OrderItems
            .Any(r => Previous?.OrderItems?
            .FirstOrDefault(x => x.CatalogueItemId == r.CatalogueItemId) == null);

        public ICollection<OrderRecipient> ExistingOrderRecipients => Previous?.OrderRecipients ?? Enumerable.Empty<OrderRecipient>().ToList();

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

        public ICollection<OrderRecipient> AddedOrderRecipients() => Order.AddedOrderRecipients(Previous);

        public ICollection<OrderRecipient> DetermineOrderRecipients(CatalogueItemId catalogueItemId) => Order.DetermineOrderRecipients(Previous, catalogueItemId);

        public OrderRecipient InitialiseOrderRecipient(string odsCode)
        {
            var newRecipient = Order.InitialiseOrderRecipient(odsCode);
            if (Order.DeliveryDate.HasValue)
            {
                Order.OrderItems.ToList().ForEach(i =>
                {
                    if (Previous == null
                    || !Previous.Exists(i.CatalogueItemId)
                    || !Previous.OrderRecipients.Exists(odsCode))
                    {
                        newRecipient.SetDeliveryDateForItem(i.CatalogueItemId, Order.DeliveryDate.Value);
                    }
                });
            }

            return newRecipient;
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
