using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderItemService
    {
        Task<AggregateValidationResult> Create(string callOffId, CreateOrderItemModel model);

        Task<List<OrderItem>> GetOrderItems(string callOffId, CatalogueItemType catalogueItemType);

        Task<OrderItem> GetOrderItem(string callOffId, string catalogueItemId);

        Task<int> DeleteOrderItem(string callOffId, string catalogueItemId);
    }
}
