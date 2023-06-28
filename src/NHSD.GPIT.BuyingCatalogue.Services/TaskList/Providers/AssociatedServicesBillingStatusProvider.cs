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

            var contractBillingEntered = order.Contract?.ContractBilling is not null;

            if ((state.FundingSource == TaskProgress.InProgress || state.ImplementationPlan == TaskProgress.InProgress)
                && contractBillingEntered)
            {
                return TaskProgress.InProgress;
            }

            if (state.FundingSource != TaskProgress.Completed
                || state.ImplementationPlan != TaskProgress.Completed)
            {
                return TaskProgress.CannotStart;
            }

            return contractBillingEntered
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
        }

        private static bool HasAssociatedServices(Order order) =>
            order.AssociatedServicesOnly
            || order.HasAssociatedService();
    }
}
