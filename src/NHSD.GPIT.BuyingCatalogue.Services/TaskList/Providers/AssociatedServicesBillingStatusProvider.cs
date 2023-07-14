using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class AssociatedServicesBillingStatusProvider : ITaskProgressProvider
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

            var fundingSourceStatus = new[] { TaskProgress.Completed, TaskProgress.Amended };
            var planStatus = new[] { TaskProgress.Completed, TaskProgress.NotApplicable };
            var contractBillingEntered = order.Contract?.ContractBilling is not null;
            var requirementsEntered = order.Contract?.ContractBilling?.HasConfirmedRequirements ?? false;

            if ((!fundingSourceStatus.Contains(state.FundingSource)
                || !planStatus.Contains(state.ImplementationPlan))
                && contractBillingEntered)
            {
                return TaskProgress.InProgress;
            }

            if ((state.ImplementationPlan != TaskProgress.Completed)
                && (state.ImplementationPlan != TaskProgress.NotApplicable
                    || state.FundingSource != TaskProgress.Completed))
                return TaskProgress.CannotStart;

            if (contractBillingEntered || requirementsEntered)
            {
                return contractBillingEntered && requirementsEntered
                    ? TaskProgress.Completed
                    : TaskProgress.InProgress;
            }

            return TaskProgress.NotStarted;
        }

        private static bool HasAssociatedServices(Order order) =>
            order.AssociatedServicesOnly
            || order.HasAssociatedService();
    }
}
