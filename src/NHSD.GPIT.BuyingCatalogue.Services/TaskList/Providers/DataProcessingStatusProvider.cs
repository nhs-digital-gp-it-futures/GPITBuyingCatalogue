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
            if (wrapper?.Order is null
                || state is null)
            {
                return TaskProgress.CannotStart;
            }

            if (!DependentTasksComplete(state))
            {
                return TaskProgress.CannotStart;
            }

            return wrapper.Order.ContractFlags?.UseDefaultDataProcessing == true
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;
        }

        private static bool DependentTasksComplete(OrderProgress state)
        {
            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.Amended };
            return !(!okToProgress.Contains(state.DescriptionStatus)
                || !okToProgress.Contains(state.OrderingPartyStatus)
                || !okToProgress.Contains(state.SupplierStatus)
                || !okToProgress.Contains(state.CommencementDateStatus)
                || !okToProgress.Contains(state.ServiceRecipients)
                || !okToProgress.Contains(state.SolutionOrService)
                || !okToProgress.Contains(state.DeliveryDates)
                || !okToProgress.Contains(state.FundingSource)
                || !okToProgress.Contains(state.ImplementationPlan)
                || !AssociatedServiceTaskComplete(state.AssociatedServiceBilling)
                || !AssociatedServiceTaskComplete(state.AssociatedServiceRequirements));
        }

        private static bool AssociatedServiceTaskComplete(TaskProgress associatedServiceTaskState)
        {
            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.NotApplicable };
            return okToProgress.Contains(associatedServiceTaskState);
        }
    }
}
