using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class ImplementationPlanStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order == null
                || state == null)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.Amended };

            if (!okToProgress.Contains(state.FundingSource))
            {
                return order.ContractFlags?.UseDefaultImplementationPlan != null
                    ? TaskProgress.InProgress
                    : TaskProgress.CannotStart;
            }

            return order.ContractFlags?.UseDefaultImplementationPlan != null
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;
        }
    }
}
