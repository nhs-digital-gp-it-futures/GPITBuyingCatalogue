using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Review
{
    public class ReviewExpanderModel
    {
        public ReviewExpanderModel(
            IEnumerable<OrderRecipient> rolledUp,
            IEnumerable<OrderRecipient> previousRecipients,
            OrderItem orderItem,
            OrderItem previous,
            bool isAmendment)
        {
            IsAmendment = isAmendment;
            IsOrderItemAdded = orderItem != null && previous == null;
            OrderItem = orderItem;
            Previous = previous;
            RolledUpRecipients = rolledUp.ToList();
            PreviousRecipients = (previousRecipients ?? Enumerable.Empty<OrderRecipient>()).ToList();
        }

        public bool IsAmendment { get; }

        public bool IsOrderItemAdded { get; }

        public OrderItemPrice OrderItemPrice => OrderItem.OrderItemPrice;

        public CatalogueItem CatalogueItem => OrderItem.CatalogueItem;

        public List<OrderRecipient> RolledUpRecipients { get; }

        public int RolledUpTotalQuantity => OrderItem.TotalQuantity(RolledUpRecipients);

        public int PreviousTotalQuantity => Previous?.TotalQuantity(PreviousRecipients) ?? 0;

        private List<OrderRecipient> PreviousRecipients { get; }

        private OrderItem OrderItem { get; }

        private OrderItem Previous { get; }

        public bool IsServiceRecipientAdded(string odsCode)
        {
            var rolledUpRecipient = RolledUpRecipients?.FirstOrDefault(x => x.OdsCode == odsCode);
            var previousRecipient = PreviousRecipients?.FirstOrDefault(x => x.OdsCode == odsCode);

            return (rolledUpRecipient != null && previousRecipient == null)
                || Previous == null;

        }
    }
}
