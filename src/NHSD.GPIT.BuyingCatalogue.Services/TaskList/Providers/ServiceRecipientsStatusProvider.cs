﻿using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

public class ServiceRecipientsStatusProvider : ITaskProgressProvider
{
    public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
    {
        if (wrapper?.Order == null
            || state == null)
        {
            return TaskProgress.CannotStart;
        }

        if (state.CommencementDateStatus != TaskProgress.Completed)
        {
            return TaskProgress.CannotStart;
        }

        if (wrapper.HasNewOrderRecipients)
        {
            return wrapper.IsAmendment ? TaskProgress.Amended : TaskProgress.Completed;
        }

        if (wrapper.IsAmendment && wrapper.Order.OrderRecipients.Any())
        {
            return TaskProgress.Completed;
        }

        return TaskProgress.NotStarted;
    }
}
