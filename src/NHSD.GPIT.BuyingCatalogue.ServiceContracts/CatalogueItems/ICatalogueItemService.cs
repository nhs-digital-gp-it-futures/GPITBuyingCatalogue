using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.CatalogueItems;

public interface ICatalogueItemService
{
    Task<string> GetCatalogueItemName(CatalogueItemId catalogueItemId);

    Task<CatalogueItem> GetCatalogueItem(CatalogueItemId catalogueItemId);
}
