using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

public class OrderRecipient
{
    public OrderRecipient()
    {
    }

    public OrderRecipient(
        string odsCode)
    {
        OdsCode = odsCode;
    }

    public OrderRecipient(
        int orderId,
        string odsCode)
        : this(odsCode)
    {
        OrderId = orderId;
    }

    public int OrderId { get; set; }

    public string OdsCode { get; set; }

    public Order Order { get; set; }

    public OdsOrganisation OdsOrganisation { get; set; }

    public ICollection<OrderItemRecipient> OrderItemRecipients { get; set; } = new HashSet<OrderItemRecipient>();

    public void SetQuantityForItem(CatalogueItemId catalogueItemId, int quantity)
    {
        var itemRecipient = OrderItemRecipients.FirstOrDefault(
            x => x.CatalogueItemId == catalogueItemId);

        if (itemRecipient == null)
        {
            itemRecipient = new OrderItemRecipient(OrderId, OdsCode, catalogueItemId);
            OrderItemRecipients.Add(itemRecipient);
        }

        itemRecipient.Quantity = quantity;
    }

    public void SetDeliveryDateForItem(CatalogueItemId catalogueItemId, DateTime deliveryDate)
    {
        var itemRecipient = OrderItemRecipients.FirstOrDefault(
            x => x.CatalogueItemId == catalogueItemId);

        if (itemRecipient == null)
        {
            itemRecipient = new OrderItemRecipient(OrderId, OdsCode, catalogueItemId);
            OrderItemRecipients.Add(itemRecipient);
        }

        itemRecipient.DeliveryDate = deliveryDate;
    }

    public int? GetQuantityForItem(CatalogueItemId catalogueItemId) => OrderItemRecipients
        .FirstOrDefault(x => x.CatalogueItemId == catalogueItemId)
        ?.Quantity;

    public DateTime? GetDeliveryDateForItem(CatalogueItemId catalogueItemId) => OrderItemRecipients
        .FirstOrDefault(x => x.CatalogueItemId == catalogueItemId)
        ?.DeliveryDate;
}
