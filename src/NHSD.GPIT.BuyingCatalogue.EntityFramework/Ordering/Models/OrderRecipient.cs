using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

public class OrderRecipient
{
    public int OrderId { get; set; }

    public string OdsCode { get; set; }

    public Order Order { get; set; }

    public OdsOrganisation OdsOrganisation { get; set; }

    public ICollection<OrderItemRecipient> OrderItemRecipients { get; set; }

    public void SetQuantityForItem(CatalogueItemId catalogueItemId, int quantity)
    {
        var itemRecipient = OrderItemRecipients.FirstOrDefault(
            x => x.OrderId == OrderId && x.OdsCode == OdsCode && x.CatalogueItemId == catalogueItemId);

        if (itemRecipient == null)
        {
            itemRecipient = new OrderItemRecipient(OrderId, OdsCode, catalogueItemId);
            OrderItemRecipients.Add(itemRecipient);
        }

        itemRecipient.Quantity = quantity;
    }

    public int? GetQuantityForItem(CatalogueItemId catalogueItemId) => OrderItemRecipients.FirstOrDefault(
            x => x.OrderId == OrderId && x.OdsCode == OdsCode && x.CatalogueItemId == catalogueItemId)
        ?.Quantity;
}
