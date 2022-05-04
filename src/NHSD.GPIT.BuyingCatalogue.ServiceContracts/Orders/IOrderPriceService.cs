using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderPriceService
    {
        Task AddPrice(int orderId, CataloguePrice price, List<OrderPricingTierDto> agreedPrices);
    }
}
