using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice
{
    public interface IDuplicateListPriceService
    {
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
            decimal price,
            int lowerRange,
            int? upperRange);
    }
}
