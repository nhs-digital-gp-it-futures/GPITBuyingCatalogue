using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class DataProcessingStatusProvider : ITaskProgressProvider
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

            if ((!okToProgress.Contains(state.FundingSource)
                || (state.AssociatedServiceBilling != TaskProgress.Completed && state.AssociatedServiceBilling != TaskProgress.NotApplicable))
                && order.ContractFlags?.UseDefaultDataProcessing == true)
            {
                return TaskProgress.InProgress;
            }

            if ((state.AssociatedServiceBilling == TaskProgress.Completed)
                || (state.AssociatedServiceBilling == TaskProgress.NotApplicable && state.ImplementationPlan == TaskProgress.Completed))
            {
                return order.ContractFlags?.UseDefaultDataProcessing == true
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }

            return TaskProgress.CannotStart;
        }
    }
}
