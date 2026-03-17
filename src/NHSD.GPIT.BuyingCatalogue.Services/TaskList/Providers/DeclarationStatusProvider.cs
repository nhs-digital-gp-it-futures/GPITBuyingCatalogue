using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

public class DeclarationStatusProvider : ITaskProgressProvider
{
    public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
    {
        if (wrapper?.Order == null
            || state is not { DataProcessingInformation: TaskProgress.Completed })
        {
            return TaskProgress.CannotStart;
        }

        return wrapper.Order.AcceptedTermsAndConditions ? TaskProgress.Completed : TaskProgress.NotStarted;
    }
}
