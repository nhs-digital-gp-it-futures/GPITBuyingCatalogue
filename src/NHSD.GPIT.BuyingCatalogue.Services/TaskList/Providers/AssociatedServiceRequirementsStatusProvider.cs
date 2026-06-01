using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

public class AssociatedServiceRequirementsStatusProvider : ITaskProgressProvider
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

        var requirementsEntered = order.Contract?.ContractBilling?.HasConfirmedRequirements ?? false;

        return requirementsEntered
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;
    }
}
