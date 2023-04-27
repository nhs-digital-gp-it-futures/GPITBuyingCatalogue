using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Frameworks;

namespace NHSD.GPIT.BuyingCatalogue.Services.Framework
{
    public class FrameworkService : IFrameworkService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public FrameworkService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<EntityFramework.Catalogue.Models.Framework>> GetFrameworksWithActiveAndPublishedSolutions() => dbContext.Frameworks
            .Join(dbContext.FrameworkSolutions, f => f.Id, fs => fs.FrameworkId, (f, fs) => new { Framework = f, FrameworkSolution = fs })
            .Join(dbContext.Solutions, fs => fs.FrameworkSolution.SolutionId, s => s.CatalogueItemId, (fs, s) => new { fs.Framework, Solution = s })
            .Join(dbContext.CatalogueItems, x => x.Solution.CatalogueItemId, ci => ci.Id, (x, ci) => new { x.Framework, x.Solution, CatalogueItem = ci })
            .Where(x => x.CatalogueItem.PublishedStatus == PublicationStatus.Published)
            .Select(x => x.Framework)
            .Distinct()
            .OrderBy(f => f.Id)
            .ThenBy(f => f.Name)
            .AsNoTracking()
            .ToListAsync();

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFrameworksById(string frameworkId) =>
            await dbContext.Frameworks.FirstOrDefaultAsync(f => f.Id == frameworkId);
    }
}
