using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionListPriceService
    {
        Task<CatalogueItem> GetSolutionWithListPrices(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithPublishedListPrices(CatalogueItemId solutionId);

        Task AddListPrice(CatalogueItemId solutionId, CataloguePrice cataloguePrice);

        Task<CataloguePrice> UpdateListPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            PricingUnit pricingUnit,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            TimeUnit? timeUnit);

        Task UpdateListPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            PricingUnit pricingUnit,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            TimeUnit? timeUnit,
            decimal price);

        Task AddListPriceTier(CatalogueItemId solutionId, int cataloguePriceId, CataloguePriceTier tier);

        Task UpdateListPriceTier(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int tierId,
            decimal price,
            int lowerRange,
            int? upperRange);

        Task UpdateTierPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int tierId,
            decimal price);

        Task SetPublicationStatus(CatalogueItemId solutionId, int cataloguePriceId, PublicationStatus status);

        Task DeleteListPrice(CatalogueItemId solutionId, int cataloguePriceId);

        Task DeletePriceTier(CatalogueItemId solutionId, int cataloguePriceId, int tierId);
    }
}
