using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface IOrderProgressProvider
    {
        TaskProgress Process(OrderWrapper wrapper);
    }
}
