using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace BuyingCatalogueFunction.Notifications.Interfaces
{
    public interface IContractExpiryService
    {
        Task<List<Order>> GetOrdersNearingExpiry(DateTime day);

        Task<EmailPreferenceType> GetDefaultEmailPreference(EventTypeEnum eventType);

        Task RaiseExpiry(DateTime date, Order order, EventTypeEnum eventType, EmailPreferenceType defaultEmailPreference);
    }
}
