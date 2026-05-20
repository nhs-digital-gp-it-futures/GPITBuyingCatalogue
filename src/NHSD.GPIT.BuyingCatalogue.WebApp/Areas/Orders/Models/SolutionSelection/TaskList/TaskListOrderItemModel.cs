using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList
{
    public class TaskListOrderItemModel
    {
        private readonly OrderItem rolledUpOrderItem;

        public TaskListOrderItemModel(
            string internalOrgId,
            CallOffId callOffId,
            OrderType orderType,
            IEnumerable<OrderSublocationRecipient> rolledUpOrderRecipients,
            OrderItem rolledUpOrderItem)
        {
            this.rolledUpOrderItem = rolledUpOrderItem;

            IsPerServiceRecipient = ((IPrice)rolledUpOrderItem.OrderItemPrice)?.IsPerServiceRecipient() ?? false;
            IsAssociatedService = rolledUpOrderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService;
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            OrderType = orderType;

            CatalogueItemId = rolledUpOrderItem?.CatalogueItemId ?? default;
            Name = rolledUpOrderItem?.CatalogueItem?.Name ?? string.Empty;
            RolledUpOrderRecipients = (rolledUpOrderRecipients ?? []).ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CallOffId QuantityViewCallOffId { get; set; }

        public OrderType OrderType { get; set; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool FromPreviousRevision { get; set; }

        public bool HasNewRecipients { get; set; }

        public List<OrderSublocationRecipient> RolledUpOrderRecipients { get; set; }

        public int PreviousRecipients { get; set; }

        public bool QuantityChanged { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string Name { get; set; }

        public int NumberOfPrices { get; set; }

        public int PriceId { get; set; }

        public bool IsPerServiceRecipient { get; set; }

        public bool IsAssociatedService { get; set; }

        public List<CatalogueItem> AssociatedServicesCatalogueItems { get; set; } = new();

        public List<OrderItem> AssociatedServicesOrderItems { get; set; } = new();

        public bool CanBeRemoved { get; set; }

        public int? OrderItemId { get; set; }

        public RoutingSource Source { get; set; }

        public TaskProgress PriceStatus
        {
            get
            {
                return GetPriceStatus(rolledUpOrderItem);
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

                if (IsAmendment && IsAssociatedService && FromPreviousRevision)
                {
                    return TaskProgress.Completed;
                }

                if (RolledUpOrderRecipients.AllQuantitiesEntered(rolledUpOrderItem))
                {
                    return FromPreviousRevision && HasNewRecipients ? TaskProgress.Amended : TaskProgress.Completed;
                }

                return RolledUpOrderRecipients.SomeNewQuantitiesEntered(rolledUpOrderItem)
                    ? TaskProgress.InProgress
                    : TaskProgress.NotStarted;
            }
        }

        public TaskProgress AssociatedServicesStatus
        {
            get
            {
                if (AssociatedServicesOrderItems.Count > 0)
                {
                    return AssociatedServicesOrderItems.All(orderItem =>
                        GetPriceStatus(orderItem) == TaskProgress.Completed
                    && RolledUpOrderRecipients.AllQuantitiesEntered(orderItem))
                        ? TaskProgress.Completed
                        : TaskProgress.InProgress;
                }

                return TaskProgress.Optional;
            }
        }

        private TaskProgress GetPriceStatus(OrderItem orderItem)
        {
            return (orderItem?.OrderItemPrice?.OrderItemPriceTiers?.Count ?? 0) == 0
                ? TaskProgress.NotStarted
                : TaskProgress.Completed;
        }
    }
}
