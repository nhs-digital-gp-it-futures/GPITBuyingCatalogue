using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review
{
    public class ReviewExpanderModel
    {
        private readonly OrderItem previous;

        public ReviewExpanderModel(OrderItem orderItem, OrderItem previous, bool isAmendment = false)
        {
            IsAmendment = isAmendment;
            IsOrderItemAdded = orderItem != null && previous == null;
            OrderItem = orderItem;

            this.previous = previous;
        }

        public bool IsAmendment { get; }

        public bool IsOrderItemAdded { get; }

        public OrderItem OrderItem { get; }

        public bool IsServiceRecipientAdded(string odsCode) =>
            OrderItem?.OrderItemRecipients?.FirstOrDefault(x => x.OdsCode == odsCode) != null
            && previous?.OrderItemRecipients?.FirstOrDefault(x => x.OdsCode == odsCode) == null;
    }
}
