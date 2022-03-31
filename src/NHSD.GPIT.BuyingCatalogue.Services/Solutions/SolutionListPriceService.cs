using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public class SolutionListPriceService : ISolutionListPriceService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public SolutionListPriceService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<CatalogueItem> GetSolutionWithListPrices(CatalogueItemId solutionId)
            => await GetSolutionWithListPrices(solutionId, tracked: false);

        public async Task AddListPrice(CatalogueItemId solutionId, CataloguePrice cataloguePrice)
        {
            if (cataloguePrice is null)
                throw new ArgumentNullException(nameof(cataloguePrice));

            var solution = await GetSolutionWithListPrices(solutionId, true);

            solution.CataloguePrices.Add(cataloguePrice);

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateListPrice(CatalogueItemId solutionId, int cataloguePriceId, CataloguePrice cataloguePrice)
        {
            if (cataloguePrice is null)
                throw new ArgumentNullException(nameof(cataloguePrice));

            var solution = await GetSolutionWithListPrices(solutionId, true);
            var price = solution.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);

            price.ProvisioningType = cataloguePrice.ProvisioningType;
            price.CataloguePriceCalculationType = cataloguePrice.CataloguePriceCalculationType;
            price.TimeUnit = cataloguePrice.TimeUnit;
            price.PricingUnit.RangeDescription = cataloguePrice.PricingUnit.RangeDescription;
            price.PricingUnit.Definition = cataloguePrice.PricingUnit.Definition;
            price.PricingUnit.Description = cataloguePrice.PricingUnit.Description;

            await dbContext.SaveChangesAsync();
        }

        public async Task SetPublicationStatus(CatalogueItemId solutionId, int cataloguePriceId, PublicationStatus status)
        {
            var cataloguePrice = await dbContext
                .CataloguePrices
                .SingleAsync(cp => cp.CatalogueItemId == solutionId && cp.CataloguePriceId == cataloguePriceId);

            cataloguePrice.PublishedStatus = status;

            await dbContext.SaveChangesAsync();
        }

        public async Task AddListPriceTier(CatalogueItemId solutionId, int cataloguePriceId, CataloguePriceTier tier)
        {
            if (tier is null)
                throw new ArgumentNullException(nameof(tier));

            var cataloguePrice = await dbContext
                .CataloguePrices
                .Include(p => p.CataloguePriceTiers)
                .SingleAsync(cp => cp.CatalogueItemId == solutionId && cp.CataloguePriceId == cataloguePriceId);

            cataloguePrice.CataloguePriceTiers.Add(tier);

            await dbContext.SaveChangesAsync();
        }

        private Task<CatalogueItem> GetSolutionWithListPrices(CatalogueItemId solutionId, bool tracked)
        {
            var baseQuery = dbContext
                .CatalogueItems
                .Include(ci => ci.CataloguePrices)
                .ThenInclude(cp => cp.PricingUnit)
                .Include(ci => ci.CataloguePrices)
                .ThenInclude(cp => cp.CataloguePriceTiers)
                .AsQueryable();

            if (!tracked)
                baseQuery = baseQuery.AsNoTracking();

            return baseQuery.SingleOrDefaultAsync(ci => ci.Id == solutionId);
        }
    }
}
