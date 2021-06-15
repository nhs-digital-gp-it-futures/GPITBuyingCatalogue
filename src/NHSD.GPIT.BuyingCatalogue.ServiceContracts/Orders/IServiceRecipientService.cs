using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IServiceRecipientService
    {
        Task<List<ServiceRecipient>> GetAllOrderItemRecipients(CallOffId callOffId);

        Task<IReadOnlyDictionary<string, ServiceRecipient>> AddOrUpdateServiceRecipients(
            IEnumerable<ServiceRecipient> recipients);
    }
}
