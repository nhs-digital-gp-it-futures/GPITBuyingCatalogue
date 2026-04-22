using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed class OrderSublocationRecipient : ICloneable<OrderSublocationRecipient>
    {
        public OrderSublocationRecipient()
        {
        }

        public OrderSublocationRecipient(string recipientOdsCode, string parentSublocationOdsCode)
        {
            RecipientOdsCode = recipientOdsCode;
            ParentSublocationOdsCode = parentSublocationOdsCode;
        }

        public OrderSublocationRecipient(CompetitionSublocationRecipient competitionSublocationRecipient)
        {
            RecipientOdsCode = competitionSublocationRecipient.RecipientOdsCode;
            ParentSublocationOdsCode = competitionSublocationRecipient.ParentSublocationOdsCode;
        }

        public int OrderId { get; set; }

        public string ParentSublocationOdsCode { get; set; }

        public string RecipientOdsCode { get; set; }

        public Order Order { get; set; }

        public OrderSublocation ParentSublocation { get; set; }

        public OdsOrganisation RecipientOdsOrganisation { get; set; }

        public ICollection<OrderItemSublocationRecipient> OrderItemSublocationRecipients { get; set; } = [];

        public void SetQuantityForItem(OrderItem orderItem, int? quantity)
        {
            OrderItemSublocationRecipient itemRecipient =
                OrderItemSublocationRecipients.FirstOrDefault(x => x.OrderItemId == orderItem.Id);

            if (itemRecipient is null)
            {
                itemRecipient = new OrderItemSublocationRecipient(OrderId, RecipientOdsCode, orderItem);
                OrderItemSublocationRecipients.Add(itemRecipient);
            }

            itemRecipient.Quantity = quantity;
        }

        public OrderSublocationRecipient Clone()
        {
            return new OrderSublocationRecipient
            {
                RecipientOdsCode = RecipientOdsCode,
                RecipientOdsOrganisation = RecipientOdsOrganisation,
                ParentSublocationOdsCode = ParentSublocationOdsCode,
                OrderItemSublocationRecipients = OrderItemSublocationRecipients.Select(x => x.Clone()).ToList(),
            };
        }

        public void SetDeliveryDateForItem(OrderItem orderItem, DateTime deliveryDate)
        {
            OrderItemSublocationRecipient itemRecipient =
                OrderItemSublocationRecipients.FirstOrDefault(x => x.OrderItemId == orderItem.Id);

            if (itemRecipient is null)
            {
                itemRecipient = new OrderItemSublocationRecipient(OrderId, RecipientOdsCode, orderItem);
                OrderItemSublocationRecipients.Add(itemRecipient);
            }

            itemRecipient.DeliveryDate = deliveryDate;
        }

        public int? GetQuantityForItem(int orderItemId)
        {
            return OrderItemSublocationRecipients
                .FirstOrDefault(x => x.OrderItemId == orderItemId)
                ?.Quantity;
        }

        public DateTime? GetDeliveryDateForItem(int orderItemId)
        {
            return OrderItemSublocationRecipients
                .FirstOrDefault(x => x.OrderItemId == orderItemId)
                ?.DeliveryDate;
        }
    }
}
