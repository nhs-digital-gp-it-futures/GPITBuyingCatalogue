using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;

public class OrderItemRecipientRowModel
{
    public OrderItemRecipientRowModel()
    {
    }

    public OrderItemRecipientRowModel(
        OrderSublocationRecipient recipient,
        AmendOrderItemModel amendOrderItemModel,
        string callOffId)
    {
        ServiceRecipient = recipient;
        IsAmendment = amendOrderItemModel.IsAmendment;
        CallOffId = callOffId;
        OrderItemId = amendOrderItemModel.OrderItem.Id;
        IsServiceRecipientAdded = amendOrderItemModel.IsServiceRecipientAdded(recipient.RecipientOdsCode);
        OrderType = amendOrderItemModel.OrderType;
        FromPreviousRevision = amendOrderItemModel.FromPreviousRevision;
    }

    public OrderSublocationRecipient ServiceRecipient { get; init; }

    public bool IsAmendment { get; init; }

    public string CallOffId { get; init; }

    public int OrderItemId { get; init; }

    public bool IsServiceRecipientAdded { get; init; }

    public OrderType OrderType { get; init; }

    public bool FromPreviousRevision { get; init; }
}
