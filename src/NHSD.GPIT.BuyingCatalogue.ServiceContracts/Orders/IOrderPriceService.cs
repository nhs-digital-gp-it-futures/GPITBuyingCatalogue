using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderPriceService
    {
        Task UpdatePrice(int orderId, int orderItemId, List<PricingTierDto> agreedPrices);

        Task UpsertPrice(int orderId, int orderItemId, CataloguePrice price, List<PricingTierDto> agreedPrices);
    }
}
