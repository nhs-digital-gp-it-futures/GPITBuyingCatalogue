using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface ITaskListService
    {
        public OrderTaskList GetTaskListStatusModelForOrder(Order order);
    }
}
