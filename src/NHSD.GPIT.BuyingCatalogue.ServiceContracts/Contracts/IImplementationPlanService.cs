using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IImplementationPlanService
    {
        Task<ImplementationPlan> GetDefaultImplementationPlan();

        Task<int> AddBespokeMilestone(int orderId, int contractId, string name, string paymentTrigger);
    }
}
