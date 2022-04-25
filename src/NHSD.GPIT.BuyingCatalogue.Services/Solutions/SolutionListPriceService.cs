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

        public async Task<CatalogueItem> GetSolutionWithPublishedListPrices(CatalogueItemId solutionId)
        {
            var solution = await GetSolutionWithListPrices(solutionId, tracked: false);

            solution.CataloguePrices = solution.CataloguePrices.Where(cp => cp.PublishedStatus == PublicationStatus.Published).ToList();

            return solution;
        }

        public async Task AddListPrice(CatalogueItemId solutionId, CataloguePrice cataloguePrice)
        {
            if (cataloguePrice is null)
                throw new ArgumentNullException(nameof(cataloguePrice));

            var solution = await GetSolutionWithListPrices(solutionId, true);

            solution.CataloguePrices.Add(cataloguePrice);

            await dbContext.SaveChangesAsync();
        }

        public async Task<CataloguePrice> UpdateListPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            PricingUnit pricingUnit,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            TimeUnit? timeUnit)
        {
            if (pricingUnit is null)
                throw new ArgumentNullException(nameof(pricingUnit));

            var solution = await GetSolutionWithListPrices(solutionId, true);
            var price = solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId);

            price.ProvisioningType = provisioningType;
            price.CataloguePriceCalculationType = calculationType;
            price.TimeUnit = timeUnit;
            price.PricingUnit.RangeDescription = pricingUnit.RangeDescription;
            price.PricingUnit.Definition = pricingUnit.Definition;
            price.PricingUnit.Description = pricingUnit.Description;

            await dbContext.SaveChangesAsync();

            return price;
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

        public async Task UpdateListPriceTier(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            int tierId,
            decimal price,
            int lowerRange,
            int? upperRange)
        {
            var cataloguePrice = await dbContext
                .CataloguePrices
                .Include(p => p.CataloguePriceTiers)
                .SingleAsync(cp => cp.CatalogueItemId == solutionId && cp.CataloguePriceId == cataloguePriceId);

            var tier = cataloguePrice.CataloguePriceTiers.Single(p => p.Id == tierId);

            tier.Price = price;
            tier.LowerRange = lowerRange;
            tier.UpperRange = upperRange;

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateTierPrice(CatalogueItemId solutionId, int cataloguePriceId, int tierId, decimal price)
        {
            var cataloguePrice = await dbContext
                   .CataloguePrices
                   .Include(p => p.CataloguePriceTiers)
                   .SingleAsync(cp => cp.CatalogueItemId == solutionId && cp.CataloguePriceId == cataloguePriceId);

            var tier = cataloguePrice.CataloguePriceTiers.Single(p => p.Id == tierId);

            tier.Price = price;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteListPrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await GetSolutionWithListPrices(solutionId, true);
            var price = solution.CataloguePrices.SingleOrDefault(p => p.CataloguePriceId == cataloguePriceId);

            if (price is null)
                return;

            dbContext.Remove(price);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeletePriceTier(CatalogueItemId solutionId, int cataloguePriceId, int tierId)
        {
            var cataloguePrice = await dbContext
                   .CataloguePrices
                   .Include(p => p.CataloguePriceTiers)
                   .SingleAsync(cp => cp.CatalogueItemId == solutionId && cp.CataloguePriceId == cataloguePriceId);

            var tier = cataloguePrice.CataloguePriceTiers.SingleOrDefault(p => p.Id == tierId);

            if (tier is null)
                return;

            dbContext.Remove(tier);
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
