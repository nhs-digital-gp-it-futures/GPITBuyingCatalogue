using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList
{
    public class TaskListOrderItemModel
    {
        private readonly OrderItem rolledUpOrderItem;

        public TaskListOrderItemModel(string internalOrgId, CallOffId callOffId, OrderType orderType, IEnumerable<OrderRecipient> rolledUpOrderRecipients, OrderItem rolledUpOrderItem)
        {
            this.rolledUpOrderItem = rolledUpOrderItem;

            IsPerServiceRecipient = ((IPrice)rolledUpOrderItem.OrderItemPrice)?.IsPerServiceRecipient() ?? false;
            IsAssociatedService = rolledUpOrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService;
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            OrderType = orderType;

            CatalogueItemId = rolledUpOrderItem?.CatalogueItemId ?? default;
            Name = rolledUpOrderItem?.CatalogueItem?.Name ?? string.Empty;
            RolledUpOrderRecipients = (rolledUpOrderRecipients ?? Enumerable.Empty<OrderRecipient>()).ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderType OrderType { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool FromPreviousRevision { get; set; }

        public bool HasNewRecipients { get; set; }

        public List<OrderRecipient> RolledUpOrderRecipients { get; set; }

        public int PreviousRecipients { get; set; }

        public bool QuantityChanged { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Name { get; set; }

        public int NumberOfPrices { get; set; }

        public int PriceId { get; set; }

        public int ItemNumber { get; set; }

        public bool IsPerServiceRecipient { get; set; }

        public bool IsAssociatedService { get; set; }

        public bool CanBeRemoved { get; set; }

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

                if (IsAmendment && IsAssociatedService)
                {
                    return TaskProgress.Completed;
                }

                if (!IsPerServiceRecipient && IsAmendment && FromPreviousRevision)
                {
                    return QuantityChanged ? TaskProgress.Amended : TaskProgress.Completed;
                }
                else
                {
                    if (RolledUpOrderRecipients.AllQuantitiesEntered(rolledUpOrderItem))
                    {
                        return FromPreviousRevision && HasNewRecipients ? TaskProgress.Amended : TaskProgress.Completed;
                    }
                    else if (RolledUpOrderRecipients.SomeButNotAllNewQuantitiesEntered(rolledUpOrderItem, PreviousRecipients))
                    {
                        return TaskProgress.InProgress;
                    }
                }

                return TaskProgress.NotStarted;
            }
        }
    }
}
