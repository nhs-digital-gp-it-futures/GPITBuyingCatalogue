using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class SupplierStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.Amended };

            if (wrapper?.Order is null
                || state is null
                || !okToProgress.Contains(state.DescriptionStatus))
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if (order.Supplier is null)
            {
                return TaskProgress.NotStarted;
            }

            if (order.SupplierContact is null)
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
