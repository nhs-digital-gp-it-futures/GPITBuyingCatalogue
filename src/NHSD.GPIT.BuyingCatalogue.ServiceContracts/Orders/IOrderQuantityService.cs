using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderQuantityService
    {
        Task SetOrderItemQuantity(int orderId, CatalogueItemId catalogueItemId, int quantity);

        Task SetServiceRecipientQuantities(int orderId, CatalogueItemId catalogueItemId, List<OrderItemRecipientQuantityDto> quantities);
    }
}
