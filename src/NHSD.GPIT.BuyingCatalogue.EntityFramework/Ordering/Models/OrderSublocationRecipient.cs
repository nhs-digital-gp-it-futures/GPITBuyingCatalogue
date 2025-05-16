using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public record OrderSublocationRecipient : ISublocationRecipient
    {
        public OrderSublocationRecipient()
        {
        }

        public OrderSublocationRecipient(OrderSublocationRecipient old)
        {
            RecipientOdsCode = old.RecipientOdsCode;
            ParentSublocationOdsCode = old.ParentSublocationOdsCode;
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

        public ICollection<OrderItemSublocationRecipient> OrderItemSublocationRecipients { get; set; } =
            new HashSet<OrderItemSublocationRecipient>();

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
