using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList
{
    public interface IOrderProgressService
    {
        TaskProgress DescriptionStatus(OrderWrapper wrapper);
    }
}
