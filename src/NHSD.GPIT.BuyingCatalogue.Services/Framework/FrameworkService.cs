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
                .Where(x => x.Solution.CatalogueItem.PublishedStatus == PublicationStatus.Published)
                .GroupBy(x => new { x.FrameworkId, x.Framework.ShortName, x.Framework.IsExpired })
                .Select(
                    x => new FrameworkFilterInfo
                    {
                        Id = x.Key.FrameworkId,
                        ShortName = x.Key.ShortName,
                        CountOfActiveSolutions = catalogueItems.Any()
                                        ? x.Count(y => catalogueItems.Contains(y.SolutionId))
                                        : 0,
                        Expired = x.Key.IsExpired,
                    })
                .ToListAsync();

        public async Task<EntityFramework.Catalogue.Models.Framework> GetFramework(string frameworkId) =>
            await dbContext.Frameworks.FirstOrDefaultAsync(f => f.Id == frameworkId);

        public async Task<IList<EntityFramework.Catalogue.Models.Framework>> GetFrameworks()
            => await dbContext.Frameworks.ToListAsync();

        [ExcludeFromCodeCoverage(
            Justification =
                "Can't be tested until the ID migration due to another bizarre Entity Framework design choice where HasDefaultValue doesn't work for the In-memory provider.")]
        public async Task AddFramework(string name, IEnumerable<FundingType> fundingTypes, bool supportsFoundationSolution)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentNullException.ThrowIfNull(fundingTypes);

            var framework =
                new EntityFramework.Catalogue.Models.Framework
                {
                    Name = name, ShortName = name, FundingTypes = fundingTypes.ToArray(), SupportsFoundationSolution = supportsFoundationSolution,
                };

            dbContext.Frameworks.Add(framework);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateFramework(string frameworkId, string name, IEnumerable<FundingType> fundingTypes, bool supportsFoundationSolution)
        {
            var framework = await GetFramework(frameworkId);
            if (framework is null)
                return;
            ArgumentNullException.ThrowIfNullOrEmpty(name);
            ArgumentNullException.ThrowIfNull(fundingTypes);

            framework.Name = name;
            framework.ShortName = name;
            framework.FundingTypes = fundingTypes.ToArray();
            framework.SupportsFoundationSolution = supportsFoundationSolution;
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

        public async Task<bool> FrameworkNameExistsExcludeSelf(string frameworkName, string frameworkId) =>
            await dbContext.Frameworks.AsNoTracking().AnyAsync(x => x.ShortName == frameworkName && x.Id != frameworkId);
    }
}
