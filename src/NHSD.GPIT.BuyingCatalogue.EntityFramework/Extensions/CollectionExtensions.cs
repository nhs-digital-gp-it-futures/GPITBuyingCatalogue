using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

public static class CollectionExtensions
{
    public static ICollection<OrderSublocationRecipient> ForCatalogueItem(
        this ICollection<OrderSublocationRecipient> recipients,
        CatalogueItemId catalogueItemId)
    {
        return recipients == null
            ? []
            : recipients
                .Where(r => r.OrderItemSublocationRecipients.Any(oir => oir.CatalogueItemId == catalogueItemId))
                .ToList();
    }

    public static bool AllDeliveryDatesEntered(
        this ICollection<OrderSublocationRecipient> recipients,
        CatalogueItemId catalogueItemId)
    {
        if (recipients is not { Count: > 0 })
        {
            throw new ArgumentException("Recipients cannot be null or empty");
        }

        return recipients.All(r => r.GetDeliveryDateForItem(catalogueItemId).HasValue);
    }

    public static bool NoDeliveryDatesEntered(
        this ICollection<OrderSublocationRecipient> recipients,
        CatalogueItemId catalogueItemId)
    {
        if (recipients is not { Count: > 0 })
        {
            throw new ArgumentException("Recipients cannot be null or empty");
        }

        return recipients.All(r => r.GetDeliveryDateForItem(catalogueItemId).HasValue == false);
    }

    public static bool AllQuantitiesEntered(this ICollection<OrderSublocationRecipient> recipients, OrderItem orderItem)
    {
        if (recipients == null || orderItem?.OrderItemPrice == null)
        {
            return false;
        }

        return ((IPrice)orderItem.OrderItemPrice).IsPerServiceRecipient()
            ? recipients.All(x => x.GetQuantityForItem(orderItem.CatalogueItemId).HasValue)
            : orderItem.Quantity.HasValue;
    }

    public static bool SomeButNotAllNewQuantitiesEntered(
        this ICollection<OrderSublocationRecipient> recipients,
        OrderItem orderItem,
        int previousRecipients = 0)
    {
        if (orderItem.OrderItemPrice == null || recipients == null)
            return false;

        if (((IPrice)orderItem.OrderItemPrice).IsPerServiceRecipient())
        {
            var count = recipients.Count(x => x.GetQuantityForItem(orderItem.CatalogueItemId).HasValue) - previousRecipients;
            return count > 0 && count < recipients.Count - previousRecipients;
        }

        return false;
    }
}
