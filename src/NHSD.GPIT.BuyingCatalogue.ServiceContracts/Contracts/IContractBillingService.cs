using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IContractBillingService
    {
        Task AddContractBilling(int orderId, int contractId);

        Task AddBespokeContractBillingItem(
            int orderId,
            int contractId,
            CatalogueItemId catalogueItemId,
            string name,
            string paymentTrigger,
            int quantity);

        Task<ContractBillingItem> GetContractBillingItem(int orderId, int itemId);

        Task EditContractBillingItem(
            int orderId,
            int itemId,
            CatalogueItemId catalogueItemId,
            string name,
            string paymentTrigger,
            int quantity);

        Task DeleteContractBillingItem(int orderId, int itemId);
    }
}
