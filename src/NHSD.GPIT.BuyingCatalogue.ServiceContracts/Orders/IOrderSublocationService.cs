using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderSublocationService
    {
        Task<OrderSublocation> GetOrderSublocationWithRecipients(
            string externalOrgId,
            CallOffId callOffId,
            string sublocationOdsCode);

        Task<int> GetCountForOrderSublocationRecipients(
            string externalOrgId,
            CallOffId callOffId,
            string sublocationOdsCode);

        Task SetSublocationRecipients(
            string parentOdsCode,
            CallOffId callOffId,
            string sublocationOdsCode,
            HashSet<string> newRecipientOdsCodes);
    }
}
