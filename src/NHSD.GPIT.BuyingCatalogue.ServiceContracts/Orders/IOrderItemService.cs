using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemService
    {
        Task Create(CallOffId callOffId, CreateOrderItemModel model);

        Task<List<OrderItem>> GetOrderItems(CallOffId callOffId, CatalogueItemType? catalogueItemType);

        Task<OrderItem> GetOrderItem(CallOffId callOffId, CatalogueItemId catalogueItemId);

        Task DeleteOrderItem(CallOffId callOffId, CatalogueItemId catalogueItemId);
    }
}
