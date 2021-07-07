﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class SolutionsService : ISolutionsService
    {
        private const string GpitFuturesFrameworkId = "NHSDGP001";
        private const string DfocvcFrameworkId = "DFOCVC001";

        private readonly GPITBuyingCatalogueDbContext dbContext;
        private readonly IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext> marketingContactRepository;
        private readonly IDbRepository<Solution, GPITBuyingCatalogueDbContext> solutionRepository;
        private readonly IDbRepository<Supplier, GPITBuyingCatalogueDbContext> supplierRepository;
        private readonly ICatalogueItemRepository catalogueItemRepository;

        public SolutionsService(
            GPITBuyingCatalogueDbContext dbContext,
            IDbRepository<MarketingContact, GPITBuyingCatalogueDbContext> marketingContactRepository,
            IDbRepository<Solution, GPITBuyingCatalogueDbContext> solutionRepository,
            IDbRepository<Supplier, GPITBuyingCatalogueDbContext> supplierRepository,
            ICatalogueItemRepository catalogueItemRepository)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.marketingContactRepository = marketingContactRepository ?? throw new ArgumentNullException(nameof(marketingContactRepository));
            this.solutionRepository = solutionRepository ?? throw new ArgumentNullException(nameof(solutionRepository));
            this.supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            this.catalogueItemRepository = catalogueItemRepository;
        }

        public Task<List<CatalogueItem>> GetFuturesFoundationSolutions()
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published
                    && i.Solution.FrameworkSolutions.Any(x => x.IsFoundation)
                    && i.Solution.FrameworkSolutions.Any(x => x.FrameworkId == GpitFuturesFrameworkId))
                .ToListAsync();
        }

        public async Task<List<CatalogueItem>> GetFuturesSolutionsByCapabilities(string[] capabilities)
        {
            var solutions = await dbContext.CatalogueItems
                .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published
                    && i.Solution.FrameworkSolutions.Any(x => x.FrameworkId == GpitFuturesFrameworkId))
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

        public Task<CatalogueItem> GetSolutionListPrices(CatalogueItemId solutionId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.CataloguePrices).ThenInclude(p => p.PricingUnit)
                .Where(i => i.CatalogueItemId == solutionId)
                .SingleOrDefaultAsync();
        }

        public Task<CatalogueItem> GetSolution(CatalogueItemId solutionId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Where(i => i.CatalogueItemId == solutionId)
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetSolutionCapability(CatalogueItemId catalogueItemId, Guid capabilityId)
        {
            catalogueItemId.ValidateNotNull(nameof(catalogueItemId));
            if (capabilityId == Guid.Empty)
                throw new ArgumentException(nameof(capabilityId));

            return await dbContext.CatalogueItems
                .Include(c => c.Solution)
                .ThenInclude(s => s.SolutionCapabilities)
                .ThenInclude(sc => sc.Capability)
                .ThenInclude(sc => sc.Epics)
                .Where(
                    c => c.CatalogueItemId == catalogueItemId
                        && c.Solution.SolutionCapabilities.Any(sc => sc.CapabilityId == capabilityId))
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetSolutionOverview(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
                .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier).ThenInclude(s => s.SupplierContacts)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(i => i.Solution).ThenInclude(s => s.SolutionEpics).ThenInclude(se => se.Status)
                .Include(i => i.Solution).ThenInclude(s => s.SolutionEpics).ThenInclude(se => se.Epic)
                .Include(i => i.CataloguePrices).ThenInclude(p => p.PricingUnit)
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService).Take(1)).ThenInclude(c => c.AssociatedService)
                .Where(i => i.CatalogueItemId == solutionId)
                .FirstOrDefaultAsync();

            var additionalServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.AdditionalService != null).Take(1))
                .ThenInclude(c => c.AdditionalService)
                .Where(i => i.CatalogueItemId == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(additionalServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public async Task<CatalogueItem> GetSolutionWithAllAssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
               .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities)
               .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions)
               .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
               .Include(i => i.Solution).ThenInclude(s => s.SolutionEpics)
               .Include(i => i.CataloguePrices)
               .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService || c.CatalogueItemType == CatalogueItemType.AssociatedService)).ThenInclude(c => c.AssociatedService)
               .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService || c.CatalogueItemType == CatalogueItemType.AssociatedService)).ThenInclude(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
               .Where(i => i.CatalogueItemId == solutionId)
               .SingleOrDefaultAsync();

            var additionalServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.AdditionalService != null).Take(1))
                .ThenInclude(a => a.AdditionalService)
                .Where(i => i.CatalogueItemId == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(additionalServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public async Task<CatalogueItem> GetSolutionWithAllAdditionalServices(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
                .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(i => i.Solution).ThenInclude(s => s.SolutionEpics)
                .Include(i => i.CataloguePrices)
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService || c.CatalogueItemType == CatalogueItemType.AdditionalService)).ThenInclude(c => c.AdditionalService)
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService || c.CatalogueItemType == CatalogueItemType.AdditionalService)).ThenInclude(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Where(i => i.CatalogueItemId == solutionId)
                .SingleOrDefaultAsync();

            var associatedServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.AssociatedService != null).Take(1))
                .ThenInclude(a => a.AssociatedService)
                .Where(i => i.CatalogueItemId == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(associatedServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public Task<List<CatalogueItem>> GetDFOCVCSolutions()
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published
                    && i.Solution.FrameworkSolutions.Any(fs => fs.FrameworkId == DfocvcFrameworkId))
                .ToListAsync();
        }

        public Task<List<Capability>> GetFuturesCapabilities()
        {
            return dbContext.Capabilities.Where(c => c.Category.Name == "GP IT Futures")
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IList<Supplier>> GetAllSuppliers()
        {
            return await dbContext.Suppliers.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task SaveSolutionDescription(CatalogueItemId solutionId, string summary, string description, string link)
        {
            summary.ValidateNotNullOrWhiteSpace(nameof(summary));

            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            solution.Summary = summary;
            solution.FullDescription = description;
            solution.AboutUrl = link;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveSolutionFeatures(CatalogueItemId solutionId, string[] features)
        {
            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            solution.Features = JsonConvert.SerializeObject(features);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveIntegrationLink(CatalogueItemId solutionId, string integrationLink)
        {
            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            solution.IntegrationsUrl = integrationLink;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveImplementationDetail(CatalogueItemId solutionId, string detail)
        {
            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            solution.ImplementationDetail = detail;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveRoadMap(CatalogueItemId solutionId, string roadMap)
        {
            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            solution.RoadMap = roadMap;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<ClientApplication> GetClientApplication(CatalogueItemId solutionId)
        {
            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            return solution.GetClientApplication();
        }

        public async Task SaveClientApplication(CatalogueItemId solutionId, ClientApplication clientApplication)
        {
            clientApplication.ValidateNotNull(nameof(clientApplication));

            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<Hosting> GetHosting(CatalogueItemId solutionId)
        {
            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            return solution.GetHosting();
        }

        public async Task SaveHosting(CatalogueItemId solutionId, Hosting hosting)
        {
            hosting.ValidateNotNull(nameof(hosting));

            var solution = await solutionRepository.SingleAsync(s => s.Id == solutionId);
            solution.Hosting = JsonConvert.SerializeObject(hosting);
            await solutionRepository.SaveChangesAsync();
        }

        public Task<Supplier> GetSupplier(string supplierId)
        {
            supplierId.ValidateNotNullOrWhiteSpace(nameof(supplierId));

            return supplierRepository.SingleAsync(s => s.Id == supplierId);
        }

        public async Task SaveSupplierDescriptionAndLink(string supplierId, string description, string link)
        {
            supplierId.ValidateNotNullOrWhiteSpace(nameof(supplierId));

            var supplier = await supplierRepository.SingleAsync(s => s.Id == supplierId);
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

        public Task<List<CatalogueItem>> GetSupplierSolutions(string supplierId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution).ThenInclude(s => s.SolutionCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.SupplierId == supplierId
                    && i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<IList<CatalogueItem>> GetAllSolutions(PublicationStatus? publicationStatus = null)
        {
            var query = dbContext.CatalogueItems
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution)
                .Include(i => i.Solution)
                .Include(i => i.Supplier)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution)
                .OrderByDescending(i => i.Created)
                .ThenBy(i => i.Name);

            if (publicationStatus == null)
            {
                return await query.ToListAsync();
            }

            return await query
                .Where(i => i.PublishedStatus == publicationStatus.Value)
                .ToListAsync();
        }

        public async Task AddCatalogueSolution(CreateSolutionModel model)
        {
            model.ValidateNotNull(nameof(CreateSolutionModel));
            model.FrameworkModel.ValidateNotNull(nameof(CreateSolutionModel.FrameworkModel));

            var latestCatalogueItemId = await catalogueItemRepository.GetLatestCatalogueItemIdFor(model.SupplierId);
            var catalogueItemId = latestCatalogueItemId.NextSolutionId();

            var nowTime = DateTime.UtcNow;

            var frameworkSolutions = new List<FrameworkSolution>();
            if (model.FrameworkModel.DfocvcFramework)
            {
                frameworkSolutions.Add(new FrameworkSolution
                {
                    FrameworkId = DfocvcFrameworkId,
                    LastUpdated = nowTime,
                    LastUpdatedBy = model.UserId,
                });
            }

            if (model.FrameworkModel.GpitFuturesFramework)
            {
                frameworkSolutions.Add(new FrameworkSolution
                {
                    FrameworkId = GpitFuturesFrameworkId,
                    IsFoundation = model.FrameworkModel.FoundationSolutionFramework,
                    LastUpdated = nowTime,
                    LastUpdatedBy = model.UserId,
                });
            }

            catalogueItemRepository.Add(
                new CatalogueItem
                {
                    CatalogueItemId = catalogueItemId,
                    CatalogueItemType = CatalogueItemType.Solution,
                    Solution =
                        new Solution
                        {
                            FrameworkSolutions = frameworkSolutions,
                            LastUpdated = nowTime,
                            LastUpdatedBy = model.UserId,
                        },
                    Name = model.Name,
                    PublishedStatus = PublicationStatus.Draft,
                    SupplierId = model.SupplierId,
                });

            await catalogueItemRepository.SaveChangesAsync();
        }

        public Task<bool> SupplierHasSolutionName(string supplierId, string solutionName) =>
            catalogueItemRepository.SupplierHasSolutionName(supplierId, solutionName);
    }
}
