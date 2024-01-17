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
        public Task<int> GetOrderId(string internalOrgId, CallOffId callOffId);

        public Task<int> GetOrderId(CallOffId callOffId);

        public Task<bool> HasSubsequentRevisions(CallOffId callOffId);

        public Task<OrderWrapper> GetOrderThin(CallOffId callOffId, string internalOrgId);

        public Task<OrderWrapper> GetOrderWithCatalogueItemAndPrices(CallOffId callOffId, string internalOrgId);

        public Task<OrderWrapper> GetOrderWithOrderItems(CallOffId callOffId, string internalOrgId);

        public Task<OrderWrapper> GetOrderWithOrderItemsForFunding(CallOffId callOffId, string internalOrgId);

        public Task<OrderWrapper> GetOrderWithSupplier(CallOffId callOffId, string internalOrgId);

        public Task<OrderWrapper> GetOrderForSummary(CallOffId callOffId, string internalOrgId);

        public Task<OrderWrapper> GetOrderForTaskListStatuses(CallOffId callOffId, string internalOrgId);

        public Task<(PagedList<Order> Orders, IEnumerable<CallOffId> OrderIds)> GetPagedOrders(int organisationId, PageOptions options, string search = null);

        public Task<IList<SearchFilterModel>> GetOrdersBySearchTerm(int organisationId, string searchTerm);

        public Task<Order> CreateOrder(string description, string internalOrgId, OrderTriageValue? orderTriageValue, OrderTypeEnum orderType);

        public Task<Order> AmendOrder(string internalOrgId, CallOffId callOffId);

        public Task EnsureOrderItemsForAmendment(string internalOrgId, CallOffId callOffId);

        public Task SoftDeleteOrder(CallOffId callOffId, string internalOrgId);

        public Task HardDeleteOrder(CallOffId callOffId, string internalOrgId);

        public Task TerminateOrder(CallOffId callOffId, string internalOrgId, int userId, DateTime terminationDate, string reason);

        public Task CompleteOrder(CallOffId callOffId, string internalOrgId, int userId);

        public Task<List<Order>> GetUserOrders(int userId);

        public Task SetSolutionId(string internalOrgId, CallOffId callOffId, CatalogueItemId solutionId);

        public Task SetOrderPracticeReorganisationRecipient(string internalOrgId, CallOffId callOffId, string odsCode);

        public Task SetFundingSourceForForceFundedItems(string internalOrgId, CallOffId callOffId);

        public Task DeleteSelectedFramework(string internalOrgId, CallOffId callOffId);
    }
}
