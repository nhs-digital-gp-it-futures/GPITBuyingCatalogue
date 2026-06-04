using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class SolutionOrServiceStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order is null
                || state is null
                || !TaskListStatusService.IsTaskCompleted(state.ServiceRecipients)
                || !TaskListStatusService.IsTaskCompleted(state.SupplierStatus))
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (!ValidCatalogueItems(wrapper))
            {
                return TaskProgress.NotStarted;
            }

            return SolutionsCompleted(wrapper)
                ? TaskListStatusService.CompletedOrAmended(order.IsAmendment)
                : TaskProgress.InProgress;
        }

        private static bool ValidCatalogueItems(OrderWrapper orderWrapper)
        {
            var order = orderWrapper.Order;

            if (orderWrapper.IsAmendment && !orderWrapper.HasNewOrderRecipients && !orderWrapper.HasNewOrderItems)
            {
                return false;
            }

            if (order.OrderType.AssociatedServicesOnly)
            {
                return order.AssociatedServicesOnlyDetails.SolutionId != null;
            }

            return order.OrderItems.Any(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
        }

        private static bool SolutionsCompleted(OrderWrapper orderWrapper)
        {
            if (orderWrapper.Order.OrderItems.Count == 0)
            {
                return false;
            }

            return orderWrapper.Order.OrderItems.All(x =>
            {
                ICollection<OrderSublocationRecipient> recipients =
                    orderWrapper.DetermineOrderRecipients(x.CatalogueItemId);
                var allQuantities = recipients.AllQuantitiesEntered(x);

                return x.CatalogueItem != null
                    && x.OrderItemPrice != null
                    && allQuantities;
            });
        }
    }
}
