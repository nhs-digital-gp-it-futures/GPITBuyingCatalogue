using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFramework(string frameworkId) =>
            await dbContext.Frameworks.FirstOrDefaultAsync(f => f.Id == frameworkId);

        public async Task<IList<EntityFramework.Catalogue.Models.Framework>> GetFrameworks()
            => await dbContext.Frameworks.ToListAsync();

        [ExcludeFromCodeCoverage(Justification = "Can't be tested until the ID migration due to another bizarre Entity Framework design choice where HasDefaultValue doesn't work for the In-memory provider.")]
        public async Task AddFramework(string name, bool isLocalFundingOnly)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var framework =
                new EntityFramework.Catalogue.Models.Framework
                {
                    Name = name, ShortName = name, LocalFundingOnly = isLocalFundingOnly,
                };

            dbContext.Frameworks.Add(framework);
            await dbContext.SaveChangesAsync();
        }

        public async Task MarkAsExpired(string frameworkId)
        {
            var framework = await GetFramework(frameworkId);
            if (framework is null)
                return;

            framework.IsExpired = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> FrameworkNameExists(string frameworkName) =>
            await dbContext.Frameworks.AsNoTracking().AnyAsync(x => x.ShortName == frameworkName);
    }
}
