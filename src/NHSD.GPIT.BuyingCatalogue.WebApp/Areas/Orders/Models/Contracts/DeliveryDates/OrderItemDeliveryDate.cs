using System;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

public class OrderItemDeliveryDate : DateInputModel
{
    public OrderItemDeliveryDate()
    {
    }

    public OrderItemDeliveryDate(int orderItemId, string catalogueItemName, DateTime? deliveryDate)
    {
        OrderSublocationRecipientId = orderItemId;
        CatalogueItemName = catalogueItemName;
        SetDateFields(deliveryDate);
    }

    public int OrderSublocationRecipientId { get; init; }

    public string CatalogueItemName { get; init; }
}
