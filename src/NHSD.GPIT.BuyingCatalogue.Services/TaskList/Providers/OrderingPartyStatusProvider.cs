using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Comparers.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers
{
    public class OrderingPartyStatusProvider : ITaskProgressProvider
    {
        private static readonly IEqualityComparer<Contact> ContactComparer = new ContactEqualityComparer();

        public TaskProgress Get(OrderWrapper wrapper, OrderProgress state)
        {
            var order = wrapper?.Order;

            if (order == null)
            {
                return TaskProgress.CannotStart;
            }

            if (order.OrderingPartyContact == null)
                return TaskProgress.NotStarted;

            if (!order.IsAmendment)
                return TaskProgress.Completed;

            return ContactComparer.Equals(order.OrderingPartyContact, wrapper.Last.OrderingPartyContact)
                ? TaskProgress.Completed
                : TaskProgress.Amended;
        }
    }
}
