using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices
{
    public interface IListPricesService
    {
        Task<int> SaveListPrice(CatalogueItemId itemId, SaveListPriceModel model);

        Task UpdateListPrice(CatalogueItemId itemId, SaveListPriceModel model);

        Task UpdateListPriceStatus(CatalogueItemId itemId, SaveListPriceModel model);

        Task DeleteListPrice(CatalogueItemId itemId, int cataloguePriceId);

        Task<CatalogueItem> GetCatalogueItemWithPrices(CatalogueItemId itemId);
    }
}
