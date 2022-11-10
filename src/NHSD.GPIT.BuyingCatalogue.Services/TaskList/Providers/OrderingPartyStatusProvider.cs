using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class OrderingPartyStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            var order = wrapper?.Order;

            if (order == null)
            {
                return TaskProgress.CannotStart;
            }

            return order.OrderingPartyContact != null
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;
        }
    }
}
