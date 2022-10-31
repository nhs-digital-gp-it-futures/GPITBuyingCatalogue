using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class DescriptionStatusProvider : IOrderProgressProvider
    {
        public TaskProgress Process(OrderWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            if (string.IsNullOrWhiteSpace(wrapper.Order.Description))
            {
                return TaskProgress.NotStarted;
            }

            if (wrapper.Last == null)
            {
                return TaskProgress.Completed;
            }

            return wrapper.Last.Description == wrapper.Order.Description
                ? TaskProgress.Completed
                : TaskProgress.Amended;
        }
    }
}
