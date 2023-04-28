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

        public async Task<List<FrameworkFilterInfo>> GetFrameworksByCatalogueItems(IList<CatalogueItem> catalogueItems)
        {
            var frameworks = await dbContext.FrameworkSolutions
                .Include(fs => fs.Solution)
                .ThenInclude(s => s.CatalogueItem)
                .Where(
                    fs => fs.Solution.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                        && fs.Solution.CatalogueItem.PublishedStatus == PublicationStatus.Published)
                .Select(f => new FrameworkFilterInfo
                {
                    Id = f.FrameworkId,
                    ShortName = f.Framework.ShortName,
                    Name = f.Framework.Name,
                })
                .Distinct()
                .OrderBy(f => f.Name)
                .ToListAsync();
            foreach (var framework in frameworks)
            {
                framework.CountOfActiveSolutions = catalogueItems.Count(c => c.Solution.FrameworkSolutions.Any(x => x.FrameworkId == framework.Id));
            }

            return frameworks;
        }

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFrameworksById(string frameworkId) =>
            await dbContext.Frameworks.FirstOrDefaultAsync(f => f.Id == frameworkId);
    }
}
