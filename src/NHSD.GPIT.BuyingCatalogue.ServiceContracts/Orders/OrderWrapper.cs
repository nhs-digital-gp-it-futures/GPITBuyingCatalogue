using System;
using System.Collections.Generic;
using System.Linq;
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
            // TODO Guard on previous orders to make sure doesnt match current
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

        public bool HasNewOrderRecipients
        {
            get
            {
                if (Order.Revision <= 1)
                {
                    return Order.FlattenedRecipients.Any();
                }

                if (Order?.FlattenedRecipients is null || !Order.FlattenedRecipients.Any())
                {
                    throw new InvalidOperationException("Order recipients is null or empty");
                }

                if (Previous?.FlattenedRecipients is null || !Previous.FlattenedRecipients.Any())
                {
                    throw new InvalidOperationException("Previous order recipients is null or empty");
                }

                return Order.FlattenedRecipients.Any(cr =>
                    Previous.FlattenedRecipients.All(pr => pr.RecipientOdsCode != cr.RecipientOdsCode));
            }
        }

        public bool HasNewOrderItems
        {
            get
            {
                if (Order.Revision <= 1)
                {
                    return Order.OrderItems.Count > 0;
                }

                if (Order?.OrderItems is null || !Order.OrderItems.Any())
                {
                    throw new InvalidOperationException("Order items is null or empty");
                }

                if (Previous?.OrderItems is null || !Previous.OrderItems.Any())
                {
                    throw new InvalidOperationException("Previous order items is null or empty");
                }

                return Order.OrderItems.Any(ci =>
                    Previous.OrderItems.All(pi => pi.CatalogueItemId != ci.CatalogueItemId));
            }
        }

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

        public ICollection<OrderSublocationRecipient> DetermineOrderRecipients(CatalogueItemId catalogueItemId)
        {
            return Order.DetermineOrderRecipients(Previous, catalogueItemId);
        }

        public bool CanComplete()
        {
            return Order.CanComplete(RolledUp.FlattenedRecipients.ToList(), OrderItems);
        }

        public OrderSublocationRecipient CreateRecipientWithExistingOrderContext(
            string recipientOdsCode,
            string parentSublocationOdsCode)
        {
            OrderSublocationRecipient newRecipient = new(Order.Id, recipientOdsCode, parentSublocationOdsCode);
            if (Order.DeliveryDate.HasValue)
            {
                Order.OrderItems.ToList().ForEach(i =>
                {
                    if (Previous == null
                        || !Previous.Exists(i.CatalogueItemId)
                        || Previous.FlattenedRecipients.All(x => x.RecipientOdsCode != recipientOdsCode))
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
