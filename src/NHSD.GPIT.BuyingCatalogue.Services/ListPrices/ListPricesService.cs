using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.ListPrices
{
    public sealed class ListPricesService : IListPricesService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ListPricesService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> SaveListPrice(CatalogueItemId itemId, SaveListPriceModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var catalogueItem = await GetCatalogueItem(itemId);

            var cataloguePrice = new CataloguePrice
            {
                CataloguePriceType = CataloguePriceType.Flat,
                Price = model.Price,
                PricingUnit = model.PricingUnit,
                ProvisioningType = model.ProvisioningType,
                TimeUnit = model.TimeUnit,
                CurrencyCode = "GBP",
                IsLocked = false,
                PublishedStatus = PublicationStatus.Draft,
            };

            catalogueItem.CataloguePrices.Add(cataloguePrice);
            await dbContext.SaveChangesAsync();

            return cataloguePrice.CataloguePriceId;
        }

        public async Task UpdateListPrice(CatalogueItemId itemId, SaveListPriceModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var listPrice = await dbContext
                .CataloguePrices
                .Include(p => p.PricingUnit)
                .SingleAsync(p => p.CataloguePriceId == model.CataloguePriceId && p.CatalogueItemId == itemId);

            if (listPrice.IsLocked)
                throw new InvalidOperationException($"Catalogue price {listPrice.CataloguePriceId} cannot be edited because it is locked");

            listPrice.Price = model.Price;
            listPrice.ProvisioningType = model.ProvisioningType;
            listPrice.TimeUnit = model.TimeUnit;
            listPrice.PricingUnit.TierName = model.PricingUnit.TierName;
            listPrice.PricingUnit.Description = model.PricingUnit.Description;
            listPrice.PricingUnit.Definition = model.PricingUnit.Definition;

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateListPriceStatus(CatalogueItemId itemId, SaveListPriceModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var listPrice = await dbContext
                .CataloguePrices
                .SingleAsync(p => p.CataloguePriceId == model.CataloguePriceId && p.CatalogueItemId == itemId);

            var isLocked =
                (listPrice.PublishedStatus == PublicationStatus.Draft && model.Status == PublicationStatus.Published)
                || listPrice.IsLocked;

            listPrice.PublishedStatus = model.Status;
            listPrice.IsLocked = isLocked;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteListPrice(CatalogueItemId itemId, int cataloguePriceId)
        {
            var cataloguePrice = await dbContext
                .CataloguePrices
                .Include(p => p.PricingUnit)
                .SingleAsync(p => p.CataloguePriceId == cataloguePriceId && p.CatalogueItemId == itemId);

            if (cataloguePrice.IsLocked)
                throw new InvalidOperationException($"Catalogue price {cataloguePriceId} cannot be deleted because it is locked");

            dbContext.CataloguePrices.Remove(cataloguePrice);

            await dbContext.SaveChangesAsync();
        }

        public Task<CatalogueItem> GetCatalogueItemWithPrices(CatalogueItemId itemId)
        {
            return dbContext.CatalogueItems
                .Include(ci => ci.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .FirstOrDefaultAsync(ci => ci.Id == itemId);
        }

        private async Task<CatalogueItem> GetCatalogueItem(CatalogueItemId itemId)
        {
            return await dbContext.CatalogueItems
                .Include(i => i.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Where(i => i.Id == itemId)
                .FirstOrDefaultAsync();
        }
    }
}
