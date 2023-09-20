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

            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.Amended };
            if (!okToProgress.Contains(state.ServiceRecipients))
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (!ValidCatalogueItems(wrapper))
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

        private static bool ValidCatalogueItems(OrderWrapper orderWrapper)
        {
            var order = orderWrapper.Order;

            if (orderWrapper.IsAmendment && !orderWrapper.HasNewOrderRecipients && !orderWrapper.HasNewOrderItems)
            {
                return false;
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
            {
                var recpients = orderWrapper.DetermineOrderRecipients(x.CatalogueItemId);
                var allQuantites = recpients.AllQuantitiesEntered(x);
                var allDatesIfAmendment = orderWrapper.Order.IsAmendment
                        ? recpients.AllDeliveryDatesEntered(x.CatalogueItemId)
                        : true;

                return x.CatalogueItem != null
                    && x.OrderItemPrice != null
                    && allQuantites
                    && allDatesIfAmendment;
            });
        }
    }
}
