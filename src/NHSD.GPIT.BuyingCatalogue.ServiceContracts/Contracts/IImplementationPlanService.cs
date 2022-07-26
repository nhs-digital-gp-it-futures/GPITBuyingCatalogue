using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IImplementationPlanService
    {
        Task<ImplementationPlan> CreateImplementationPlan();

        Task<ImplementationPlan> GetDefaultImplementationPlan();
    }
}
