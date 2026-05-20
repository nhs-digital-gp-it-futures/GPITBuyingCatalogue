using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemService
    {
        Task AddOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds, int? parentId = null);

        Task DeleteOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<int> itemIds);

        public Task<OrderItem> GetOrderItem(CallOffId callOffId, string internalOrgId, int orderItemId);

        public Task<OrderItem> GetOrderItem(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId);

        public Task UpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, OrderItemFundingType selectedFundingType);

        public Task SetOrderItemEstimationPeriod(int orderItemId, CataloguePrice price);

        public Task DetectChangesInFundingAndDelete(CallOffId callOffId, string internalOrgId, int orderItemId);
    }
}
