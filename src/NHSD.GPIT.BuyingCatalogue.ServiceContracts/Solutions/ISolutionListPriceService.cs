using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionListPriceService
    {
        Task<CatalogueItem> GetSolutionWithListPrices(CatalogueItemId solutionId);

        Task AddListPrice(CatalogueItemId solutionId, CataloguePrice cataloguePrice);

        Task UpdateListPrice(CatalogueItemId solutionId, int cataloguePriceId, CataloguePrice cataloguePrice);

        Task AddListPriceTier(CatalogueItemId solutionId, int cataloguePriceId, CataloguePriceTier tier);

        Task SetPublicationStatus(CatalogueItemId solutionId, int cataloguePriceId, PublicationStatus status);
    }
}
