using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class CommencementDateStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order is null
                || state is null
                || !TaskListStatusService.IsTaskCompleted(state.DescriptionStatus))
            {
                return TaskProgress.CannotStart;
            }

            var values = new[]
            {
                wrapper.Order.CommencementDate.HasValue,
                wrapper.Order.InitialPeriod.HasValue,
                wrapper.Order.MaximumTerm.HasValue,
            };

            return values.All(x => x)
                ? TaskProgress.Completed
                : values.All(x => !x)
                    ? TaskProgress.NotStarted
                    : TaskProgress.InProgress;
        }
    }
}
