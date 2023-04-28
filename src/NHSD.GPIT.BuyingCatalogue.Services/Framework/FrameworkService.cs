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
            return dbContext.FrameworkSolutions
                .Where(fs => catalogueItemIds.Contains(fs.SolutionId))
                .GroupBy(fs => new { fs.FrameworkId, fs.Framework.ShortName })
                .Select(g => new FrameworkFilterInfo
                {
                    Id = g.Key.FrameworkId,
                    ShortName = g.Key.ShortName,
                    Name = g.First().Framework.Name,
                    CountOfActiveSolutions = g.Count(),
                })
                .ToListAsync();
        }

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFrameworksById(string frameworkId) =>
            await dbContext.Frameworks.FirstOrDefaultAsync(f => f.Id == frameworkId);
    }
}
