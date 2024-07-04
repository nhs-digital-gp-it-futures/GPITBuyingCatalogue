using System;
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

        Task<PagedList<AdminManageOrder>> GetPagedOrders(PageOptions options, string search = null, string searchTermType = null, string framework = null, OrderStatus? status = null);

        Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(string search);

        Task DeleteOrder(CallOffId callOffId, string nameOfRequester, string nameOfApprover, DateTime? dateOfApproval);
    }
}
