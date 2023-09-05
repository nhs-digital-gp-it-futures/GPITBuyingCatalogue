using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class SolutionOrServiceStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order == null
                || state == null)
            {
                return TaskProgress.CannotStart;
            }

            if (state.ServiceRecipients != TaskProgress.Completed)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (!ValidCatalogueItems(order))
            {
                return TaskProgress.NotStarted;
            }

            return SolutionsCompleted(wrapper)
                ? CompletedOrAmended(order.IsAmendment)
                : TaskProgress.InProgress;
        }

        private static TaskProgress CompletedOrAmended(bool isAmendment)
        {
            return isAmendment ? TaskProgress.Amended : TaskProgress.Completed;
        }

        private static bool ValidCatalogueItems(EntityFramework.Ordering.Models.Order order)
        {
            if (order.IsAmendment)
            {
                return order.OrderItems.Any(
                    x => x.CatalogueItem.CatalogueItemType is CatalogueItemType.Solution
                        or CatalogueItemType.AdditionalService);
            }

            if (order.AssociatedServicesOnly)
            {
                return order.SolutionId != null;
            }

            return order.OrderItems.Any(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
        }

        private static bool SolutionsCompleted(OrderWrapper orderWrapper)
        {
            if (!orderWrapper.Order.OrderItems.Any())
            {
                return false;
            }

            return orderWrapper.Order.OrderItems.All(x =>
                x.CatalogueItem != null
                && x.OrderItemPrice != null
                && orderWrapper.DetermineOrderRecipients(x.CatalogueItemId)
                    .AllQuantitiesEntered(x)
                && (orderWrapper.Order.IsAmendment
                        ? orderWrapper.DetermineOrderRecipients(x.CatalogueItemId)
                            .AllDeliveryDatesEntered(x.CatalogueItemId)
                        : true));
        }
    }
}
