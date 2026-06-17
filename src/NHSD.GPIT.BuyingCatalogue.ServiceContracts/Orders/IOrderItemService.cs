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

        public Task UpdateOrderItemFunding(CallOffId callOffId, string internalOrgId, int orderItemId, OrderItemFundingType selectedFundingType);

        public Task SetOrderItemEstimationPeriod(CallOffId callOffId, string internalOrgId, int orderItemId, CataloguePrice price);

        public Task DetectChangesInFundingAndDelete(CallOffId callOffId, string internalOrgId, int orderItemId);
    }
}
