using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IImplementationPlanService
    {
        Task<ImplementationPlan> GetDefaultImplementationPlan();

        Task AddBespokeMilestone(int orderId, int contractId, string name, string paymentTrigger);

        Task<ImplementationPlanMilestone> GetBespokeMilestone(int orderId, int milestoneId);

        Task EditBespokeMilestone(int orderId, int milestoneId, string name, string paymentTrigger);

        Task DeleteBespokeMilestone(int orderId, int milestoneId);
    }
}
