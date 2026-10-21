using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

public class OrderItemDeliveryDate : DateInputModel
{
    public OrderItemDeliveryDate()
    {
    }

    public OrderItemDeliveryDate(
        int orderItemId,
        string catalogueItemName,
        DateTime? deliveryDate,
        Order order)
    {
        OrderSublocationRecipientId = orderItemId;
        CatalogueItemName = catalogueItemName;
        CommencementDate = order.CommencementDate;
        MaximumTerm = order.MaximumTerm;
        IsAmend = order.IsAmendment;
        SetDateFields(deliveryDate);
    }

    public int OrderSublocationRecipientId { get; init; }

    public string CatalogueItemName { get; init; }

    public DateTime? CommencementDate { get; set; }

    public int? MaximumTerm { get; set; }

    public bool IsAmend { get; set; }

    public DateTime? ContractEndDate
    {
        get
        {
            var endDate = new EndDate(CommencementDate, MaximumTerm);
            return endDate.DateTime;
        }
    }
}
