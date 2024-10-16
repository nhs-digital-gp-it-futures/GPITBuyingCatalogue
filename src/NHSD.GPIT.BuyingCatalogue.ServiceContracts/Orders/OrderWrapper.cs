﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public class OrderWrapper
    {
        private readonly List<Order> previous = new();
        private Lazy<Order> previousLazy;
        private Lazy<Order> rolledUpLazy;

        public OrderWrapper()
        {
            previousLazy = new Lazy<Order>((Order)null);
            rolledUpLazy = new Lazy<Order>((Order)null);
        }

        [Obsolete("Only used in tests - we should look to remove this")]
        public OrderWrapper(Order order)
        {
            Order = order;
            previousLazy = new Lazy<Order>((Order)null);
            rolledUpLazy = new Lazy<Order>(() => Order.Clone());
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

            previousLazy = new Lazy<Order>(() =>
            {
                if (!previous.Any())
                {
                    return null;
                }

                var output = previous.First().Clone();

                previous.Skip(1).ToList().ForEach(p => output.Apply(p.Clone()));

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

        public Order Order { get; set; }

        public Order Previous => previousLazy.Value;

        public Order RolledUp => rolledUpLazy.Value;

        public static OrderWrapper Create(IEnumerable<Order> orders, CallOffId requestedCallOffId)
        {
            var wrapper = new OrderWrapper(orders);
            if (wrapper.Order == null)
            {
                throw new InvalidOperationException($"Order not found {requestedCallOffId}");
            }

            if (wrapper.Order.CallOffId != requestedCallOffId)
            {
                throw new InvalidOperationException($"Latest order does not match {requestedCallOffId}");
            }

            return wrapper;
        }

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
