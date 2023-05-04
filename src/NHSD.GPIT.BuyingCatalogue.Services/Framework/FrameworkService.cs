using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

        public async Task<List<FrameworkFilterInfo>> GetFrameworksByCatalogueItems(
            IList<CatalogueItemId> catalogueItems) =>
            await dbContext.FrameworkSolutions.AsNoTracking()
                .Where(
                    x => x.Solution.CatalogueItem.PublishedStatus == PublicationStatus.Published)
                .GroupBy(x => new { x.FrameworkId, x.Framework.ShortName })
                .Select(
                    x => new FrameworkFilterInfo
                    {
                        Id = x.Key.FrameworkId,
                        ShortName = x.Key.ShortName,
                        CountOfActiveSolutions = x.Count(y => catalogueItems.Contains(y.SolutionId)),
                    })
                .ToListAsync();

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFrameworksById(string frameworkId) =>
            await dbContext.Frameworks.FirstOrDefaultAsync(f => f.Id == frameworkId);
    }
}
