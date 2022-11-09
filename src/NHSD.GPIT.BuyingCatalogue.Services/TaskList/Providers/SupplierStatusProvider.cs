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

            if (state.OrderingPartyStatus != TaskProgress.Completed)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (order.Supplier != null)
            {
                return order.SupplierContact != null
                    ? TaskProgress.Completed
                    : TaskProgress.InProgress;
            }

            return TaskProgress.NotStarted;
        }
    }
}
