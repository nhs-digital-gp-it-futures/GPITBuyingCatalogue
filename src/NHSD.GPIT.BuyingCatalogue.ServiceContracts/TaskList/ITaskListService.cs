using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface ITaskListService
    {
        public Task<OrderTaskList> GetTaskListStatusModelForOrder(int? orderId);

        public Task<OrderTaskListCompletedSections> GetOrderSectionFlags(int orderId);
    }
}
