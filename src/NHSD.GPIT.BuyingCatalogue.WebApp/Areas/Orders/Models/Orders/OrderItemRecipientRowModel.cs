using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
        IsServiceRecipientAdded = amendOrderItemModel.IsServiceRecipientAdded(recipient.RecipientOdsCode);
        OrderType = amendOrderItemModel.OrderType;
        FromPreviousRevision = amendOrderItemModel.FromPreviousRevision;

        CatalogueItemId = amendOrderItemModel.OrderItem.CatalogueItemId;
        OrderItemId = recipient.OrderItemSublocationRecipients
            .Where(OrderItemFilterPredicate)
            .OrderByDescending(x => x.OrderItemId)
            .FirstOrDefault()
            ?.OrderItemId ?? amendOrderItemModel.OrderItem.Id;
        return;

        bool OrderItemFilterPredicate(OrderItemSublocationRecipient oisr) =>
            oisr.OrderItem?.CatalogueItem.CatalogueItemType != CatalogueItemType.AssociatedService
                ? oisr.OrderItem?.CatalogueItemId == CatalogueItemId
                : oisr.OrderItem.Id == amendOrderItemModel.OrderItem.Id;
    }

    public OrderSublocationRecipient ServiceRecipient { get; init; }

    public bool IsAmendment { get; init; }

    public string CallOffId { get; init; }

    public CatalogueItemId CatalogueItemId { get; init; }

    public int OrderItemId { get; init; }

    public bool IsServiceRecipientAdded { get; init; }

    public OrderType OrderType { get; init; }

    public bool FromPreviousRevision { get; init; }
}
