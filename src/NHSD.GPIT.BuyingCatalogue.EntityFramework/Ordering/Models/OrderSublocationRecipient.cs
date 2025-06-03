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

        public OrderSublocationRecipient(int orderId, string recipientOdsCode, string parentSublocationOdsCode)
        {
            OrderId = orderId;
            RecipientOdsCode = recipientOdsCode;
            ParentSublocationOdsCode = parentSublocationOdsCode;
        }

        public OrderSublocationRecipient(CompetitionSublocationRecipient competitionSublocationRecipient)
        {
            RecipientOdsCode = competitionSublocationRecipient.RecipientOdsCode;
            ParentSublocationOdsCode = competitionSublocationRecipient.ParentSublocationOdsCode;
        }

        public int OrderId { get; set; }

        public string RecipientOdsCode { get; set; }

        public string ParentSublocationOdsCode { get; set; }

        public Order Order { get; set; }

        public OrderSublocation ParentSublocation { get; set; }

        public OdsOrganisation RecipientOdsOrganisation { get; set; }

        public ICollection<OrderItemSublocationRecipient> OrderItemSublocationRecipients { get; set; } = [];

        public void SetQuantityForItem(CatalogueItemId catalogueItemId, int quantity)
        {
            OrderItemSublocationRecipient itemRecipient =
                OrderItemSublocationRecipients.FirstOrDefault(x => x.CatalogueItemId == catalogueItemId);

            if (itemRecipient is null)
            {
                itemRecipient = new OrderItemSublocationRecipient(OrderId, RecipientOdsCode, catalogueItemId);
                OrderItemSublocationRecipients.Add(itemRecipient);
            }

            itemRecipient.Quantity = quantity;
        }

        public OrderSublocationRecipient Clone()
        {
            return new OrderSublocationRecipient
            {
                RecipientOdsOrganisation = RecipientOdsOrganisation,
                RecipientOdsCode = RecipientOdsCode,
                ParentSublocation = ParentSublocation,
                ParentSublocationOdsCode = ParentSublocationOdsCode,
                OrderItemSublocationRecipients = [],
            };
        }

        public void SetDeliveryDateForItem(CatalogueItemId catalogueItemId, DateTime deliveryDate)
        {
            OrderItemSublocationRecipient itemRecipient =
                OrderItemSublocationRecipients.FirstOrDefault(x => x.CatalogueItemId == catalogueItemId);

            if (itemRecipient is null)
            {
                itemRecipient = new OrderItemSublocationRecipient(OrderId, RecipientOdsCode, catalogueItemId);
                OrderItemSublocationRecipients.Add(itemRecipient);
            }

            itemRecipient.DeliveryDate = deliveryDate;
        }

        public int? GetQuantityForItem(CatalogueItemId catalogueItemId)
        {
            return OrderItemSublocationRecipients
                .FirstOrDefault(x => x.CatalogueItemId == catalogueItemId)
                ?.Quantity;
        }

        public DateTime? GetDeliveryDateForItem(CatalogueItemId catalogueItemId)
        {
            return OrderItemSublocationRecipients
                .FirstOrDefault(x => x.CatalogueItemId == catalogueItemId)
                ?.DeliveryDate;
        }
    }
}
