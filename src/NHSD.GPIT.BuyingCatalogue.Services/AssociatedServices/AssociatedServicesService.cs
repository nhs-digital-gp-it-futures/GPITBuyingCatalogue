using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.Services.AssociatedServices
{
    public class AssociatedServicesService : IAssociatedServicesService
    {
        private readonly GPITBuyingCatalogueDbContext dbContext;

        public AssociatedServicesService(GPITBuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<CatalogueItem>> GetAssociatedServicesForSupplier(string supplierId)
        {
            return await dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Include(x => x.AssociatedService)
                .Where(
                    x => x.SupplierId == supplierId
                        && x.CatalogueItemType == CatalogueItemType.AssociatedService
                        && x.PublishedStatus == PublicationStatus.Published)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
