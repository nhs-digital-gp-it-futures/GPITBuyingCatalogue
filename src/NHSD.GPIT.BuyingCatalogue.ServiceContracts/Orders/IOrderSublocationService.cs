using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderSublocationService
    {
        Task<OrderSublocation> GetOrderSublocationWithRecipients(
            string externalOrgId,
            int orderId,
            string sublocationOdsCode);

        Task<int> GetCountForOrderSublocationRecipients(
            string externalOrgId,
            int orderId,
            string sublocationOdsCode);

        Task SetSublocationRecipients(
            string parentOdsCode,
            int orderId,
            string sublocationOdsCode,
            HashSet<string> newRecipientOdsCodes);
    }
}
