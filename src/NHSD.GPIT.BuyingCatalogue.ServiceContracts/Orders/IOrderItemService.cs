using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemService
    {
        Task AddOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds);

        Task DeleteOrderItems(string internalOrgId, CallOffId callOffId, IEnumerable<CatalogueItemId> itemIds);

        public Task<OrderItem> GetOrderItem(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId);

        public Task<OrderItem> SaveOrUpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId, OrderItemFundingType selectedFundingType, decimal? centrallyAllocatedAmount);
    }
}
