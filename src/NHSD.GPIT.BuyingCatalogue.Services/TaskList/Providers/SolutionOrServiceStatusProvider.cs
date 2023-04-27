using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

            if (state.CommencementDateStatus != TaskProgress.Completed)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (!ValidCatalogueItems(order))
            {
                return TaskProgress.NotStarted;
            }

            return SolutionsCompleted(order)
                ? CompletedOrAmended(order.IsAmendment)
                : TaskProgress.InProgress;
        }

        private static TaskProgress CompletedOrAmended(bool isAmendment)
        {
            return isAmendment ? TaskProgress.Amended : TaskProgress.Completed;
        }

        private static bool ValidCatalogueItems(EntityFramework.Ordering.Models.Order order)
        {
            if (!order.IsAmendment)
            {
              if (order.AssociatedServicesOnly)
              {
                  return order.SolutionId != null;
              }

              return order.OrderItems.Any(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
            }
            else
            {
                return order.OrderItems.Any(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                || x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService);
            }
        }

        private static bool SolutionsCompleted(EntityFramework.Ordering.Models.Order order)
        {
            if (!order.OrderItems.Any())
            {
                return false;
            }

            return order.OrderItems.All(x =>
                x.CatalogueItem != null
                && x.OrderItemPrice != null
                && (x.OrderItemRecipients?.Any() ?? false)
                && x.AllQuantitiesEntered)
                && AllDeliveryDatesEnteredIfRequired(order);
        }

        private static bool AllDeliveryDatesEnteredIfRequired(EntityFramework.Ordering.Models.Order order)
        {
            if (order.IsAmendment)
            {
                var recipients = order.OrderItems
                    .SelectMany(x => x.OrderItemRecipients)
                    .ToList();

                return recipients.Any()
                    && recipients.All(x => x.DeliveryDate != null);
            }

            return true;
        }
    }
}
