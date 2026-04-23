using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

public static class CollectionExtensions
{
    public static ICollection<OrderSublocationRecipient> ForCatalogueItem(
        this ICollection<OrderSublocationRecipient> recipients,
        int orderItemId)
    {
        return recipients == null
            ? []
            : recipients
                .Where(r => r.OrderItemSublocationRecipients.Any(oir => oir.OrderItemId == orderItemId))
                .ToList();
    }

    public static bool AllDeliveryDatesEntered(
        this IEnumerable<OrderSublocationRecipient> recipients,
        int orderItemId)
    {
        ArgumentNullException.ThrowIfNull(recipients);

        return recipients.All(r => r.GetDeliveryDateForItem(orderItemId).HasValue);
    }

    public static bool AllQuantitiesEntered(this ICollection<OrderSublocationRecipient> recipients, OrderItem orderItem)
    {
        if (recipients == null || orderItem?.OrderItemPrice == null)
        {
            return false;
        }

        return recipients.All(x => x.GetQuantityForItem(orderItem.Id).HasValue);
    }

    public static bool SomeButNotAllNewQuantitiesEntered(
        this ICollection<OrderSublocationRecipient> recipients,
        OrderItem orderItem)
    {
        if (orderItem.OrderItemPrice == null || recipients == null)
            return false;

        var count = recipients.Count(x => x.GetQuantityForItem(orderItem.Id).HasValue);
        return count > 0;
    }
}
