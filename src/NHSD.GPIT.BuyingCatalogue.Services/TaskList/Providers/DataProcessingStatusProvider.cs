﻿using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class DataProcessingStatusProvider : ITaskProgressProvider
    {
        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            if (wrapper?.Order == null
                || state == null)
            {
                return TaskProgress.CannotStart;
            }

            var order = wrapper.Order;

            if ((state.FundingSource != TaskProgress.Completed
                || (state.AssociatedServiceBilling != TaskProgress.Completed && state.AssociatedServiceBilling != TaskProgress.NotApplicable))
                && order.ContractFlags?.UseDefaultDataProcessing != null)
            {
                return TaskProgress.InProgress;
            }

            if ((state.AssociatedServiceBilling == TaskProgress.Completed)
                || (state.AssociatedServiceBilling == TaskProgress.NotApplicable && state.ImplementationPlan == TaskProgress.Completed))
            {
                return order.ContractFlags?.UseDefaultDataProcessing != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }

            return TaskProgress.CannotStart;
        }
    }
}
