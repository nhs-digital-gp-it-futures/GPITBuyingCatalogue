using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemService
    {
        Task Create(CallOffId callOffId, string internalOrgId, CreateOrderItemModel model);

        Task<List<OrderItem>> GetOrderItems(CallOffId callOffId, string internalOrgId, CatalogueItemType? catalogueItemType);

        Task<OrderItem> GetOrderItem(CallOffId callOffId, string internalOrgId, CatalogueItemId catalogueItemId);

        Task DeleteOrderItem(CallOffId callOffId,  string internalOrgId, CatalogueItemId catalogueItemId);
    }
}
