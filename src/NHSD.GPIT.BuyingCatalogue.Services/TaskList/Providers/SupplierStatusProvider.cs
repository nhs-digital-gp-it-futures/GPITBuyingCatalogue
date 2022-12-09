using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class SupplierStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order == null
                || state == null)
            {
                return TaskProgress.CannotStart;
            }

            if (state.OrderingPartyStatus != TaskProgress.Completed
                && state.OrderingPartyStatus != TaskProgress.Amended)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (order.Supplier == null)
            {
                return TaskProgress.NotStarted;
            }

            if (order.SupplierContact == null)
            {
                return TaskProgress.InProgress;
            }

            if (!order.IsAmendment)
            {
                return TaskProgress.Completed;
            }

            return order.SupplierContact.Equals(wrapper.Last?.SupplierContact)
                ? TaskProgress.Completed
                : TaskProgress.Amended;
        }
    }
}
