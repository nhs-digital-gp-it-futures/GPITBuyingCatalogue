using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemService
    {
        // TODO: callOffId should be of type CallOffId
        Task<AggregateValidationResult> Create(string callOffId, CreateOrderItemModel model);

        // TODO: callOffId should be of type CallOffId
        Task<List<OrderItem>> GetOrderItems(string callOffId, CatalogueItemType? catalogueItemType);

        // TODO: callOffId should be of type CallOffId
        // TODO: catalogueItemId should be of type CatalogueItemId
        Task<OrderItem> GetOrderItem(string callOffId, string catalogueItemId);

        // TODO: callOffId should be of type CallOffId
        // TODO: catalogueItemId should be of type CatalogueItemId
        Task<int> DeleteOrderItem(string callOffId, string catalogueItemId);
    }
}
