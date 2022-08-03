using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IContractsService
    {
        Task<ContractFlags> GetContract(int orderId);

        Task HasSpecificRequirements(int orderId, bool value);

        Task UseDefaultBilling(int orderId, bool value);

        Task UseDefaultDataProcessing(int orderId, bool value);

        Task UseDefaultImplementationPlan(int orderId, bool value);
    }
}
