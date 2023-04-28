using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.Services.Framework
{
    public class FrameworkService : IFrameworkService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public FrameworkService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<FrameworkFilterInfo>> GetFrameworksByCatalogueItems(IList<CatalogueItem> catalogueItems)
        {
            var catalogueItemIds = catalogueItems.Select(ci => ci.Id).ToList();

            return dbContext.Frameworks
                        .Where(f => dbContext.FrameworkSolutions.Any(fs => fs.FrameworkId == f.Id || catalogueItemIds.Contains(fs.SolutionId)))
                        .Select(g => new FrameworkFilterInfo
                        {
                            Id = g.Id,
                            ShortName = g.ShortName,
                            Name = g.Name,
                            CountOfActiveSolutions = dbContext.FrameworkSolutions.Count(fs => fs.FrameworkId == g.Id && catalogueItemIds.Contains(fs.SolutionId)),
                        })
                        .ToListAsync();
        }

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFrameworksById(string frameworkId) =>
            await dbContext.Frameworks.FirstOrDefaultAsync(f => f.Id == frameworkId);
    }
}
