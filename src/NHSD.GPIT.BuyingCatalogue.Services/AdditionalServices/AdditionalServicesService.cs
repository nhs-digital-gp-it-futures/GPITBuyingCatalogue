using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditonalServices;

namespace NHSD.GPIT.BuyingCatalogue.Services.AdditionalServices
{
    public class AdditionalServicesService : IAdditionalServicesService
    {
        private readonly GPITBuyingCatalogueDbContext dbContext;

        public AdditionalServicesService(GPITBuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<CatalogueItem>> GetAdditionalServicesBySolutionIds(IEnumerable<string> solutionIds)
        {
            return await dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Include(x => x.AdditionalService)
                .Where(
                    x => solutionIds.Contains(x.AdditionalService.SolutionId)
                        && x.CatalogueItemType.Name == "Additional Service"
                        && x.PublishedStatus.Name == "Published")
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<AdditionalService> GetAdditionalService(string catalogueItemId)
        {
            return await dbContext.AdditionalServices.SingleAsync(x => x.CatalogueItemId == catalogueItemId);
        }
    }
}
