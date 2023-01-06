using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList
{
    public class TaskListOrderItemModel
    {
        public TaskListOrderItemModel()
        {
        }

        public TaskListOrderItemModel(string internalOrgId, CallOffId callOffId, OrderItem orderItem, bool hasCurrentAmendments = false)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;

            CatalogueItemId = orderItem?.CatalogueItemId ?? default;
            Name = orderItem?.CatalogueItem?.Name ?? string.Empty;
            HasCurrentAmendments = hasCurrentAmendments;

            ServiceRecipientsStatus = GetServiceRecipientsStatus(orderItem);
            PriceStatus = GetPriceStatus(orderItem);
            QuantityStatus = GetQuantityStatus(orderItem);
            DeliveryDatesStatus = GetDeliveryDateStatus(orderItem);
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool FromPreviousRevision { get; set; }

        public bool HasCurrentAmendments { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Name { get; set; }

        public int NumberOfPrices { get; set; }

        public int PriceId { get; set; }

        public TaskProgress ServiceRecipientsStatus { get; set; }

        public TaskProgress PriceStatus { get; set; }

        public TaskProgress QuantityStatus { get; set; }

        public TaskProgress DeliveryDatesStatus { get; set; }

        private static TaskProgress GetDeliveryDateStatus(OrderItem orderItem)
        {
            if (!orderItem.OrderItemRecipients.Any())
            {
                return TaskProgress.CannotStart;
            }

            if (orderItem.OrderItemRecipients.All(x => x.DeliveryDate.HasValue))
            {
                return TaskProgress.Completed;
            }

            return orderItem.OrderItemRecipients.Any(x => x.DeliveryDate.HasValue)
                ? TaskProgress.InProgress
                : TaskProgress.NotStarted;
        }

        private static TaskProgress GetServiceRecipientsStatus(OrderItem orderItem)
        {
            return (orderItem?.OrderItemRecipients?.Count ?? 0) == 0
                ? TaskProgress.NotStarted
                : TaskProgress.Completed;
        }

        private TaskProgress GetPriceStatus(OrderItem orderItem)
        {
            if (ServiceRecipientsStatus == TaskProgress.NotStarted)
            {
                return TaskProgress.CannotStart;
            }

            return (orderItem?.OrderItemPrice?.OrderItemPriceTiers?.Count ?? 0) == 0
                ? TaskProgress.NotStarted
                : TaskProgress.Completed;
        }

        private TaskProgress GetQuantityStatus(OrderItem orderItem)
        {
            if (ServiceRecipientsStatus == TaskProgress.NotStarted
                || PriceStatus == TaskProgress.NotStarted)
            {
                return TaskProgress.CannotStart;
            }

            if (orderItem?.OrderItemPrice == null)
            {
                return TaskProgress.CannotStart;
            }

            if (orderItem.OrderItemPrice.IsPerServiceRecipient())
            {
                if (orderItem.OrderItemRecipients?.All(x => x.Quantity != null) ?? false)
                {
                    return TaskProgress.Completed;
                }

                if (orderItem.OrderItemRecipients?.Any(x => x.Quantity != null) ?? false)
                {
                    return TaskProgress.InProgress;
                }
            }
            else
            {
                if (orderItem.Quantity != null)
                {
                    return TaskProgress.Completed;
                }
            }

            return TaskProgress.NotStarted;
        }
    }
}
