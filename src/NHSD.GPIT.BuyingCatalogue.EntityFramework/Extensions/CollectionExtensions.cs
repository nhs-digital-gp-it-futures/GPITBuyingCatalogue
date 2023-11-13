using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

public static class CollectionExtensions
{
    public static ICollection<OrderRecipient> ForCatalogueItem(this ICollection<OrderRecipient> recipients, CatalogueItemId catalogueItemId)
    {
        return recipients == null
            ? new List<OrderRecipient>()
            : recipients
                .Where(r => r.OrderItemRecipients.Any(oir => oir.CatalogueItemId == catalogueItemId))
                .ToList();
    }

    public static bool AllDeliveryDatesEntered(this ICollection<OrderRecipient> recipients, CatalogueItemId catalogueItemId)
    {
        return recipients != null && recipients.All(r => r.GetDeliveryDateForItem(catalogueItemId).HasValue);
    }

    public static bool NoDeliveryDatesEntered(this ICollection<OrderRecipient> recipients, CatalogueItemId catalogueItemId)
    {
        if (recipients != null)
        {
            return recipients.All(r => r.GetDeliveryDateForItem(catalogueItemId).HasValue == false);
        }

        return false;
    }

    public static bool AllQuantitiesEntered(this ICollection<OrderRecipient> recipients, OrderItem orderItem)
    {
        if (recipients == null || orderItem?.OrderItemPrice == null)
        {
            return false;
        }

        return ((IPrice)orderItem.OrderItemPrice).IsPerServiceRecipient()
            ? recipients.All(x => x.GetQuantityForItem(orderItem.CatalogueItemId).HasValue)
            : orderItem.Quantity.HasValue;
    }

    public static bool SomeButNotAllQuantitiesEntered(this ICollection<OrderRecipient> recipients, OrderItem orderItem)
    {
        if (orderItem.OrderItemPrice == null || recipients == null)
            return false;

        if (((IPrice)orderItem.OrderItemPrice).IsPerServiceRecipient())
        {
            var count = recipients.Count(x => x.GetQuantityForItem(orderItem.CatalogueItemId).HasValue);
            return count > 0 && count < recipients.Count;
        }

        return false;
    }

    public static bool Exists(this ICollection<OrderRecipient> recipients, string odsCode)
    {
        return recipients?.Get(odsCode) != null;
    }

    public static OrderRecipient Get(this ICollection<OrderRecipient> recipients, string odsCode)
    {
        return recipients?.FirstOrDefault(x => x.OdsCode == odsCode);
    }
}
