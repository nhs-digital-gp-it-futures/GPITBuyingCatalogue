using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace BuyingCatalogueFunction.Notifications.ContractExpiry.Interfaces
{
    public interface IContractExpiryService
    {
        Task<List<Order>> GetOrdersNearingExpiry(DateTime day);

        Task RaiseExpiry(DateTime date, Order order, OrderExpiryEventTypeEnum eventType, EmailPreferenceType defaultEmailPreference);
    }
}
