using System.Linq;
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
            var anyDeliveryDatesEntered = AnyDeliveryDatesEntered(order);

            if (state.SolutionOrService != TaskProgress.Completed
                && !anyDeliveryDatesEntered)
            {
                return TaskProgress.CannotStart;
            }

            return AllDeliveryDatesEntered(order)
                ? TaskProgress.Completed
                : (anyDeliveryDatesEntered ? TaskProgress.InProgress : TaskProgress.NotStarted);
        }

        private static bool AllDeliveryDatesEntered(EntityFramework.Ordering.Models.Order order)
        {
            var recipients = order.OrderItems
                .SelectMany(x => x.OrderItemRecipients)
                .ToList();

            return recipients.Any()
                && recipients.All(x => x.DeliveryDate != null);
        }

        private static bool AnyDeliveryDatesEntered(EntityFramework.Ordering.Models.Order order)
        {
            return order.OrderItems
                .SelectMany(x => x.OrderItemRecipients)
                .Any(x => x.DeliveryDate != null);
        }
    }
}
