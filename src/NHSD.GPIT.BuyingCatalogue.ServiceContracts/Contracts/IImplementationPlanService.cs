using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts
{
    public interface IImplementationPlanService
    {
        Task<ImplementationPlan> GetDefaultImplementationPlan();

        Task AddImplementationPlan(int orderId, int contractId);

        Task AddBespokeMilestone(int orderId, int contractId, string name, string paymentTrigger);

        Task<ImplementationPlanMilestone> GetMilestone(int orderId, int milestoneId);

        Task EditMilestone(int orderId, int milestoneId, string name, string paymentTrigger);

        Task DeleteMilestone(int orderId, int milestoneId);
    }
}
