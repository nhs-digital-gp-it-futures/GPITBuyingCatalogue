using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderWrapper
    {
        private readonly IDictionary<int, Order> previous = new Dictionary<int, Order>();
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

            previous = previousOrders.ToDictionary(x => x.Id, x => x);

            var orderedPreviousOrders = previous.Values;

            previousLazy = new Lazy<Order>(() =>
            {
                if (orderedPreviousOrders.Count == 0)
                {
                    return null;
                }

                Order output = orderedPreviousOrders.First().Clone(true);

                foreach (OrderSublocationRecipient recipient in output.GetOrderRecipients())
                {
                    recipient.OrderId = orderedPreviousOrders.First().Id;
                }

                foreach (Order amendment in orderedPreviousOrders.Skip(1))
                {
                    output.Apply(amendment);
                }

                return output;
            });

            rolledUpLazy = new Lazy<Order>(() =>
            {
                var output = Previous?.Clone(true);

                if (output == null)
                {
                    return Order.Clone(true);
                }

                output.Apply(Order.Clone(true));
                output.Revision = Order.Revision;

                return output;
            });
        }

        public IReadOnlyList<Order> PreviousOrders => GetPreviousOrders();

        public bool IsAmendment => Order.CallOffId.IsAmendment;

        public bool HasNewOrderRecipients => Order.FlattenedRecipients
            .Any(r => Previous?.FlattenedRecipients?
                .FirstOrDefault(x => x.RecipientOdsCode == r.RecipientOdsCode) == null);

        public bool HasNewOrderItems => Order.OrderItems
            .Any(r => Previous?.OrderItems?
                .FirstOrDefault(x => x.CatalogueItemId == r.CatalogueItemId) == null) || Order.OrderItems
            .Any(item => item?.CatalogueItem?.CatalogueItemType == CatalogueItemType.AssociatedService);

        public ICollection<OrderItem> OrderItems =>
            Order.OrderItems.Where(oi => DetermineOrderRecipients(oi).Count > 0)
                .ToList();

        public Order Last => previous.Any() ? previous.Last().Value : null;

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

        public string GetCallOffIdForPreviousRecipient(OrderSublocationRecipient recipient, int? orderIdWithCatalogueItem)
        {
            var initialOrderWithCatalogueItem = orderIdWithCatalogueItem.HasValue
                    ? previous[orderIdWithCatalogueItem.Value]
                    : null;
            var initialOrderWithRecipient = previous[recipient.OrderId];

            var callOffId = initialOrderWithCatalogueItem == null || initialOrderWithRecipient.Revision >= initialOrderWithCatalogueItem.Revision
                ? initialOrderWithRecipient.CallOffId
                : initialOrderWithCatalogueItem.CallOffId;

            return callOffId.ToString();
        }

        public ICollection<OrderSublocationRecipient> DetermineOrderRecipients(OrderItem orderItem)
        {
            return Order.DetermineOrderRecipients(Previous, orderItem.Id);
        }

        public bool CanComplete()
        {
            return Order.CanComplete(RolledUp.GetOrderRecipients().ToList(), OrderItems, Previous);
        }

        public OrderSublocationRecipient CreateRecipientWithExistingOrderContext(
            string recipientOdsCode,
            string parentSublocationOdsCode)
        {
            OrderSublocationRecipient newRecipient = new(recipientOdsCode, parentSublocationOdsCode);
            if (Order.DeliveryDate.HasValue)
            {
                Order.OrderItems.ToList().ForEach(i =>
                {
                    var item = i.Parent?.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                        ? i.Parent
                        : i;
                    if (Previous == null
                        || !Previous.Exists(item.CatalogueItemId)
                        || Previous.FlattenedRecipients.All(x => x.RecipientOdsCode != recipientOdsCode))
                    {
                        newRecipient.SetDeliveryDateForItem(i, Order.DeliveryDate.Value);
                    }
                });
            }

            if (Order.OrderType.MergerOrSplit)
            {
                Order.OrderItems.ToList().ForEach(i =>
                {
                    newRecipient.SetQuantityForItem(i, 1);
                });
            }

            return newRecipient;
        }

        public IEnumerable<OrderItemFundingType> FundingTypesForItem(CatalogueItemId catalogueItemId)
        {
            var fundingTypes = PreviousOrders
                .SelectMany(o => o.OrderItems.Where(oi => oi.CatalogueItemId == catalogueItemId).Select(oi => oi.FundingType))
                .ToList();

            fundingTypes.AddRange(Order.OrderItems.Where(oi => oi.CatalogueItemId == catalogueItemId).Select(oi => oi.FundingType));
            return fundingTypes.Distinct();
        }

        private IReadOnlyList<Order> GetPreviousOrders() => previous.Values.ToList();
    }
}
