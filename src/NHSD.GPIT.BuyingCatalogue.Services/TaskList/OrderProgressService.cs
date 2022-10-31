using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList
{
    public class OrderProgressService : IOrderProgressService
    {
        public TaskProgress DescriptionStatus(OrderWrapper wrapper) => new DescriptionStatusProvider().Process(wrapper);
    }
}
