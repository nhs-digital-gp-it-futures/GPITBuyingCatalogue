using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

public class ServiceRecipientsStatusProvider : ITaskProgressProvider
{
    public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
    {
        if (wrapper?.Order is null
            || state is null || state.CommencementDateStatus != TaskProgress.Completed)
        {
            return TaskProgress.CannotStart;
        }

        if (wrapper.Order.HasSublocationsWithNoRecipients())
        {
            return TaskProgress.InProgress;
        }

        if (!wrapper.FlattenedRecipients.Any())
        {
            return TaskProgress.NotStarted;
        }

        if (wrapper.HasNewOrderRecipients && wrapper.IsAmendment)
        {
            return TaskProgress.Amended;
        }

        return TaskProgress.Completed;
    }
}
