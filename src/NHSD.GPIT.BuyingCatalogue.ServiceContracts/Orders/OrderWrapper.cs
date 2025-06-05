using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderWrapper
    {
        private readonly List<Order> previous = new();
        private readonly Lazy<Order> previousLazy;
        private readonly Lazy<Order> rolledUpLazy;

        public OrderWrapper()
        {
            previousLazy = new Lazy<Order>((Order)null);
            rolledUpLazy = new Lazy<Order>((Order)null);
        }

        public OrderWrapper(Order order)
            : this(order, [])
        {
        }

        public OrderWrapper(Order currentOrder, IEnumerable<Order> previousOrders)
        {
            ArgumentNullException.ThrowIfNull(currentOrder);

            Order = currentOrder;

            previous = previousOrders.OrderBy(x => x.CallOffId.Revision).ToList();

            previousLazy = new Lazy<Order>(() =>
            {
                if (previous.Count == 0)
                {
                    return null;
                }

                var output = previous.First().Clone();

                foreach (var amendment in previous.Skip(1))
                {
                    output.Apply(amendment);
                }

                return output;
            });

            rolledUpLazy = new Lazy<Order>(() =>
            {
                var output = Previous?.Clone();

                if (output == null)
                {
                    return Order.Clone();
                }

                output.Apply(Order.Clone());
                output.Revision = Order.Revision;

                return output;
            });
        }

        public IReadOnlyList<Order> PreviousOrders => previous;

        public bool IsAmendment => Order.CallOffId.IsAmendment;

        public bool HasNewOrderRecipients => Order.OrderRecipients
            .Any(r => Previous?.OrderRecipients?
            .FirstOrDefault(x => x.OdsCode == r.OdsCode) == null);

        public bool HasNewOrderItems => Order.OrderItems
            .Any(r => Previous?.OrderItems?
            .FirstOrDefault(x => x.CatalogueItemId == r.CatalogueItemId) == null);

        public ICollection<OrderItem> OrderItems =>
            Order.OrderItems.Where(oi => DetermineOrderRecipients(oi.CatalogueItemId).Count > 0)
            .ToList();

        public Order Last => previous.Any()
            ? previous.Last()
            : null;

        /// <summary>
        /// Gets or sets the most recent Order.
        /// </summary>
        /// <remarks>
        /// This will be the order that is being placed at that time. That could either be the original order or an amendment.
        /// </remarks>
        public Order Order { get; set; }

        /// <summary>
        /// Gets a flattened order that contains all previous amendments (excluding the current) projected over original order.
        ///
        /// Otherwise null if this <see cref="OrderWrapper"/> relates to an original order.
        /// </summary>
        public Order Previous => previousLazy.Value;

        /// <summary>
        /// Gets a flattened order that projects the current amendment over the <see cref="Previous"/> order projection.
        /// </summary>
        public Order RolledUp => rolledUpLazy.Value;

        public IEnumerable<string> AddedRecipientsOdsCodes()
        {
            var codes = Order.AddedOrderRecipients(Previous)
                .Select(r => r.OdsCode);
            if (Order.AssociatedServicesOnlyDetails?.PracticeReorganisationOdsCode != null)
            {
                codes = codes.Append(Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode);
            }

            return codes;
        }

        public IEnumerable<string> PreviousRecipientsOdsCodes()
        {
            return (Previous?.OrderRecipients ?? Enumerable.Empty<OrderRecipient>())
                .Select(r => r.OdsCode);
        }

        public ICollection<OrderRecipient> DetermineOrderRecipients(CatalogueItemId catalogueItemId) => Order.DetermineOrderRecipients(Previous, catalogueItemId);

        public bool CanComplete()
        {
            return Order.CanComplete(RolledUp.OrderRecipients, OrderItems);
        }

        public OrderRecipient InitialiseOrderRecipient(string odsCode)
        {
            var newRecipient = new OrderRecipient(Order.Id, odsCode);
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

            if (Order.OrderType.MergerOrSplit)
            {
                Order.OrderItems.ToList().ForEach(i =>
                {
                    newRecipient.SetQuantityForItem(i.CatalogueItemId, 1);
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
