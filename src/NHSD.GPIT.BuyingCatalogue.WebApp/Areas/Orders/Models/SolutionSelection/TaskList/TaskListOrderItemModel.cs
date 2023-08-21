using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList
{
    public class TaskListOrderItemModel
    {
        private readonly OrderItem orderItem;

        public TaskListOrderItemModel(string internalOrgId, CallOffId callOffId, OrderItem orderItem)
        {
            this.orderItem = orderItem;

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;

            CatalogueItemId = orderItem?.CatalogueItemId ?? default;
            Name = orderItem?.CatalogueItem?.Name ?? string.Empty;
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

        public bool DisplayPriceViewLink
        {
            get
            {
                var priceStatus = PriceStatus;

                return FromPreviousRevision
                    && priceStatus != TaskProgress.NotApplicable
                    && priceStatus != TaskProgress.CannotStart;
            }
        }

        public TaskProgress ServiceRecipientsStatus
        {
            get
            {
                return (orderItem?.OrderItemRecipients?.Count ?? 0) == 0
                    ? TaskProgress.NotStarted
                    : (FromPreviousRevision && HasCurrentAmendments) ? TaskProgress.Amended : TaskProgress.Completed;
            }
        }

        public TaskProgress PriceStatus
        {
            get
            {
                if (ServiceRecipientsStatus == TaskProgress.NotStarted)
                {
                    return TaskProgress.CannotStart;
                }

                return (orderItem?.OrderItemPrice?.OrderItemPriceTiers?.Count ?? 0) == 0
                    ? TaskProgress.NotStarted
                    : TaskProgress.Completed;
            }
        }

        public TaskProgress QuantityStatus
        {
            get
            {
                if (ServiceRecipientsStatus == TaskProgress.NotStarted
                    || PriceStatus == TaskProgress.NotStarted)
                {
                    return TaskProgress.CannotStart;
                }

                if (((IPrice)orderItem.OrderItemPrice).IsPerServiceRecipient())
                {
                    if (orderItem.OrderItemRecipients?.All(x => x.Quantity != null) ?? false)
                    {
                        return (FromPreviousRevision && HasCurrentAmendments) ? TaskProgress.Amended : TaskProgress.Completed;
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
                        return (FromPreviousRevision && HasCurrentAmendments) ? TaskProgress.Amended : TaskProgress.Completed;
                    }
                }

                return TaskProgress.NotStarted;
            }
        }

        public TaskProgress DeliveryDatesStatus
        {
            get
            {
                if ((QuantityStatus is TaskProgress.CannotStart or TaskProgress.NotStarted)
                    || !orderItem.OrderItemRecipients.Any())
                {
                    return TaskProgress.CannotStart;
                }

                if (orderItem.OrderItemRecipients.All(x => x.DeliveryDate.HasValue))
                {
                    return (FromPreviousRevision && HasCurrentAmendments) ? TaskProgress.Amended : TaskProgress.Completed;
                }

                return orderItem.OrderItemRecipients.Any(x => x.DeliveryDate.HasValue)
                    ? TaskProgress.InProgress
                    : TaskProgress.NotStarted;
            }
        }
    }
}
