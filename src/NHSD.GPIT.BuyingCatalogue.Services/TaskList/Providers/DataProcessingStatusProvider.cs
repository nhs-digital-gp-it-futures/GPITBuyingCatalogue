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
            return !(!TaskListStatusService.IsTaskCompleted(state.DescriptionStatus)
                || !TaskListStatusService.IsTaskCompleted(state.OrderingPartyStatus)
                || !TaskListStatusService.IsTaskCompleted(state.SupplierStatus)
                || !TaskListStatusService.IsTaskCompleted(state.CommencementDateStatus)
                || !TaskListStatusService.IsTaskCompleted(state.ServiceRecipients)
                || !TaskListStatusService.IsTaskCompleted(state.SolutionOrService)
                || !TaskListStatusService.IsTaskCompleted(state.DeliveryDates)
                || !TaskListStatusService.IsTaskCompleted(state.FundingSource)
                || !IsTaskCompletedOrNotApplicable(state.ImplementationPlan)
                || !IsTaskCompletedOrNotApplicable(state.AssociatedServiceBilling)
                || !IsTaskCompletedOrNotApplicable(state.AssociatedServiceRequirements));
        }

        private static bool IsTaskCompletedOrNotApplicable(TaskProgress associatedServiceTaskState)
        {
            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.NotApplicable };
            return okToProgress.Contains(associatedServiceTaskState);
        }
    }
}
