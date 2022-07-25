using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IContractsService
    {
        Task<Contract> GetContract(int orderId);

        Task SetImplementationPlanId(int orderId, int? implementationPlanId);
    }
}
