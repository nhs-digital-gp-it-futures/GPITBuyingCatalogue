using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public class SolutionsService : ISolutionsService
    {
        private readonly ILogger<SolutionsService> _logger;
        private readonly BuyingCatalogueDbContext _dbContext;

        public SolutionsService(ILogger<SolutionsService> logger, BuyingCatalogueDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<CatalogueItem>> GetFuturesFoundationSolutions()
        {
            var foundationSolutions = await _dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(x => x.PublishedStatus.Name == "Published" 
                    && x.Solution.FrameworkSolutions.Any(x => x.IsFoundation)
                    && x.Solution.FrameworkSolutions.Any( x=> x.FrameworkId == "NHSDGP001"))
                .ToListAsync();

            return foundationSolutions;
        }

        public async Task<List<CatalogueItem>> GetDFOCVCSolutions()
        {
            var dfocvcSolutions = await _dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(x => x.PublishedStatus.Name == "Published"                    
                    && x.Solution.FrameworkSolutions.Any(x => x.FrameworkId == "DFOCVC001"))
                .ToListAsync();

            return dfocvcSolutions;
        }
    }
}
