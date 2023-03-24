using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review
{
    public class ReviewExpanderModel
    {
        public ReviewExpanderModel(OrderItem orderItem, OrderItem previous, bool isAmendment)
        {
            IsAmendment = isAmendment;
            IsOrderItemAdded = orderItem != null && previous == null;
            OrderItem = orderItem;
            Previous = previous;
        }

        public bool IsAmendment { get; }

        public bool IsOrderItemAdded { get; }

        public OrderItemPrice OrderItemPrice => OrderItem.OrderItemPrice;

        public CatalogueItem CatalogueItem => OrderItem.CatalogueItem;

        public ICollection<OrderItemRecipient> OrderItemRecipients => OrderItem.OrderItemRecipients;

        public int RolledUpTotalQuantity => OrderItem.TotalQuantity;

        public int PreviousTotalQuantity => Previous?.TotalQuantity ?? 0;

        private OrderItem OrderItem { get; }

        private OrderItem Previous { get; }

        public bool IsServiceRecipientAdded(string odsCode) =>
            OrderItem?.OrderItemRecipients?.FirstOrDefault(x => x.OdsCode == odsCode) != null
            && Previous?.OrderItemRecipients?.FirstOrDefault(x => x.OdsCode == odsCode) == null;
    }
}
