using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderPriceService
    {
        Task AddPrice(int orderId, CataloguePrice price, List<OrderPricingTierDto> agreedPrices);

        Task UpdatePrice(int orderId, CatalogueItemId catalogueItemId, List<OrderPricingTierDto> agreedPrices);

        Task SetOrderItemQuantity(int orderId, CatalogueItemId catalogueItemId, int quantity);

        Task SetServiceRecipientQuantities(int orderId, CatalogueItemId catalogueItemId, List<OrderPricingTierQuantityDto> quantities);
    }
}
