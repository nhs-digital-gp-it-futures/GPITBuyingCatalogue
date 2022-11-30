using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class DescriptionStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            var order = wrapper?.Order;

            if (order == null)
            {
                return TaskProgress.CannotStart;
            }

            if (string.IsNullOrWhiteSpace(order.Description))
            {
                return TaskProgress.NotStarted;
            }

            if (!order.IsAmendment)
            {
                return TaskProgress.Completed;
            }

            return string.Equals(order.Description, wrapper.Last.Description, StringComparison.InvariantCulture)
                ? TaskProgress.Completed
                : TaskProgress.Amended;
        }
    }
}
