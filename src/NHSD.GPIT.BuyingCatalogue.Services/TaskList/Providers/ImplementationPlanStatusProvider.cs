using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class ImplementationPlanStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order is null
                || state is null
                || !TaskListStatusService.IsTaskCompleted(state.DescriptionStatus))
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (!order.OrderType.ImplementationPlanRequired)
            {
                return TaskProgress.NotApplicable;
            }

            return order.Contract?.ImplementationPlan is not null
                ? TaskListStatusService.CompletedOrAmended(order.IsAmendment)
                : TaskProgress.NotStarted;
        }
    }
}
