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
            if (wrapper?.Order == null
                || state == null)
            {
                return TaskProgress.CannotStart;
            }

            var anyFundingSourcesEntered = AnyFundingSourcesEntered(wrapper.OrderItems);

            var okToProgress = new[] { TaskProgress.Completed, TaskProgress.Amended };

            if (!okToProgress.Contains(state.DeliveryDates)
                && !anyFundingSourcesEntered)
            {
                return TaskProgress.CannotStart;
            }

            return AllFundingSourcesEntered(wrapper)
                ? CompletedOrAmended(wrapper.IsAmendment)
                : (anyFundingSourcesEntered ? TaskProgress.InProgress : TaskProgress.NotStarted);
        }

        private static TaskProgress CompletedOrAmended(bool isAmendment)
        {
            return isAmendment ? TaskProgress.Amended : TaskProgress.Completed;
        }

        private static bool AllFundingSourcesEntered(OrderWrapper wrapper)
        {
            return wrapper.Order.SelectedFramework != null
                && wrapper.OrderItems.Any()
                && wrapper.OrderItems.All(x => x.OrderItemFunding != null);
        }

        private static bool AnyFundingSourcesEntered(ICollection<OrderItem> orderItems)
        {
            return orderItems.Any(x => x.OrderItemFunding != null);
        }
    }
}
