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
    public class ListPriceService : IListPriceService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ListPriceService(BuyingCatalogueDbContext dbContext)
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
        {
            var results = dbContext
                .CataloguePrices
                .Where(p => p.CataloguePriceId != cataloguePriceId && p.CatalogueItemId == catalogueItemId);

            return await results.AnyAsync(p => p.ProvisioningType == provisioningType
                && p.CataloguePriceCalculationType == calculationType
                && string.Equals(p.PricingUnit.Description, unitDescription)
                && string.Equals(p.PricingUnit.RangeDescription, rangeDefinition));
        }

        public async Task<bool> HasDuplicatePriceTier(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            int? tierId,
            decimal price,
            int lowerRange,
            int? upperRange)
            => await dbContext
            .CataloguePriceTiers
            .Where(p => p.CataloguePriceId == cataloguePriceId && p.CataloguePrice.CatalogueItemId == catalogueItemId && p.Id != tierId)
            .AnyAsync(p => p.Price == price
                && p.LowerRange == lowerRange
                && p.UpperRange == upperRange);

        public async Task<int> GetNumberOfListPrices(CatalogueItemId catalogueItemId, int cataloguePriceId)
            => await dbContext
            .CataloguePrices
            .Where(p => p.CatalogueItemId == catalogueItemId && p.CataloguePriceId != cataloguePriceId)
            .CountAsync();
    }
}
