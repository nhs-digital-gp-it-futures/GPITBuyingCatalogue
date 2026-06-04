using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

public class ServiceRecipientsStatusProvider : ITaskProgressProvider
{
    public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
    {
        if (wrapper?.Order is null
            || state is null
            || !TaskListStatusService.IsTaskCompleted(state.DescriptionStatus))
        {
            return TaskProgress.CannotStart;
        }

        if (wrapper.Order.OrderSublocations.Count == 0)
        {
            return TaskProgress.NotStarted;
        }

        if (wrapper.Order.HasSublocationsWithNoRecipients() || (wrapper.Order.OrderType.MergerOrSplit
                && string.IsNullOrWhiteSpace(
                    wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode)))
        {
            return TaskProgress.InProgress;
        }

        if (wrapper.HasNewOrderRecipients && wrapper.IsAmendment)
        {
            return TaskProgress.Amended;
        }

        return TaskProgress.Completed;
    }
}
