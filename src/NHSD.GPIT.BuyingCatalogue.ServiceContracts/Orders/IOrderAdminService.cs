using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageOrders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderAdminService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task<PagedList<AdminManageOrder>> GetPagedOrders(PageOptions options, string search = null);

        Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(string search);
    }
}
