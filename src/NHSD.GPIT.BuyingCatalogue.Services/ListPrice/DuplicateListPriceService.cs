using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;

namespace NHSD.GPIT.BuyingCatalogue.Services.ListPrice
{
    public class DuplicateListPriceService : IDuplicateListPriceService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public DuplicateListPriceService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<bool> HasDuplicateTieredPrice(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            string unitDescription,
            string rangeDefinition)
            => await dbContext
            .CataloguePrices
            .Where(p => p.CataloguePriceId != cataloguePriceId)
            .AnyAsync(p => p.CatalogueItemId == catalogueItemId
                && p.ProvisioningType == provisioningType
                && p.CataloguePriceCalculationType == calculationType
                && string.Equals(p.PricingUnit.Description, unitDescription)
                && string.Equals(p.PricingUnit.RangeDescription, rangeDefinition));

        public async Task<bool> HasDuplicatePriceTier(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            decimal price,
            int lowerRange,
            int? upperRange)
            => await dbContext
            .CataloguePriceTiers
            .Where(p => p.CataloguePriceId == cataloguePriceId)
            .AnyAsync(p => p.CataloguePrice.CatalogueItemId == catalogueItemId
                && p.Price == price
                && p.LowerRange == lowerRange
                && p.UpperRange == upperRange);
    }
}
