﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnumsNET;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class SolutionsService : ISolutionsService
    {
        private const string FuturesFrameworkShortName = "GP IT Futures";
        private const string DfocvcShortName = "DFOCVC";

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IDbRepository<MarketingContact, BuyingCatalogueDbContext> marketingContactRepository;
        private readonly IDbRepository<Solution, BuyingCatalogueDbContext> solutionRepository;
        private readonly IDbRepository<Supplier, BuyingCatalogueDbContext> supplierRepository;
        private readonly ICatalogueItemRepository catalogueItemRepository;
        private readonly IMemoryCache memoryCache;

        public SolutionsService(
            BuyingCatalogueDbContext dbContext,
            IDbRepository<MarketingContact, BuyingCatalogueDbContext> marketingContactRepository,
            IDbRepository<Solution, BuyingCatalogueDbContext> solutionRepository,
            IDbRepository<Supplier, BuyingCatalogueDbContext> supplierRepository,
            ICatalogueItemRepository catalogueItemRepository,
            IMemoryCache memoryCache)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.marketingContactRepository = marketingContactRepository ?? throw new ArgumentNullException(nameof(marketingContactRepository));
            this.solutionRepository = solutionRepository ?? throw new ArgumentNullException(nameof(solutionRepository));
            this.supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
            this.catalogueItemRepository = catalogueItemRepository ?? throw new ArgumentNullException(nameof(catalogueItemRepository));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task<List<CatalogueItem>> GetFuturesFoundationSolutions()
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published
                    && i.Solution.FrameworkSolutions.Any(x => x.IsFoundation)
                    && i.Solution.FrameworkSolutions.Any(fs => fs.Framework.ShortName == FuturesFrameworkShortName))
                .ToListAsync();
        }

        public async Task<List<CatalogueItem>> GetFuturesSolutionsByCapabilities(string[] capabilities)
        {
            var solutions = await dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(s => s.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published
                    && i.Solution.FrameworkSolutions.Any(fs => fs.Framework.ShortName == FuturesFrameworkShortName))
                .ToListAsync();

            // TODO - Refactor this. Should be possible to include in the above expression
            if (capabilities?.Length > 0)
            {
                solutions = solutions.Where(
                        solution => capabilities.All(
                            capability =>
                                solution.CatalogueItemCapabilities.Any(
                                    x => x.Capability.CapabilityRef == capability)))
                    .ToList();
            }

            return solutions;
        }

        public Task<CatalogueItem> GetSolutionListPrices(CatalogueItemId solutionId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.CataloguePrices).ThenInclude(p => p.PricingUnit)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();
        }

        public Task<CatalogueItem> GetSolution(CatalogueItemId solutionId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(i => i.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Include(i => i.SupplierServiceAssociations)
                .Where(i => i.Id == solutionId)
                .FirstOrDefaultAsync();
        }

        public Task<CatalogueItem> GetSolutionByName(string solutionName)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Include(i => i.Supplier)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Where(i => i.Name == solutionName)
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetSolutionCapability(CatalogueItemId catalogueItemId, int capabilityId)
        {
            return await dbContext.CatalogueItems
                .Include(ci => ci.Solution)
                .Include(ci => ci.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability).ThenInclude(c => c.Epics)
                .Where(
                    c => c.Id == catalogueItemId
                        && c.CatalogueItemCapabilities.Any(sc => sc.CapabilityId == capabilityId))
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetSolutionOverview(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
                .Include(s => s.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier).ThenInclude(s => s.SupplierContacts)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions).ThenInclude(fs => fs.Framework)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(s => s.CatalogueItemEpics).ThenInclude(se => se.Status)
                .Include(s => s.CatalogueItemEpics).ThenInclude(se => se.Epic)
                .Include(i => i.CataloguePrices).ThenInclude(p => p.PricingUnit)
                .Where(i => i.Id == solutionId)
                .FirstOrDefaultAsync();

            var associatedServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.AssociatedService != null).Take(1))
                .ThenInclude(c => c.AssociatedService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(associatedServices.Supplier?.CatalogueItems?.FirstOrDefault());

            var additionalServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.AdditionalService != null).Take(1))
                .ThenInclude(c => c.AdditionalService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(additionalServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public async Task<CatalogueItem> GetSolutionWithAllAssociatedServices(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
               .Include(s => s.CatalogueItemCapabilities)
               .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions)
               .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
               .Include(s => s.CatalogueItemEpics)
               .Include(i => i.CataloguePrices)
               .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService || c.CatalogueItemType == CatalogueItemType.AssociatedService)).ThenInclude(c => c.AssociatedService)
               .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService || c.CatalogueItemType == CatalogueItemType.AssociatedService)).ThenInclude(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
               .Where(i => i.Id == solutionId)
               .SingleOrDefaultAsync();

            var additionalServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService && c.AdditionalService != null).Take(1))
                .ThenInclude(a => a.AdditionalService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(additionalServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public async Task<CatalogueItem> GetSolutionAdditionalServiceCapabilities(CatalogueItemId id)
        {
            var catalogueItem = await dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities)
                .ThenInclude(cic => cic.Capability)
                .ThenInclude(c => c.Epics)
                .Where(i => i.Id == id)
                .SingleAsync();

            return catalogueItem;
        }

        public async Task<CatalogueItem> GetAdditionalServiceCapability(
            CatalogueItemId catalogueItemId,
            int capabilityId)
        {
            var catalogueItem = await dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities.Where(cic => cic.CapabilityId == capabilityId))
                .ThenInclude(cic => cic.Capability)
                .ThenInclude(c => c.Epics)
                .Where(i => i.Id == catalogueItemId)
                .SingleAsync();

            catalogueItem.CatalogueItemCapabilities ??= Array.Empty<CatalogueItemCapability>();

            return catalogueItem;
        }

        public async Task<CatalogueItem> GetSolutionWithAllAdditionalServices(CatalogueItemId solutionId)
        {
            var solution = await dbContext.CatalogueItems
                .Include(i => i.CatalogueItemCapabilities)
                .Include(i => i.Solution).ThenInclude(s => s.FrameworkSolutions)
                .Include(i => i.Solution).ThenInclude(s => s.MarketingContacts)
                .Include(s => s.CatalogueItemEpics)
                .Include(i => i.CataloguePrices)
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService || c.CatalogueItemType == CatalogueItemType.AdditionalService)).ThenInclude(c => c.AdditionalService)
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AdditionalService || c.CatalogueItemType == CatalogueItemType.AdditionalService)).ThenInclude(c => c.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            var associatedServices = await dbContext.CatalogueItems
                .Include(i => i.Supplier).ThenInclude(s => s.CatalogueItems.Where(c => c.CatalogueItemType == CatalogueItemType.AssociatedService && c.AssociatedService != null).Take(1))
                .ThenInclude(a => a.AssociatedService)
                .Where(i => i.Id == solutionId)
                .SingleOrDefaultAsync();

            solution?.Supplier?.CatalogueItems.Add(associatedServices.Supplier?.CatalogueItems?.FirstOrDefault());

            return solution;
        }

        public Task<List<CatalogueItem>> GetDFOCVCSolutions()
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published
                    && i.Solution.FrameworkSolutions.Any(fs => fs.Framework.ShortName == DfocvcShortName))
                .ToListAsync();
        }

        public Task<List<Capability>> GetFuturesCapabilities()
        {
            return dbContext.Capabilities.Where(c => c.Category.Name == FuturesFrameworkShortName)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task SaveSolutionDescription(CatalogueItemId solutionId, string summary, string description, string link)
        {
            summary.ValidateNotNullOrWhiteSpace(nameof(summary));

            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.Summary = summary;
            solution.FullDescription = description;
            solution.AboutUrl = link;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveSolutionFeatures(CatalogueItemId solutionId, string[] features)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.Features = JsonConvert.SerializeObject(features);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveImplementationDetail(CatalogueItemId solutionId, string detail)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.ImplementationDetail = detail;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task SaveRoadMap(CatalogueItemId solutionId, string roadMap)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.RoadMap = roadMap;
            await solutionRepository.SaveChangesAsync();
        }

        public async Task<ClientApplication> GetClientApplication(CatalogueItemId solutionId)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            return solution.GetClientApplication();
        }

        public async Task SaveClientApplication(CatalogueItemId solutionId, ClientApplication clientApplication)
        {
            clientApplication.ValidateNotNull(nameof(clientApplication));

            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.ClientApplication = JsonConvert.SerializeObject(clientApplication);
            await solutionRepository.SaveChangesAsync();
        }

        public async Task DeleteClientApplication(CatalogueItemId solutionId, ClientApplicationType clientApplicationType)
        {
            var clientApplication = await GetClientApplication(solutionId);

            RemoveClientApplicationType(clientApplication, clientApplicationType);

            await SaveClientApplication(solutionId, clientApplication);
        }

        public ClientApplication RemoveClientApplicationType(ClientApplication clientApplication, ClientApplicationType clientApplicationType)
        {
            if (clientApplication.ClientApplicationTypes != null)
            {
                if (clientApplication.ClientApplicationTypes.Contains(clientApplicationType.AsString(EnumFormat.EnumMemberValue)))
                    clientApplication.ClientApplicationTypes.Remove(clientApplicationType.AsString(EnumFormat.EnumMemberValue));
            }

            if (clientApplicationType == ClientApplicationType.BrowserBased)
            {
                clientApplication.AdditionalInformation = null;
                clientApplication.BrowsersSupported = null;
                clientApplication.HardwareRequirements = null;
                clientApplication.MinimumConnectionSpeed = null;
                clientApplication.MinimumDesktopResolution = null;
                clientApplication.MobileFirstDesign = null;
                clientApplication.MobileResponsive = null;
                clientApplication.Plugins = null;
            }
            else if (clientApplicationType == ClientApplicationType.Desktop)
            {
                clientApplication.NativeDesktopAdditionalInformation = null;
                clientApplication.NativeDesktopHardwareRequirements = null;
                clientApplication.NativeDesktopMemoryAndStorage = null;
                clientApplication.NativeDesktopMinimumConnectionSpeed = null;
                clientApplication.NativeDesktopOperatingSystemsDescription = null;
                clientApplication.NativeDesktopThirdParty = null;
            }
            else
            {
                clientApplication.MobileConnectionDetails = null;
                clientApplication.MobileMemoryAndStorage = null;
                clientApplication.MobileOperatingSystems = null;
                clientApplication.MobileThirdParty = null;
                clientApplication.NativeMobileAdditionalInformation = null;
                clientApplication.NativeMobileFirstDesign = null;
                clientApplication.NativeMobileHardwareRequirements = null;
            }

            return clientApplication;
        }

        public async Task<Hosting> GetHosting(CatalogueItemId solutionId)
        {
            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            return solution.GetHosting();
        }

        public async Task SaveHosting(CatalogueItemId solutionId, Hosting hosting)
        {
            hosting.ValidateNotNull(nameof(hosting));

            var solution = await solutionRepository.SingleAsync(s => s.CatalogueItemId == solutionId);
            solution.Hosting = JsonConvert.SerializeObject(hosting);
            await solutionRepository.SaveChangesAsync();
        }

        public Task<Supplier> GetSupplier(int supplierId) => supplierRepository.SingleAsync(s => s.Id == supplierId);

        public async Task SaveSupplierDescriptionAndLink(int supplierId, string description, string link)
        {
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

        public Task<List<CatalogueItem>> GetSupplierSolutions(int? supplierId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Where(i => i.SupplierId == supplierId.GetValueOrDefault()
                    && i.CatalogueItemType == CatalogueItemType.Solution
                    && i.PublishedStatus == PublicationStatus.Published)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<IList<CatalogueItem>> GetAllSolutions(
            PublicationStatus? publicationStatus = null)
        {
            var query = dbContext.CatalogueItems
                .Include(i => i.Solution)
                .Include(i => i.Supplier)
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(cic => cic.Capability)
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution);

            if (publicationStatus is not null)
                query = query.Where(i => i.PublishedStatus == publicationStatus.Value);

            return await query
                .OrderByDescending(i => i.Created)
                .ThenBy(i => i.Name)
                .ToListAsync();
        }

        public async Task<CatalogueItemId> AddCatalogueSolution(CreateSolutionModel model)
        {
            model.ValidateNotNull(nameof(CreateSolutionModel));
            model.Frameworks.ValidateNotNull(nameof(CreateSolutionModel.Frameworks));

            var latestCatalogueItemId = await catalogueItemRepository.GetLatestCatalogueItemIdFor(model.SupplierId);
            var catalogueItemId = latestCatalogueItemId.NextSolutionId();

            var dateTimeNow = DateTime.UtcNow;

            var frameworkSolutions = new List<FrameworkSolution>();

            foreach (var framework in model.Frameworks.Where(f => f.Selected))
            {
                frameworkSolutions.Add(new FrameworkSolution
                {
                    FrameworkId = framework.FrameworkId,
                    IsFoundation = framework.IsFoundation,
                    LastUpdated = dateTimeNow,
                    LastUpdatedBy = model.UserId,
                });
            }

            catalogueItemRepository.Add(new CatalogueItem
            {
                Id = catalogueItemId,
                CatalogueItemType = CatalogueItemType.Solution,
                Solution =
                        new Solution
                        {
                            FrameworkSolutions = frameworkSolutions,
                            LastUpdated = dateTimeNow,
                            LastUpdatedBy = model.UserId,
                        },
                Name = model.Name,
                PublishedStatus = PublicationStatus.Draft,
                SupplierId = model.SupplierId,
            });

            await catalogueItemRepository.SaveChangesAsync();

            return catalogueItemId;
        }

        public async Task<IList<EntityFramework.Catalogue.Models.Framework>> GetAllFrameworks()
        {
            return await dbContext.Frameworks.ToListAsync();
        }

        public Task<bool> SupplierHasSolutionName(int supplierId, string solutionName) =>
            catalogueItemRepository.SupplierHasSolutionName(supplierId, solutionName);
    }
}
