using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IServiceRecipientService
    {
        Task<List<ServiceRecipient>> GetAllOrderItemRecipients(CallOffId callOffId, string internalOrgId);

        Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(
            IEnumerable<ServiceRecipient> recipients);
    }
}
