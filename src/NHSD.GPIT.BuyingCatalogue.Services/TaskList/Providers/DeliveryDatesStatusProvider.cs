using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
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
            var anyDeliveryDatesEntered = !order.OrderItems
                .All(x => wrapper
                    .DetermineOrderRecipients(x.CatalogueItemId)
                    .NoDeliveryDatesEntered(x.CatalogueItemId));
            var defaultDeliveryDateEntered = order.DeliveryDate.HasValue;

            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.Amended };

            if (!okToProgress.Contains(state.SolutionOrService)
                && !anyDeliveryDatesEntered)
            {
                return TaskProgress.CannotStart;
            }

            return order.HaveAllDeliveryDates(wrapper.RolledUp.OrderRecipients)
                ? order.IsAmendment ? TaskProgress.Amended : TaskProgress.Completed
                : (anyDeliveryDatesEntered || defaultDeliveryDateEntered ? TaskProgress.InProgress : TaskProgress.NotStarted);
        }
    }
}
