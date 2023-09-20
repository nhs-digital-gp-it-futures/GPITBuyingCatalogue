using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList
{
    public class TaskListOrderItemModel
    {
        private readonly OrderItem rolledUpOrderItem;

        public TaskListOrderItemModel(string internalOrgId, CallOffId callOffId, IEnumerable<OrderRecipient> rolledUpOrderRecipients, OrderItem rolledUpOrderItem)
        {
            this.rolledUpOrderItem = rolledUpOrderItem;

            InternalOrgId = internalOrgId;
            CallOffId = callOffId;

            CatalogueItemId = rolledUpOrderItem?.CatalogueItemId ?? default;
            Name = rolledUpOrderItem?.CatalogueItem?.Name ?? string.Empty;
            RolledUpOrderRecipients = (rolledUpOrderRecipients ?? Enumerable.Empty<OrderRecipient>()).ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool FromPreviousRevision { get; set; }

        public bool HasNewRecipients { get; set; }

        public List<OrderRecipient> RolledUpOrderRecipients { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Name { get; set; }

        public int NumberOfPrices { get; set; }

        public int PriceId { get; set; }

        public TaskProgress PriceStatus
        {
            get
            {
                return (rolledUpOrderItem?.OrderItemPrice?.OrderItemPriceTiers?.Count ?? 0) == 0
                    ? TaskProgress.NotStarted
                    : TaskProgress.Completed;
            }
        }

        public TaskProgress QuantityStatus
        {
            get
            {
                if (PriceStatus == TaskProgress.NotStarted)
                {
                    return TaskProgress.CannotStart;
                }

                if (RolledUpOrderRecipients.AllQuantitiesEntered(rolledUpOrderItem))
                {
                    return FromPreviousRevision && HasNewRecipients ? TaskProgress.Amended : TaskProgress.Completed;
                }
                else if (RolledUpOrderRecipients.SomeButNotAllQuantitiesEntered(rolledUpOrderItem))
                {
                    return TaskProgress.InProgress;
                }

                return TaskProgress.NotStarted;
            }
        }

        public TaskProgress DeliveryDatesStatus
        {
            get
            {
                if (QuantityStatus is TaskProgress.CannotStart or TaskProgress.NotStarted)
                {
                    return TaskProgress.CannotStart;
                }

                if (RolledUpOrderRecipients.AllDeliveryDatesEntered(rolledUpOrderItem.CatalogueItemId))
                {
                    return FromPreviousRevision && HasNewRecipients ? TaskProgress.Amended : TaskProgress.Completed;
                }
                else if (!RolledUpOrderRecipients.NoDeliveryDatesEntered(rolledUpOrderItem.CatalogueItemId))
                {
                    return TaskProgress.InProgress;
                }

                return TaskProgress.NotStarted;
            }
        }
    }
}
