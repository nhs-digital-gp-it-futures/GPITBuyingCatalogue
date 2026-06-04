using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class AssociatedServicesMilestonesStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order is null
                || state is null)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (!TaskListStatusService.HasAssociatedServices(order))
            {
                return TaskProgress.NotApplicable;
            }

            if (!TaskListStatusService.IsTaskCompleted(state.SolutionOrService))
            {
                return TaskProgress.CannotStart;
            }

            var contractBillingEntered = order.Contract?.ContractBilling is not null;

            return contractBillingEntered
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;
        }
    }
}
