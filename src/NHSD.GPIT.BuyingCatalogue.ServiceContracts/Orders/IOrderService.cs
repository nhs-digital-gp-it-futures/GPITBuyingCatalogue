using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderService
    {
        public Task<Order> GetOrderThin(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderWithCatalogueItemAndPrices(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderWithOrderItems(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderWithOrderItemsForFunding(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderWithSupplier(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderForSummary(CallOffId callOffId, string internalOrgId);

        public Task<PagedList<Order>> GetPagedOrders(int organisationId, PageOptions options, string search = null);

        public Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(int organisationId, string searchTerm);

        public Task<Order> GetOrderSummary(CallOffId callOffId, string internalOrgId);

        public Task<Order> GetOrderForStatusUpdate(CallOffId callOffId, string internalOrgId);

        public Task<Order> CreateOrder(string description, string internalOrgId, OrderTriageValue? orderTriageValue, bool isAssociatedServiceOnly);

        public Task DeleteOrder(CallOffId callOffId, string internalOrgId);

        public Task CompleteOrder(CallOffId callOffId, string internalOrgId, int userId, Uri orderSummaryUri);

        public Task<List<Order>> GetUserOrders(int userId);

        public Task SetSolutionId(string internalOrgId, CallOffId callOffId, CatalogueItemId solutionId);
    }
}
