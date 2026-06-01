using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class FundingSourceStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order is null
                || state is null)
            {
                return TaskProgress.CannotStart;
            }

            var anyFundingSourcesEntered = AnyFundingSourcesEntered(wrapper.OrderItems);

            if (!TaskListStatusService.IsTaskCompleted(state.SolutionOrService)
                && !anyFundingSourcesEntered)
            {
                return TaskProgress.CannotStart;
            }

            return AllFundingSourcesEntered(wrapper)
                ? TaskListStatusService.CompletedOrAmended(wrapper.IsAmendment)
                : (anyFundingSourcesEntered ? TaskProgress.InProgress : TaskProgress.NotStarted);
        }

        private static bool AllFundingSourcesEntered(OrderWrapper wrapper)
        {
            return wrapper.Order.SelectedFramework != null
                && wrapper.OrderItems.Count != 0
                && wrapper.OrderItems.All(x => x.OrderItemFunding != null);
        }

        private static bool AnyFundingSourcesEntered(ICollection<OrderItem> orderItems)
        {
            return orderItems.Any(x => x.OrderItemFunding != null);
        }
    }
}
