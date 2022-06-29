using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice
{
    public interface IListPriceService
    {
        Task<bool> HasDuplicateFlatPrice(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            decimal price,
            string unitDescription);

        Task<bool> HasDuplicateTieredPrice(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            string unitDescription,
            string rangeDefinition);

        Task<bool> HasDuplicatePriceTier(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            int? tierId,
            int lowerRange,
            int? upperRange);

        Task<int> GetNumberOfListPrices(
            CatalogueItemId catalogueItemId,
            int cataloguePriceId);

        Task<CatalogueItem> GetCatalogueItemWithListPrices(CatalogueItemId catalogueItemId);

        Task<CatalogueItem> GetCatalogueItemWithPublishedListPrices(CatalogueItemId catalogueItemId);

        Task AddListPrice(CatalogueItemId solutionId, CataloguePrice cataloguePrice);

        Task<CataloguePrice> UpdateListPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            PricingUnit pricingUnit,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            TimeUnit? timeUnit,
            CataloguePriceQuantityCalculationType? quantityCalculationType);

        Task UpdateListPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            PricingUnit pricingUnit,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            TimeUnit? timeUnit,
            CataloguePriceQuantityCalculationType? quantityCalculationType,
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
