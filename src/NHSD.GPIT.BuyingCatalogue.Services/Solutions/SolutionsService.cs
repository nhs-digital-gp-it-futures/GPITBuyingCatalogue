﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
                .FirstAsync();

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

        public async Task SaveSolutionDescription(string id, string summary, string description, string link)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == id);

            solution.Summary = summary;
            solution.FullDescription = description;
            solution.AboutUrl = link;

            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveSolutionFeatures(string id, string featuresJson)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == id);

            solution.Features = featuresJson;

            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveIntegrationLink(string id, string integrationLink)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == id);

            solution.IntegrationsUrl = integrationLink;

            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveImplementationDetail(string id, string detail)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == id);

            solution.ImplementationDetail = detail;

            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveRoadmap(string id, string roadmap)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == id);

            solution.RoadMap = roadmap;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<ClientApplication> GetClientApplication(string solutionId)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == solutionId);

            var clientApplication = new ClientApplication();

            if (!string.IsNullOrWhiteSpace(solution.ClientApplication))
                clientApplication = JsonConvert.DeserializeObject<ClientApplication>(solution.ClientApplication);

            return clientApplication;
        }

        public async Task SaveClientApplication(string solutionId, ClientApplication clientApplication)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == solutionId);

            var json = JsonConvert.SerializeObject(clientApplication);

            solution.ClientApplication = json;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Hosting> GetHosting(string solutionId)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == solutionId);

            var hosting = new Hosting();

            if (!string.IsNullOrWhiteSpace(solution.Hosting))
                hosting = JsonConvert.DeserializeObject<Hosting>(solution.Hosting);

            return hosting;
        }

        public async Task SaveHosting(string solutionId, Hosting hosting)
        {
            var solution = await _dbContext.Solutions.SingleAsync(x => x.Id == solutionId);

            var json = JsonConvert.SerializeObject(hosting);

            solution.Hosting = json;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Supplier> GetSupplier(string supplierId)
        {
            var supplier = await _dbContext.Suppliers.SingleAsync(x => x.Id == supplierId);

            return supplier;
        }

        public async Task SaveSupplierDescriptionAndLink(string supplierId, string description, string link)
        {
            var supplier = await _dbContext.Suppliers.SingleAsync(x => x.Id == supplierId);

            supplier.Summary = description;
            supplier.SupplierUrl = link;

            await _dbContext.SaveChangesAsync();
        }
    }
}
