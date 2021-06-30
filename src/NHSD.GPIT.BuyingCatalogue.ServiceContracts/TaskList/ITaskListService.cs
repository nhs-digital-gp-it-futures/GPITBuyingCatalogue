using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface ITaskListService
    {
        public OrderTaskList GetTaskListStatusModelForOrder(Order order);
    }
}
