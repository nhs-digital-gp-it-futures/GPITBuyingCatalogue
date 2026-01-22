using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

public class AssociatedServiceRequirementsStatusProvider : ITaskProgressProvider
{
    public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
    {
        if (wrapper?.Order == null
            || state == null)
        {
            return TaskProgress.CannotStart;
        }

        var order = wrapper.Order;
        if (order.IsAmendment || !HasAssociatedServices(order))
        {
            return TaskProgress.NotApplicable;
        }

        var planStatus = new[] { TaskProgress.Completed, TaskProgress.NotApplicable };
        var requirementsEntered = order.Contract?.ContractBilling?.HasConfirmedRequirements ?? false;

        if (!planStatus.Contains(state.AssociatedServiceBilling)
            && requirementsEntered)
        {
            return TaskProgress.InProgress;
        }

        if (state.AssociatedServiceBilling != TaskProgress.Completed)
            return TaskProgress.CannotStart;

        return requirementsEntered
            ? TaskProgress.Completed
            : TaskProgress.NotStarted;
    }

    private static bool HasAssociatedServices(Order order) =>
        order.OrderType.AssociatedServicesOnly
        || order.HasAssociatedService();
}
