using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders
{
    public interface IOrderPriceService
    {
        Task UpdatePrice(int orderId, CatalogueItemId catalogueItemId, List<PricingTierDto> agreedPrices);

        Task UpsertPrice(int orderId, CataloguePrice price, List<PricingTierDto> agreedPrices);
    }
}
