using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public class SolutionsService : ISolutionsService
    {
        private const string GpitFuturesFrameworkId = "NHSDGP001";
        private const string DfocvcFrameworkId = "DFOCVC001";

        private readonly GPITBuyingCatalogueDbContext dbContext;
        private readonly IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext> marketingContactRepository;
        private readonly IDbRepository<Solution, GPITBuyingCatalogueDbContext> solutionRepository;
        private readonly IDbRepository<Supplier, GPITBuyingCatalogueDbContext> supplierRepository;

        public SolutionsService(
            GPITBuyingCatalogueDbContext dbContext,
            IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext> marketingContactRepository,
            IDbRepository<Solution, GPITBuyingCatalogueDbContext> solutionRepository,
            IDbRepository<Supplier, GPITBuyingCatalogueDbContext> supplierRepository)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.marketingContactRepository = marketingContactRepository ?? throw new ArgumentNullException(nameof(marketingContactRepository));
            this.solutionRepository = solutionRepository ?? throw new ArgumentNullException(nameof(solutionRepository));
            this.supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
        }

        public async Task<List<CatalogueItem>> GetFuturesFoundationSolutions()
        {
            return await dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(
                    x => x.CatalogueItemType.Name == "Solution"
                        && x.PublishedStatus.Name == "Published"
                        && x.Solution.FrameworkSolutions.Any(x => x.IsFoundation)
                        && x.Solution.FrameworkSolutions.Any(x => x.FrameworkId == GpitFuturesFrameworkId))
                .ToListAsync();
        }

        public async Task<List<CatalogueItem>> GetFuturesSolutionsByCapabilities(string[] capabilities)
        {
            var solutions = await dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(
                    x => x.CatalogueItemType.Name == "Solution"
                        && x.PublishedStatus.Name == "Published"
                        && x.Solution.FrameworkSolutions.Any(x => x.FrameworkId == GpitFuturesFrameworkId))
                .ToListAsync();

            // TODO - Refactor this. Should be possible to include in the above expression
            if (capabilities?.Length > 0)
            {
                solutions = solutions.Where(
                        solution => capabilities.All(
                            capability =>
                                solution.Solution.SolutionCapabilities.Any(
                                    x => x.Capability.CapabilityRef == capability)))
                    .ToList();
            }

            return solutions;
        }

        public async Task<CatalogueItem> GetSolutionListPrices(string solutionId)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            return await dbContext.CatalogueItems
                .Include(x => x.CataloguePrices)
                .ThenInclude(x => x.ProvisioningType)
                .Include(x => x.CataloguePrices)
                .ThenInclude(x => x.CataloguePriceType)
                .Include(x => x.CataloguePrices)
                .ThenInclude(x => x.TimeUnit)
                .Where(x => x.CatalogueItemId == solutionId)
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetSolution(string solutionId)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            return await dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Solution)
                .ThenInclude(x => x.FrameworkSolutions)
                .ThenInclude(x => x.Framework)
                .Include(x => x.Solution)
                .ThenInclude(x => x.MarketingContacts)
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionEpics)
                .ThenInclude(x => x.Status)
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionEpics)
                .ThenInclude(x => x.Epic)
                .ThenInclude(x => x.CompliancyLevel)
                .Include(x => x.CataloguePrices)
                .ThenInclude(x => x.ProvisioningType)
                .Include(x => x.CataloguePrices)
                .ThenInclude(x => x.CataloguePriceType)
                .Include(x => x.CataloguePrices)
                .ThenInclude(x => x.TimeUnit)
                .Include(x => x.CataloguePrices)
                .ThenInclude(x => x.PricingUnit)
                .Include(x => x.Supplier)
                .ThenInclude(s => s.CatalogueItems)
                .ThenInclude(c => c.AssociatedService)
                .Include(x => x.Supplier)
                .ThenInclude(s => s.CatalogueItems)
                .ThenInclude(c => c.CataloguePrices)
                .ThenInclude(cp => cp.PricingUnit)
                .Where(x => x.CatalogueItemId == solutionId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CatalogueItem>> GetDFOCVCSolutions()
        {
            return await dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(
                    x => x.CatalogueItemType.Name == "Solution"
                        && x.PublishedStatus.Name == "Published"
                        && x.Solution.FrameworkSolutions.Any(y => y.FrameworkId == DfocvcFrameworkId))
                .ToListAsync();
        }

        public async Task<List<Capability>> GetFuturesCapabilities()
        {
            return await dbContext.Capabilities.Where(x => x.Category.Name == "GP IT Futures")
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task SaveSolutionDescription(string solutionId, string summary, string description, string link)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));
            summary.ValidateNotNullOrWhiteSpace(nameof(summary));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            solution.Summary = summary;
            solution.FullDescription = description;
            solution.AboutUrl = link;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveSolutionFeatures(string solutionId, string[] features)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            solution.Features = JsonConvert.SerializeObject(features);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveIntegrationLink(string solutionId, string integrationLink)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            solution.IntegrationsUrl = integrationLink;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveImplementationDetail(string solutionId, string detail)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            solution.ImplementationDetail = detail;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveRoadmap(string solutionId, string roadmap)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            solution.RoadMap = roadmap;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<ClientApplication> GetClientApplication(string solutionId)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            return solution.GetClientApplication();
        }

        public async Task SaveClientApplication(string solutionId, ClientApplication clientApplication)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));
            clientApplication.ValidateNotNull(nameof(clientApplication));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<Hosting> GetHosting(string solutionId)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            return solution.GetHosting();
        }

        public async Task SaveHosting(string solutionId, Hosting hosting)
        {
            solutionId.ValidateNotNullOrWhiteSpace(nameof(solutionId));
            hosting.ValidateNotNull(nameof(hosting));

            var solution = await solutionRepository.SingleAsync(x => x.Id == solutionId);
            solution.Hosting = JsonConvert.SerializeObject(hosting);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<Supplier> GetSupplier(string supplierId)
        {
            supplierId.ValidateNotNullOrWhiteSpace(nameof(supplierId));

            return await supplierRepository.SingleAsync(x => x.Id == supplierId);
        }

        public async Task SaveSupplierDescriptionAndLink(string supplierId, string description, string link)
        {
            supplierId.ValidateNotNullOrWhiteSpace(nameof(supplierId));

            var supplier = await supplierRepository.SingleAsync(x => x.Id == supplierId);
            supplier.Summary = description;
            supplier.SupplierUrl = link;
            await supplierRepository.SaveChangesAsync();
        }

        public async Task SaveSupplierContacts(SupplierContactsModel model)
        {
            model.ValidateNotNull(nameof(model));

            model.SetSolutionId();

            var marketingContacts = await marketingContactRepository.GetAllAsync(x => x.SolutionId == model.SolutionId);

            if (!marketingContacts.Any())
            {
                marketingContactRepository.AddAll(model.ValidContacts());
            }
            else
            {
                foreach (var contact in marketingContacts)
                {
                    if (model.ContactFor(contact.Id) is not { } newContact)
                        continue;

                    if (newContact.IsEmpty())
                        marketingContactRepository.Remove(contact);
                    else
                        contact.UpdateFrom(newContact);
                }

                marketingContactRepository.AddAll(model.NewAndValidContacts());
            }

            await marketingContactRepository.SaveChangesAsync();
        }

        public async Task<List<CatalogueItem>> GetSupplierSolutions(string supplierId)
        {
            return await dbContext.CatalogueItems
                .Include(x => x.Solution)
                .ThenInclude(x => x.SolutionCapabilities)
                .ThenInclude(x => x.Capability)
                .Include(x => x.Supplier)
                .Where(
                    x => x.SupplierId == supplierId
                        && x.CatalogueItemType.Name == "Solution"
                        && x.PublishedStatus.Name == "Published")
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
