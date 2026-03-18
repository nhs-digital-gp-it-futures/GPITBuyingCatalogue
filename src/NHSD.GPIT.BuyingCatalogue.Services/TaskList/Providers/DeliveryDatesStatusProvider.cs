using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class DeliveryDatesStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order == null
                || state == null)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (!order.FlattenedRecipients.Any())
            {
                return TaskProgress.CannotStart;
            }

            var anyDeliveryDatesEntered = order.OrderItems
                .Any(x =>
                {
                    ICollection<OrderSublocationRecipient> recipients = wrapper
                        .DetermineOrderRecipients(x.CatalogueItemId);

                    return recipients.Any(y => y.GetDeliveryDateForItem(x.CatalogueItemId).HasValue);
                });

            var defaultDeliveryDateEntered = order.DeliveryDate.HasValue;

            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.Amended };

            if (!okToProgress.Contains(state.SolutionOrService)
                && !anyDeliveryDatesEntered)
            {
                return TaskProgress.CannotStart;
            }

            var allDeliveryDatesSet = order.HaveAllDeliveryDates(wrapper.RolledUp.GetOrderRecipients().ToList(), wrapper.Previous);

            if (allDeliveryDatesSet && (wrapper.HasNewOrderRecipients || wrapper.HasNewOrderItems))
            {
                return order.IsAmendment ? TaskProgress.Amended : TaskProgress.Completed;
            }

            if ((anyDeliveryDatesEntered || defaultDeliveryDateEntered) && wrapper.HasNewOrderRecipients)
            {
                return TaskProgress.InProgress;
            }

            return TaskProgress.NotStarted;
        }
    }
}
