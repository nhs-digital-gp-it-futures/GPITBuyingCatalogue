using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CatalogueItems;

namespace NHSD.GPIT.BuyingCatalogue.Services.CatalogueItems;

public class CatalogueItemService : ICatalogueItemService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public CatalogueItemService(BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<string> GetCatalogueItemName(CatalogueItemId catalogueItemId)
        => (await dbContext.CatalogueItems.FirstOrDefaultAsync(x => x.Id == catalogueItemId))?.Name;

    public async Task<CatalogueItem> GetCatalogueItem(CatalogueItemId catalogueItemId)
        => await dbContext.CatalogueItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == catalogueItemId);
}
