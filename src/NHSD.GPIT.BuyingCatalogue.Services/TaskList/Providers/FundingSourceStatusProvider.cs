using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class FundingSourceStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order == null
                || state == null)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;
            var anyFundingSourcesEntered = AnyFundingSourcesEntered(order);

            if (state.DeliveryDates != TaskProgress.Completed
                && !anyFundingSourcesEntered)
            {
                return TaskProgress.CannotStart;
            }

            return AllFundingSourcesEntered(order)
                ? CompletedOrAmended(order.IsAmendment)
                : (anyFundingSourcesEntered ? TaskProgress.InProgress : TaskProgress.NotStarted);
        }

        private static TaskProgress CompletedOrAmended(bool isAmendment)
        {
            return isAmendment ? TaskProgress.Amended : TaskProgress.Completed;
        }

        private static bool AllFundingSourcesEntered(EntityFramework.Ordering.Models.Order order)
        {
            return order.SelectedFramework != null
                && order.OrderItems.Any()
                && order.OrderItems.All(x => x.OrderItemFunding != null);
        }

        private static bool AnyFundingSourcesEntered(EntityFramework.Ordering.Models.Order order)
        {
            return order.OrderItems.Any(x => x.OrderItemFunding != null);
        }
    }
}
