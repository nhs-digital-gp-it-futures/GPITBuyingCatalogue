using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface ITaskProgressProvider
    {
        TaskProgress Get(OrderWrapper wrapper, OrderProgress state);
    }
}
