using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IContractsService
    {
        Task<Contract> GetContract(int orderId);

        Task<Contract> AddContract(int orderId);

        Task<ContractFlags> GetContractFlags(int orderId);

        Task RemoveContractFlags(int orderId);

        Task RemoveBillingAndRequirements(int orderId);

        Task HasSpecificRequirements(int orderId, bool value);

        Task UseDefaultBilling(int orderId, bool value);

        Task UseDefaultDataProcessing(int orderId, bool value);
    }
}
