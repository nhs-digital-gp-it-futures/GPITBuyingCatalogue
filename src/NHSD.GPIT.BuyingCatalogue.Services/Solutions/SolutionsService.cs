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
                .Where(x => x.CatalogueItemType.Name == "Solution"
                    && x.PublishedStatus.Name == "Published" 
                    && x.Solution.FrameworkSolutions.Any(x => x.IsFoundation)
                    && x.Solution.FrameworkSolutions.Any( x=> x.FrameworkId == "NHSDGP001"))
                .ToListAsync();

            return foundationSolutions;
        }

        public async Task<List<CatalogueItem>> GetFuturesSolutionsByCapabilities(string[] capabilities)
        {
            var solutions = await _dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(x => x.CatalogueItemType.Name == "Solution" 
                    && x.PublishedStatus.Name == "Published"                    
                    && x.Solution.FrameworkSolutions.Any(x => x.FrameworkId == "NHSDGP001"))
                .ToListAsync();
            
            // TODO - Refactor this. Should be posible to include in the above expression
            if(capabilities?.Length > 0)
            {
                var filteredSolutions = new List<CatalogueItem>();

                foreach( var solution in solutions )
                {
                    bool matched = true;

                    foreach( var capability in capabilities )
                    {
                        if( !solution.Solution.SolutionCapabilities.Any(x=>x.Capability.CapabilityRef == capability ))
                        {
                            matched = false;
                            break;
                        }
                    }

                    if (matched)
                        filteredSolutions.Add(solution);
                }

                solutions = filteredSolutions;
            }

            return solutions;
        }

        public async Task<CatalogueItem> GetSolution(string id)
        {
            var solution = await _dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)                
                .Include(x => x.Supplier)                
                .Include(x => x.Solution)
                .ThenInclude(x => x.FrameworkSolutions)
                .ThenInclude(x => x.Framework)
                .Include(x => x.Solution)
                .ThenInclude(x => x.MarketingContacts)
                .Where(x => x.CatalogueItemId == id)        
                .FirstOrDefaultAsync();

            return solution;
        }

        public async Task<List<CatalogueItem>> GetDFOCVCSolutions()
        {
            var dfocvcSolutions = await _dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(x => x.CatalogueItemType.Name == "Solution" 
                    && x.PublishedStatus.Name == "Published"                    
                    && x.Solution.FrameworkSolutions.Any(x => x.FrameworkId == "DFOCVC001"))
                .ToListAsync();

            return dfocvcSolutions;
        }

        public async Task<List<Capability>> GetFuturesCapabilities()
        {
            var capabilities = await _dbContext.Capabilities.Where(x=>x.Category.Name == "GP IT Futures").OrderBy(x=>x.Name).ToListAsync();

            return capabilities;
        }
    }
}
