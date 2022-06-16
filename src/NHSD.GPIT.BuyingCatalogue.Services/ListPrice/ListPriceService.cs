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

        public async Task<bool> HasDuplicateFlatPrice(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            ProvisioningType provisioningType,
            decimal price,
            string unitDescription)
        {
            var results = dbContext
                .CataloguePrices
                .Where(p => p.CataloguePriceId != cataloguePriceId
                    && p.CatalogueItemId == catalogueItemId
                    && p.CataloguePriceType == CataloguePriceType.Flat);

            return await results.AnyAsync(
                p => p.ProvisioningType == provisioningType
                    && p.CataloguePriceTiers.Any(pt => pt.Price == price)
                    && p.PricingUnit.Description == unitDescription);
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
                .Where(p => p.CataloguePriceId != cataloguePriceId
                    && p.CatalogueItemId == catalogueItemId
                    && p.CataloguePriceType == CataloguePriceType.Tiered);

            return await results.AnyAsync(p => p.ProvisioningType == provisioningType
                && p.CataloguePriceCalculationType == calculationType
                && p.PricingUnit.Description == unitDescription
                && p.PricingUnit.RangeDescription == rangeDefinition);
        }

        public async Task<bool> HasDuplicatePriceTier(
            CatalogueItemId catalogueItemId,
            int? cataloguePriceId,
            int? tierId,
            int lowerRange,
            int? upperRange)
            => await dbContext
            .CataloguePriceTiers
            .Where(p => p.CataloguePriceId == cataloguePriceId && p.CataloguePrice.CatalogueItemId == catalogueItemId && p.Id != tierId)
            .AnyAsync(p => p.LowerRange == lowerRange
                && p.UpperRange == upperRange);

        public async Task<int> GetNumberOfListPrices(CatalogueItemId catalogueItemId, int cataloguePriceId)
            => await dbContext
            .CataloguePrices
            .Where(p => p.CatalogueItemId == catalogueItemId && p.CataloguePriceId != cataloguePriceId)
            .CountAsync();

        public async Task<CatalogueItem> GetCatalogueItemWithListPrices(CatalogueItemId catalogueItemId)
            => await GetCatalogueItemWithListPrices(catalogueItemId, tracked: false);

        public async Task<CatalogueItem> GetCatalogueItemWithPublishedListPrices(CatalogueItemId catalogueItemId)
        {
            var item = await GetCatalogueItemWithListPrices(catalogueItemId, tracked: false);

            item.CataloguePrices = item.CataloguePrices
                .Where(cp => cp.PublishedStatus == PublicationStatus.Published)
                .ToList();

            return item;
        }

        public async Task AddListPrice(CatalogueItemId solutionId, CataloguePrice cataloguePrice)
        {
            if (cataloguePrice is null)
                throw new ArgumentNullException(nameof(cataloguePrice));

            var solution = await GetCatalogueItemWithListPrices(solutionId, true);

            solution.CataloguePrices.Add(cataloguePrice);

            await dbContext.SaveChangesAsync();
        }

        public async Task<CataloguePrice> UpdateListPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            PricingUnit pricingUnit,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            TimeUnit? timeUnit,
            CataloguePriceQuantityCalculationType? quantityCalculationType)
        {
            if (pricingUnit is null)
                throw new ArgumentNullException(nameof(pricingUnit));

            var solution = await GetCatalogueItemWithListPrices(solutionId, true);
            var price = solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId);

            price.ProvisioningType = provisioningType;
            price.CataloguePriceCalculationType = calculationType;
            price.TimeUnit = timeUnit;
            price.CataloguePriceQuantityCalculationType = quantityCalculationType;
            price.PricingUnit.RangeDescription = pricingUnit.RangeDescription;
            price.PricingUnit.Definition = pricingUnit.Definition;
            price.PricingUnit.Description = pricingUnit.Description;

            await dbContext.SaveChangesAsync();

            return price;
        }

        public async Task UpdateListPrice(
            CatalogueItemId solutionId,
            int cataloguePriceId,
            PricingUnit pricingUnit,
            ProvisioningType provisioningType,
            CataloguePriceCalculationType calculationType,
            TimeUnit? timeUnit,
            CataloguePriceQuantityCalculationType? quantityCalculationType,
            decimal price)
        {
            if (pricingUnit is null)
                throw new ArgumentNullException(nameof(pricingUnit));

            var solution = await GetCatalogueItemWithListPrices(solutionId, true);
            var cataloguePrice = solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId);

            cataloguePrice.ProvisioningType = provisioningType;
            cataloguePrice.CataloguePriceCalculationType = calculationType;
            cataloguePrice.TimeUnit = timeUnit;
            cataloguePrice.CataloguePriceQuantityCalculationType = quantityCalculationType;
            cataloguePrice.PricingUnit.RangeDescription = pricingUnit.RangeDescription;
            cataloguePrice.PricingUnit.Definition = pricingUnit.Definition;
            cataloguePrice.PricingUnit.Description = pricingUnit.Description;

            var tier = cataloguePrice.CataloguePriceTiers.First();
            tier.Price = price;

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
            var solution = await GetCatalogueItemWithListPrices(solutionId, true);
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

        private Task<CatalogueItem> GetCatalogueItemWithListPrices(CatalogueItemId catalogueItemId, bool tracked)
        {
            var baseQuery = dbContext.CatalogueItems
                .Include(ci => ci.CataloguePrices)
                .ThenInclude(cp => cp.PricingUnit)
                .Include(ci => ci.CataloguePrices)
                .ThenInclude(cp => cp.CataloguePriceTiers)
                .AsQueryable();

            if (!tracked)
                baseQuery = baseQuery.AsNoTracking();

            return baseQuery.SingleOrDefaultAsync(ci => ci.Id == catalogueItemId);
        }
    }
}
